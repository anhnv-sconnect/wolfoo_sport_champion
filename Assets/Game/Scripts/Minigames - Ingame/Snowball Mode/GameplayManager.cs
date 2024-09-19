using AnhNV.GameBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Helper;
using static WFSport.Gameplay.IMinigame;

namespace WFSport.Gameplay.SnowballMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] private CharacterWorldAnimation[] audiences;
        [SerializeField] private CharacterWorldAnimation wolfooPb;
        [SerializeField] private GameplayConfig config;
        [SerializeField] private SnowmanStage stage;
        [SerializeField] private Player player;
        [SerializeField] private Player tutorialPlayer;
        [SerializeField] private Transform[] seats;
        [SerializeField] private Transform otherItemInMapHolder;

        private MinigameUI ui;
        private float totalSnowballClaimed;
        private float maxScore;
        private Tutorial tutorial;

        private IMinigame.ConfigData myData;
        private IMinigame.ResultData result;
        public IMinigame.ConfigData InternalData { get => myData; set => myData = value; }
        IMinigame.ResultData IMinigame.ExternalData { get => result; set => result = value; }

        // Start is called before the first frame update
        void Start()
        {
            EventManager.OnFinishStage += OnSnowballCreated;
            EventManager.OnTimeOut += OnGameLosing;

            Init();
            //    CreateTutorial();
            ui.OpenCountingToStart(() =>
            {
                OnGameStart();
            });
        }

        private void CreateTutorial()
        {
            tutorial = TutorialController.Instance.CreateTutorial("Snowball");
            var step = TutorialController.Instance.CreateStep<TutorialSwipe>(tutorial);
            step.Setup(tutorialPlayer.transform, AnimatorHelper.Direction.Left);
        }

        private void OnDestroy()
        {
            EventManager.OnFinishStage -= OnSnowballCreated;
            EventManager.OnTimeOut -= OnGameLosing;
        }


        private void Init()
        {
            if(myData == null)
            {
                myData = new IMinigame.ConfigData()
                {
                    playTime = 60,
                    timelineScore = new float[] { 2, 4, 6 },
                };
            }

            ui = FindAnyObjectByType<MinigameUI>();
            ui.Setup(myData.playTime, myData.timelineScore);

            var length = myData.timelineScore.Length;
            maxScore = myData.timelineScore[length - 1];

            foreach (var item in seats)
            {
                var rdIdx = UnityEngine.Random.Range(0, audiences.Length);
                var audience = Instantiate(audiences[rdIdx], item);
                audience.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 1);
                audience.ChangeSkin(CharacterWorldAnimation.SkinType.Christmas);
                audience.PlayIdleAnim();
            }
            var wolfoo = Instantiate(wolfooPb, player.transform);
            player.Setup(wolfoo);

            foreach (var item in otherItemInMapHolder.GetComponentsInChildren<SpriteRenderer>())
            {
                item.sortingOrder = CalculateTopDownPosition(item.transform);
            }
        }

        private int CalculateTopDownPosition(Transform target)
        {
            return (int)((target.position.y) * -100);
        }

        private void OnSnowballCreated()
        {
            // Update UI score
            totalSnowballClaimed++;
            ui.UpdateLoadingBar(totalSnowballClaimed / maxScore);

            if(totalSnowballClaimed == maxScore)
            {
                OnGameWining();
            }
            else
            {
                player.Play();
            }
        }

        private void OnEndgame(MatchResult matchResult)
        {
            result = new IMinigame.ResultData()
            {
                claimedCoin = 0,
                gamestate = matchResult,
            };
            DataTransporter.GameplayResultData = result;
        }

        public void OnGameLosing()
        {
            ui.PauseTime();
            player.Pause(true);
            OnEndgame(MatchResult.Lose);
        }

        public void OnGamePause()
        {
            ui.PauseTime();
            player.Pause(true);
        }

        public void OnGameResume()
        {
            ui.PlayTime();
            player.Play();
        }

        public void OnGameStart()
        {
            ui.PlayTime();
            player.Play();
        }

        public void OnGameStop()
        {
            ui.PauseTime();
            player.Pause(true);
            OnEndgame(MatchResult.Lose);
        }

        public void OnGameWining()
        {
            ui.PauseTime();
            player.Pause();
            OnEndgame(MatchResult.Win);
        }
    }
}
