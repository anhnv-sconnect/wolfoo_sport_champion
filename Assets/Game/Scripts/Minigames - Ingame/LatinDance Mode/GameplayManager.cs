using AnhNV.GameBase;
using DG.Tweening;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Helper;

namespace WFSport.Gameplay.LatinDanceMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] SpotlightGroup spotlightManager;
        [SerializeField] BonusItem itemPb;
        [SerializeField] BoxCollider2D limited;
        [SerializeField] Transform itemHolder;
        [SerializeField] float radius;
        [SerializeField] float padding;
        [SerializeField] int totalItem;
        [SerializeField] Sprite[] data;
        [SerializeField] Player player;

        private float finalScore;
        private MinigameUI ui;
        private float score;

        private IMinigame.ConfigData myData;
        private IMinigame.ResultData result;
        private IMinigame.MatchResult matchResult;
        private float beginTime;
        private Tutorial tutorial;
        private bool isDragged;

        public IMinigame.ConfigData InternalData { get => myData; set => myData = value; }
        IMinigame.ResultData IMinigame.ExternalData { get => result; set => result = value; }

        [NaughtyAttributes.Button]
        private void CreateRandomItems()
        {
            var dimension = (radius * 2 + padding / 2);

            var size = new Vector2((limited.size.x / dimension), (limited.size.y / dimension));
            var rowIndexs = new List<int>(totalItem);
            for (int i = 0; i < totalItem; i++)
            {
                var rd = UnityEngine.Random.Range(1, (int)size.x + 1);
                rowIndexs.Add(rd);
            }
            var colIndexs = new List<int>(totalItem);
            for (int i = 0; i < totalItem; i++)
            {
                var rd = UnityEngine.Random.Range(1, (int)size.y + 1);
                colIndexs.Add(rd);
            }

            var startPos = new Vector2(-limited.size.x / 2, -limited.size.y / 2);
            for (int i = 0; i < totalItem; i++)
            {
                float x = startPos.x + (rowIndexs[i]) * dimension;
                float y = startPos.y + (colIndexs[i]) * dimension;
                var pos = new Vector2(x, y);

                var obj = Instantiate(itemPb, itemHolder);
                obj.Setup(data[UnityEngine.Random.Range(0, data.Length)], pos);
                obj.Spawn();
            }
        }
        [NaughtyAttributes.Button]
        private void Clear()
        {
            foreach (var item in itemHolder.GetComponentsInChildren<BonusItem>())
            {
                DestroyImmediate(item.gameObject);
            }
        }
        [NaughtyAttributes.Button]
        private void CreateMatrix()
        {
            var dimension = (radius * 2 + padding / 2);
            var size = new Vector2((limited.size.x / dimension), (limited.size.y / dimension));
            var startPos = new Vector2(-limited.size.x / 2, -limited.size.y / 2);

            for (int c = 1; c <= size.y; c++)
            {
                for (int r = 1; r <= size.x; r++)
                {
                    float x = startPos.x + (r) * dimension;
                    float y = startPos.y + (c) * dimension;
                    var pos = new Vector2(x, y);
                    var obj = Instantiate(itemPb, itemHolder).gameObject;
                    obj.transform.position = pos;
                }
            }
        }

        private void Start()
        {
            EventManager.OnHighlight += OnHighlight;
            EventManager.OnStopHighlight += OnStopHighlight;
            EventManager.OnPlayerClaimNewStar += OnPlayerClaimNewStar;
            EventManager.OnHide += OnItemHiding;
            EventManager.OnTimeOut += OnGameLosing;
            player.OnDragEvent = OnDragging;

            Init();
            InitTutorial();
            PlayTutorial();
       //     OnGameStart();
        }

        private void OnDestroy()
        {
            EventManager.OnPlayerClaimNewStar -= OnPlayerClaimNewStar;
            EventManager.OnHide -= OnItemHiding;
            EventManager.OnHighlight -= OnHighlight;
            EventManager.OnStopHighlight -= OnStopHighlight;
            EventManager.OnTimeOut -= OnGameLosing;
        }

        private void OnDragging()
        {
            if(tutorial != null && !tutorial.IsAllStepCompleted)
            {
                tutorial.Stop();
                isDragged = true;
            }
        }

        private void InitTutorial()
        {
            tutorial = TutorialController.Instance.CreateTutorial("Latin");
            var step = TutorialController.Instance.CreateStep<TutorialDragInRange>(tutorial);
        }

        private void PlayTutorial()
        {
            tutorial.PlayNextStep();
            player.Play();
        }

        private void OnHighlight(Transform arg1, bool arg2)
        {
            if (arg2)
            {
                spotlightManager.PlayFocus(arg1);
            }
            else
            {
                spotlightManager.Play(arg1);
            }
        }

        private void OnStopHighlight(Transform obj)
        {
            spotlightManager.Stop(obj);
        }

        private void OnItemHiding(BonusItem obj)
        {
            CreateNextStar(obj);
        }

        private void CreateNextStar(BonusItem obj)
        {
            var dimension = (radius * 2 + padding / 2);
            var startPos = new Vector2(-limited.size.x / 2, -limited.size.y / 2);
            var size = new Vector2((limited.size.x / dimension), (limited.size.y / dimension));

            var rdRow = UnityEngine.Random.Range(1, (int)size.x + 1);
            var rdCol = UnityEngine.Random.Range(1, (int)size.y + 1);

            float x = startPos.x + (rdRow) * dimension;
            float y = startPos.y + (rdCol) * dimension;
            var pos = new Vector2(x, y);

            obj.Setup(data[UnityEngine.Random.Range(0, data.Length)], pos);
            obj.Spawn();
        }

        private void Init()
        {
            result = new IMinigame.ResultData();
            myData = DataTransporter.GameplayConfig;
            if (myData == null)
            {
                myData = new IMinigame.ConfigData()
                {
                    playTime = 180,
                    timelineScore = new float[] { 20, 50, 80 }
                };
            }
            finalScore = myData.timelineScore[2];
            ui = FindAnyObjectByType<SoloMinigameUI>(FindObjectsInactive.Include);
            ui.gameObject.SetActive(true);
            ui.Setup(myData.playTime, myData.timelineScore);

            player.Init();
            player.CreateWolfoo();
            CreateRandomItems();
        }

        private void OnPlayerClaimNewStar(Base.Player obj, bool isSpecial)
        {
            if (tutorial != null && !tutorial.IsAllStepCompleted && isDragged)
            {
                tutorial.SetCompletedCurrentStep();
                player.Pause(true);
                ui.OpenLoading(() =>
                {
                    ui.OpenCountingToStart(() =>
                    {
                        OnGameStart();
                    });
                },
                () =>
                {
                    player.gameObject.SetActive(false);
                },
                1.5f);
                return;
            }

            score++;
            ui.UpdateLoadingBar(score / finalScore);

            if (ui.TotalStarClaimed > 0 && score == myData.timelineScore[ui.TotalStarClaimed - 1])
            {
                result.claimedCoin += myData.milestoneCoin;
            }

            if (score == myData.timelineScore[1])
            {
                player.IntroduceWFPartner();
            }
            else if(score >= finalScore)
            {
                OnGameWining();
            }
        }

        private void OnEndgame(IMinigame.MatchResult matchResult)
        {
            result.gamestate = matchResult;
            EventDispatcher.Instance.Dispatch(new EventKey.OnGameStop { data = result });
        }

        public void OnGameLosing()
        {
            ui.PauseTime();
            player.Pause(false);
            OnEndgame(IMinigame.MatchResult.Lose);
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
            beginTime = Time.time;
            player.Setup(11);
            player.gameObject.SetActive(true);
            player.IntroduceWolfoo(() =>
            {
                ui.PlayTime();
                player.Setup(1);
                player.Play();
            });
        }

        public void OnGameStop()
        {
            ui.PauseTime();
            player.Pause(true);
            OnEndgame(IMinigame.MatchResult.Lose);
        }

        public void OnGameWining()
        {
            ui.PauseTime();
            player.PlayWining();
            OnEndgame(IMinigame.MatchResult.Win);
        }
    }
}
