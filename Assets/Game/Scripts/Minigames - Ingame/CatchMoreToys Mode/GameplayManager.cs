using AnhNV.GameBase;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WFSport.Helper;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] Transform gameHolder;
        [SerializeField] Canvas bgCanvas;
        [SerializeField] GameplayConfig config;
        [SerializeField] ThrowingMachine[] throwMachines;
        [SerializeField] GameObject[] itemData;
        [SerializeField] GameObject[] bonusItemData;
        [SerializeField] GameObject[] obstacleData;
        [SerializeField] CharacterWorldAnimation[] characters;
        [SerializeField] Player player; 

        private MinigameUI ui;
        private ThrowingMachine curCharacter;
        private (int count, int index) obstacleSpawner;
        private (int count, int index) bonusItemSpawner;
        private Tutorial tutorial;
        private TutorialSwipe catchToyStep;
        private float finalScore;


        private IMinigame.ConfigData myData;
        private IMinigame.ResultData result;
        public IMinigame.ConfigData InternalData { get => myData; set => myData = value; }
        IMinigame.ResultData IMinigame.ExternalData { get => result; set => result = value; }

        private void Awake()
        {
            ui = FindAnyObjectByType<SoloMinigameUI>(FindObjectsInactive.Include);
            ui.gameObject.SetActive(true);
            bgCanvas.worldCamera = Camera.main;

            /// Setup Object to Fit screen
            if (Camera.main.aspect <= (1.35f))
            {
                //    Debug.Log("4:3");
                gameHolder.localScale = config.ipadSize;
            }
            else
            {
                //     Debug.Log("3:2");
                gameHolder.localScale = config.normalSize;
            }
        }
        private void Start()
        {
            Init();
            SetupTutorial();
            PlayTutorial();

            EventManager.OnPlayerClaimNewStar += OnPlayerCollectStar;
            EventManager.OnFinishStage += OnGameWining;
            EventManager.OnTimeOut += OnGameLosing;
        }
        private void OnDestroy()
        {
            EventManager.OnPlayerClaimNewStar -= OnPlayerCollectStar;
            EventManager.OnFinishStage -= OnGameWining;
            EventManager.OnTimeOut -= OnGameLosing;
        }
        private void OnEndgame(IMinigame.MatchResult matchResult)
        {
            result.gamestate = matchResult;
            EventDispatcher.Instance.Dispatch(new EventKey.OnGameStop { data = result });
        }

        public void OnGameWining()
        {
            ui.PauseTime();
            OnGamePause();
            // On Endgame
            OnEndgame(IMinigame.MatchResult.Win);
        }

        public void OnGameLosing()
        {
            ui.PauseTime();
            OnGamePause();
            OnEndgame(IMinigame.MatchResult.Lose);
        }

        private void PlayTutorial()
        {
            PlayNextCharacter(config.readyMachineIdx);
        }

        private void OnTutorialCompleted()
        {
            catchToyStep.Completed();
            OnGamePause();
            StartCoroutine("DelayToPlaygame");
        }
        private IEnumerator DelayToPlaygame()
        {
            yield return new WaitForSeconds(2);


            ui.OpenLoading(() =>
            {
                ui.OpenCountingToStart(() =>
                {
                    OnGameStart();
                });
            },
            () =>
            {
                ResetDefault();
            });
        }

        private void SetupTutorial()
        {
            var tutorialController = TutorialController.Instance;

            tutorial = tutorialController.CreateTutorial("CatchMoreToys");
            catchToyStep = tutorialController.CreateStep<TutorialSwipe>(tutorial);
            catchToyStep.Setup(player.transform, AnimatorHelper.Direction.Left);
            catchToyStep.OnSwipeCorrectDirection += OnSwipeCorrect;

            foreach (var item in throwMachines)
            {
                item.SetupTutorial();
            }
            player.Pause(false);

            EventManager.OnToyIsFlying += OnToyIsFlying;
        }
        private void OnSwipeCorrect()
        {
            catchToyStep.OnSwipeCorrectDirection -= OnSwipeCorrect;
            catchToyStep.Stop();
            OnTutorialResume();
        }

        private void OnTutorialResume()
        {
            player.Play();
            foreach (var item in throwMachines)
            {
                item.ResumeThrow();
            }
            EventDispatcher.Instance.Dispatch(new EventKey.OnGameResume { catchMoreToys = this });
        }

        private void OnToyIsFlying(Item item)
        {
            var distance = Vector2.Distance(item.transform.position, player.transform.position);
            if(distance < 5)
            {
                EventManager.OnToyIsFlying -= OnToyIsFlying;
                PlayTutorialNextStep();
            }
        }

        private void PlayTutorialNextStep()
        {
            /// Tutorial is Blur background & Focus to Player
            OnGamePause();
            catchToyStep.Play();
            player.Play();
        }

        void ResetDefault()
        {
            player.ResetDefault();
            foreach (var item in throwMachines)
            {
                item.ResetDefault();
            }
        }

        void Init()
        {
            result = new IMinigame.ResultData();
            myData = DataTransporter.GameplayConfig;
            if(myData == null)
            {
                myData = new IMinigame.ConfigData()
                {
                    playTime = 60,
                    timelineScore = new float[] { 15, 30, 45 }
                };
            }
            finalScore = myData.timelineScore[myData.timelineScore.Length - 1];
            ui.Setup(myData.playTime, myData.timelineScore);

            /// Spawn character in ThrowingMachine
            foreach (var machine in throwMachines)
            {
                var idx = UnityEngine.Random.Range(0, characters.Length);
                var character = Instantiate(characters[idx], machine.transform);
                machine.Setup(transform, this, config, character);
            }
        }

        private void OnPlayerCollectStar(Base.Player obj, bool isSpecial)
        {
            if(!tutorial.IsAllStepCompleted)
            {
                OnTutorialCompleted();
                return;
            }

            ui.UpdateLoadingBar(player.Score / finalScore);

            result.claimedCoin += isSpecial ? myData.specialPlusCoin : myData.normalPlusCoin;
            if (ui.TotalStarClaimed > 0 && player.Score == myData.timelineScore[ui.TotalStarClaimed - 1])
            {
                result.claimedCoin += myData.milestoneCoin;
            }
        }

        private IEnumerator CountSpawnTime()
        {
            bonusItemSpawner.count++;
            obstacleSpawner.count++;
            yield return new WaitForSeconds(1);
            StartCoroutine("CountSpawnTime");
        }

        internal (GameObject prefab, Sprite sprite) GetRandomItem()
        {
            var rd = UnityEngine.Random.Range(0, 100);
            if(rd <= config.itemRandomPercentage)
            {
                // spawn Item
                var idx = UnityEngine.Random.Range(0, itemData.Length);
                return (itemData[idx], itemData[idx].GetComponentInChildren<SpriteRenderer>().sprite);
            }
            else if(rd <= config.bonusitemRandomPercentage)
            {
                // spawn Bonus Item
                var idx = UnityEngine.Random.Range(0, bonusItemData.Length);
                return (bonusItemData[idx], bonusItemData[idx].GetComponentInChildren<SpriteRenderer>().sprite);
            }
            else
            {
                // spawn Obstacle
                var idx = UnityEngine.Random.Range(0, obstacleData.Length);
                return (obstacleData[idx], obstacleData[idx].GetComponentInChildren<SpriteRenderer>().sprite);
            }
        }
        internal (GameObject prefab, Sprite sprite) GetAutoNextItem()
        {
            int idx;
            if(bonusItemSpawner.index < config.bonusItemSpawnTime.Length &&
                bonusItemSpawner.count >= config.bonusItemSpawnTime[bonusItemSpawner.index])
            {
                bonusItemSpawner.index++;
                // spawn Bonus Item
                idx = UnityEngine.Random.Range(0, bonusItemData.Length);
                return (bonusItemData[idx], bonusItemData[idx].GetComponentInChildren<SpriteRenderer>().sprite);
            }

            if(obstacleSpawner.index < config.obstacleSpawnTime.Length &&
                obstacleSpawner.count >= config.obstacleSpawnTime[obstacleSpawner.index])
            {
                obstacleSpawner.index++;
                // spawn Obstacle
                idx = UnityEngine.Random.Range(0, obstacleData.Length);
                return (obstacleData[idx], obstacleData[idx].GetComponentInChildren<SpriteRenderer>().sprite);
            }

            // spawn Item
            idx = UnityEngine.Random.Range(0, itemData.Length);
            return (itemData[idx], itemData[idx].GetComponentInChildren<SpriteRenderer>().sprite);
        }

        public void OnGamePause()
        {
            foreach (var item in throwMachines)
            {
                item.StopThrow();
            }
            ui.PauseTime();
            StopCoroutine("CountSpawnTime");
            EventDispatcher.Instance.Dispatch(new Gameplay.EventKey.OnGamePause { catchMoreToys = this });
        }

        public void OnGameResume()
        {
            ui.PlayTime();
            StartCoroutine("CountSpawnTime");
            foreach (var item in throwMachines)
            {
                item.ResumeThrow();
            }
            EventDispatcher.Instance.Dispatch(new Gameplay.EventKey.OnGameResume { catchMoreToys = this });
        }

        private void PlayNextCharacter()
        {
            var idx = UnityEngine.Random.Range(0, throwMachines.Length);
            curCharacter = throwMachines[idx];
            curCharacter.Throw(() =>
            {
                PlayNextCharacter();
            });
        }
        private void PlayNextCharacter(int idx)
        {
            curCharacter = throwMachines[idx];
            curCharacter.Throw(() =>
            {
                if(!tutorial.IsAllStepCompleted)
                {
                    PlayNextCharacter(idx);
                }
            });
        }

        public void OnGameStart()
        {
            PlayNextCharacter();
            ui.PlayTime();
            StartCoroutine("CountSpawnTime");
        }


        public void OnGameStop()
        {
            OnEndgame(IMinigame.MatchResult.Lose);
        }
    }
}
