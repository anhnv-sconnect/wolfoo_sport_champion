using AnhNV.GameBase;
using DG.Tweening;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using WFSport.Helper;

namespace WFSport.Gameplay.ArcheryMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] private GameplayConfig config;
        [SerializeField] private Bot bot;
        [SerializeField] private Player player;
        [SerializeField] private Transform specialArrowIcon;
        [SerializeField] private Transform sortingBgHolder;
        [SerializeField] private Transform markerHolder;
        [SerializeField] private Transform bombHolder;
        [SerializeField] private IdleMarker[] idleMarkers;
        [SerializeField] private MovingMarker[] movingMarkerPbs;
        [SerializeField] private BombController bombController;
        [SerializeField] private IdleMarker tutorialMarker;

        private IdleMarker[] curRandomMarkers;
        private MovingMarker[] poolingMovingMarkers;
        private int markedCount;
        private float countTime;
        private int movingMarkedCount;
        private float maxScore;
        private int totalSpecialMarker;
        private MultiplayerGameUI ui;
        private Tutorial tutorial;

        private Sequence _tweenSpecialArrow;

        public MovingMarker[] MovingMarkers { get => poolingMovingMarkers; }
        public IdleMarker[] IdleMarkers { get => curRandomMarkers; }
        public Vector2 ScreenWidthRange { get; private set; }
        public Vector2 ScreenHeightRange { get; private set; }

        private IMinigame.ConfigData myData;
        private IMinigame.ResultData result;
        public IMinigame.ConfigData InternalData { get => myData; set => myData = value; }
        IMinigame.ResultData IMinigame.ExternalData { get => result; set => result = value; }

        [NaughtyAttributes.Button]
        private void SortingLayer()
        {
            foreach (var item in sortingBgHolder.GetComponentsInChildren<SpriteRenderer>())
            {
                item.sortingOrder = Base.Player.SortingLayer(item.transform.position);
            }
            var records = markerHolder.GetComponentsInChildren<IdleMarker>();
            idleMarkers = new IdleMarker[records.Length];
            for (int i = 0; i < records.Length; i++)
            {
                records[i].GetComponent<SortingGroup>().sortingOrder = Base.Player.SortingLayer(records[i].transform.position);
                idleMarkers[i] = records[i];
            }
            foreach (var item in bombHolder.GetComponentsInChildren<SpriteRenderer>())
            {
                item.sortingOrder = Base.Player.SortingLayer(item.transform.position);
            }
        }
        private void Start()
        {
            EventManager.OnShooting += OnArrowShooting;
            EventManager.OnTimeOut += OnGameLosing;

            Init();
            InitTutorial();
            PlayTutorial();
        }

        private void OnDestroy()
        {
            EventManager.OnShooting -= OnArrowShooting;
            EventManager.OnTimeOut -= OnGameLosing;
            StopCoroutine("SetupNextMarker");
            StopCoroutine("CountingTime");
            _tweenSpecialArrow?.Kill();
        }
        private void SetupMainGameplay()
        {
            tutorialMarker.gameObject.SetActive(false);
            bot.gameObject.SetActive(true);
        }
        private void PlayTutorial()
        {
            tutorial.PlayNextStep();
            player.Play();
        }

        private void InitTutorial()
        {
            tutorialMarker.SetupTutorial();
            tutorialMarker.gameObject.SetActive(true);
            bot.gameObject.SetActive(false);

            tutorial = TutorialController.Instance.CreateTutorial("Archery Tutorial");
            var step = TutorialController.Instance.CreateStep<TutorialClick>(tutorial);
            step.Setup(tutorialMarker.TargetPosition);
        }

        private void PlayAnimPlayerGetSpecialArrow(Vector3 startPos, Vector3 endPos, System.Action OnComplete = null)
        {
            specialArrowIcon.transform.position = startPos;
            specialArrowIcon.transform.localScale = Vector3.one * 2;

            _tweenSpecialArrow?.Kill();
            _tweenSpecialArrow = DOTween.Sequence()
                .AppendInterval(0.2f)
                .AppendCallback(() => specialArrowIcon.gameObject.SetActive(true))
                .Append(specialArrowIcon.DOMoveY(startPos.y + 0.5f, 0.5f))
                .Append(specialArrowIcon.DOJump(endPos, 4, 1, 0.5f))
                .Join(specialArrowIcon.DOScale(Vector3.one * 1, 0.5f));
            _tweenSpecialArrow.OnComplete(() =>
            {
                specialArrowIcon.gameObject.SetActive(false);
                OnComplete?.Invoke();
            });
        }

        private IEnumerator CountingTime()
        {
            yield return new WaitForSeconds(1);

            countTime++;

            for (int i = 0; i < config.movingMarkerSpawnTimes.Length; i++)
            {
                var item = config.movingMarkerSpawnTimes[i];
                if (item.spawnTime == countTime)
                {
                    SpawnMovingMarker(item.totalSpawningMarker);
                    break;
                }
            }

            for (int i = 0; i < config.specialItemSpawnTimelines.Length; i++)
            {
                var item = config.specialItemSpawnTimelines[i];
                if (item == countTime)
                {
                    SpawnSpecialMarker();
                    break;
                }
            }

            for(int i = 0; i< config.bombSpawnTimelines.Length; i++)
            {
                var item = config.bombSpawnTimelines[i];
                if (item == countTime)
                {
                    bombController.CreateBomb();
                }
            }

            if(countTime < myData.playTime) { StartCoroutine("CountingTime"); }
        }

        private void SpawnSpecialMarker()
        {
            totalSpecialMarker++;
        }

        private void SpawnMovingMarker(int totalMarker)
        {
            Vector3 lastMarkerPos = Vector3.zero;
            MovingMarker movingMarker;
            for (int i = 0; i < totalMarker; i++)
            {
                var rdPosIdx = UnityEngine.Random.Range(0, config.movingMarkerYPos.Length);
                var rdMarkerIdx = UnityEngine.Random.Range(0, movingMarkerPbs.Length);
                if (poolingMovingMarkers[movingMarkedCount] == null)
                {
                    movingMarker = Instantiate(movingMarkerPbs[rdMarkerIdx], markerHolder);
                    poolingMovingMarkers[movingMarkedCount] = movingMarker;
                }
                else
                {
                    movingMarker = poolingMovingMarkers[movingMarkedCount];
                }
                movingMarker.SetupScore(config.movingScore);
                movingMarker.Setup(config.delayHideTime,config.movingMarkerYPos[rdPosIdx], config.movingSpeed);
                if (i > 0) { movingMarker.SetupNext(lastMarkerPos, config.movingMarkerSpacing); }
                lastMarkerPos = movingMarker.transform.position;
                movingMarker.Show();

                movingMarkedCount++;
                movingMarkedCount = movingMarkedCount >= poolingMovingMarkers.Length ? 0 : movingMarkedCount;
            }
        }

        private void OnArrowShooting(Arrow obj)
        {
            var holderPlayer = obj.AssignPlayer;
            var isBot = obj.AssignPlayer as Bot;

            if(!tutorial.IsAllStepCompleted)
            {
                tutorial.Stop();
                if (tutorialMarker.IsInside(obj.transform.position))
                {
                    player.Pause(false);
                    tutorialMarker.OnHitCorrect(obj.transform.position);
                    tutorial.SetCompletedCurrentStep();

                    if(tutorial.IsAllStepCompleted)
                    {
                        ui.OpenLoading(() =>
                        {
                            ui.OpenCountingToStart(() =>
                            {
                                OnGameStart();
                            });
                        },
                        () =>
                        {
                            SetupMainGameplay();
                        }, 1.5f);
                    }
                }
                return;
            }

            foreach (var marker in idleMarkers)
            {
                if (!obj.IsAttached && marker.IsInside(obj.transform.position))
                {
                    obj.IsAttached = true;
                    if (holderPlayer.IsSpecialMode || marker.IsSpecial)
                    {
                        marker.SetupScore(config.specialScore);
                    }
                    else
                    {
                        marker.SetupScore(config.normalScore);
                    }
                    marker.OnHitCorrect(obj.transform.position);

                    markedCount++;
                    if(curRandomMarkers != null && markedCount >= curRandomMarkers.Length)
                    {
                        holderPlayer.UpgradeScore(config.normalScore);
                        OnTrackingScore(holderPlayer, isBot, myData.normalPlusCoin);
                        SpawnNextMarker();
                    }

                    if(marker.IsSpecial)
                    {
                        holderPlayer.UpgradeScore(config.specialScore);
                        OnTrackingScore(holderPlayer, isBot, myData.specialPlusCoin);
                        PlayAnimPlayerGetSpecialArrow(obj.transform.position, holderPlayer.BowPos, () =>
                        {
                            holderPlayer.PlayWithSpecialItem();
                        });
                    }

                    return;
                }
            }

            if(poolingMovingMarkers != null)
            {
                for (int i = 0; i < poolingMovingMarkers.Length; i++)
                {
                    if (!obj.IsAttached && poolingMovingMarkers[i] != null && poolingMovingMarkers[i].IsInside(obj.transform.position))
                    {
                        obj.IsAttached = true;
                        poolingMovingMarkers[i].OnHitCorrect(obj.transform.position);
                        holderPlayer.UpgradeScore(config.movingScore);
                        OnTrackingScore(holderPlayer, isBot, myData.special2PlusCoin);
                        return;
                    }
                }
            }

            var isShootedBomb = bombController.IsArrowShooted(obj);
            if (isShootedBomb) {
                // Decrease Score
                StopCoroutine("OnShootedBomb");
                StartCoroutine("OnShootedBomb");
            }
        }
        private void OnTrackingScore(Player player, bool isBot, int plusScore)
        {
            if (isBot)
            {
                ui.UpdateLoadingBar2(player.Score / maxScore);
                if (bot.Score == maxScore)
                {
                    OnGameLosing();
                }
            }
            else
            {
                ui.UpdateLoadingBar(player.Score / maxScore);

                result.claimedCoin += plusScore;
                if (ui.TotalStarClaimed > 0 && player.Score == myData.timelineScore[ui.TotalStarClaimed - 1])
                {
                    result.claimedCoin += myData.milestoneCoin;
                }

                if (player.Score >= maxScore)
                {
                    OnGameWining();
                }
            }
        }
        private IEnumerator OnShootedBomb()
        {
            player.Pause(false);

            yield return new WaitForSeconds(config.bombUsedTime);

            player.Play();
        }
        private void InitMovingMarker()
        {
            var maxWidth = 0f;
            var deltaTime = 0f;
            var maxSpawningObject = 0;
            var data = config.movingMarkerSpawnTimes;
            var rangeTime = data.Length > 0 ? data[data.Length - 1].spawnTime - data[0].spawnTime : 0f;
            var totalObject = 0;

            foreach (var item in movingMarkerPbs)
            {
                maxWidth = Mathf.Max(item.Width, maxWidth);
                deltaTime = (config.movingMarkerSpacing + maxWidth) / config.movingSpeed;
            }
            var distance = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth + maxWidth, 0, Camera.main.transform.position.z));

            for (int i = 0; i < config.movingMarkerSpawnTimes.Length; i++)
            {
                var item = config.movingMarkerSpawnTimes[i];
                maxSpawningObject = Mathf.Max(item.totalSpawningMarker, maxSpawningObject);
                totalObject += config.movingMarkerSpawnTimes[i].totalSpawningMarker;
            }

            var totalMovingTime = (maxSpawningObject) * (deltaTime) + (distance.x / config.movingSpeed);
            var totalReloadObject = rangeTime / totalMovingTime;
            totalObject -= (int) totalReloadObject;

            poolingMovingMarkers = new MovingMarker[totalObject];
        }

        private void Init()
        {
            myData = DataTransporter.GameplayConfig;
            result = new IMinigame.ResultData();

            if (myData == null)
            {
                InternalData = new IMinigame.ConfigData()
                {
                    playTime = 90,
                    timelineScore = new float[] { 10, 20, 40 },
                };
            }
            maxScore = myData.timelineScore[myData.timelineScore.Length - 1];
            ui = FindAnyObjectByType<MultiplayerGameUI>(FindObjectsInactive.Include);
            ui.gameObject.SetActive(true);
            ui.Setup(myData.playTime, myData.timelineScore);

            var maxRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, Camera.main.transform.position.z));
            var maxUp = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, Camera.main.transform.position.z));
            ScreenWidthRange = new Vector2(-maxRight.x, maxRight.x);
            ScreenHeightRange = new Vector2(-maxUp.y, maxUp.y);

            InitMovingMarker();
            bombController.Setup(config);

            specialArrowIcon.gameObject.SetActive(false);
            player.Init();
            bot.Init();
        }
        private void SpawnNextMarker()
        {
            StopCoroutine("SetupNextMarker");
            StartCoroutine("SetupNextMarker");
        }
        private IEnumerator SetupNextMarker()
        {
            var maxRange = config.randomRange;
            if (curRandomMarkers != null)
            {
                foreach (var item in curRandomMarkers)
                {
                    item.Hide();
                }
                maxRange = Mathf.Min(idleMarkers.Length - curRandomMarkers.Length, config.randomRange);
            }

            markedCount = 0;
            var totalRandom = UnityEngine.Random.Range(1, maxRange);
            curRandomMarkers = new IdleMarker[totalRandom];

            for (int i = 0; i < totalRandom; i++)
            {
                int idx = UnityEngine.Random.Range(0, idleMarkers.Length);
                while (idleMarkers[idx].IsShowing)
                {
                    idx = UnityEngine.Random.Range(0, idleMarkers.Length);
                }
                idleMarkers[idx].Setup(config.delayHideTime);
                if(totalSpecialMarker > 0)
                {
                    idleMarkers[idx].SetupSpecial();
                    totalSpecialMarker--;
                }
                idleMarkers[idx].Show();
                curRandomMarkers[i] = idleMarkers[idx];
            }

            yield return new WaitForSeconds(config.randomSpawnTime);

            StartCoroutine("SetupNextMarker");
        }

        public void OnGameLosing()
        {
            player.Pause(false);
            bot.Pause(false);
            ui.PauseTime();
            OnEndgame(IMinigame.MatchResult.Lose);
        }

        public void OnGamePause()
        {
            player.Pause(true);
            bot.Pause(false);
            ui.PauseTime();
        }

        public void OnGameResume()
        {
            player.Play();
            bot.Play();
            ui.PlayTime();
        }

        public void OnGameStart()
        {
            SpawnNextMarker();
            player.Play();
            bot.Play();
            StartCoroutine("CountingTime");
            ui.PlayTime();
        }

        public void OnGameStop()
        {
            player.Pause(false);
            bot.Pause(false);
            ui.PauseTime();
            OnEndgame(IMinigame.MatchResult.Lose);
        }

        public void OnGameWining()
        {
            player.Pause(false);
            bot.Pause(false);
            ui.PauseTime();

            OnEndgame(IMinigame.MatchResult.Win);
        }
        private void OnEndgame(IMinigame.MatchResult matchResult)
        {
            result.gamestate = matchResult;
            EventDispatcher.Instance.Dispatch(new EventKey.OnGameStop { data = result });
        }
    }
}
