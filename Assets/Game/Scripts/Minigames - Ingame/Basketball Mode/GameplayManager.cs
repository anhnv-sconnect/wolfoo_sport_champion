using DG.Tweening;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Base;
using WFSport.Helper;

namespace WFSport.Gameplay.BasketballMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] private GameplayConfig config;
        [SerializeField] private AssetRecordConfig assetConfig;
        [SerializeField] private LevelConfig levelConfig;
        [SerializeField] private Player player;
        [SerializeField] private Bot bot;
        [SerializeField] private Transform basketHolder;
        [SerializeField] private ScoreManager scoreAnimManager;
        [SerializeField] private CharacterWorldAnimation[] characterData;

        private int totalBasket;
        private Basket[] myBaskets;
        private float maxScore;
        private MultiplayerGameUI ui;
        private LevelConfig.Mode curLevel;
        private bool HasBot => myData.level > 1;

        public Basket GetRandomBasketInScreen { get => myBaskets[UnityEngine.Random.Range(0, myBaskets.Length)]; }

        private IMinigame.ConfigData myData;
        private IMinigame.ResultData result;
        public IMinigame.ConfigData InternalData { get => myData; set => myData = value; }
        IMinigame.ResultData IMinigame.ExternalData { get => result; set => result = value; }

        private void Start()
        {
            EventManager.OnGetScore += OnPlayerGetScore;
            EventManager.OnTimeOut += OnGameLosing;
            Init();
            OnGameStart();
        }
        private void OnDestroy()
        {
            EventManager.OnGetScore -= OnPlayerGetScore;
            EventManager.OnTimeOut -= OnGameLosing;
        }

        private void OnPlayerGetScore(Base.Player basePlayer)
        {
            if (basePlayer == player)
            {
                ui.UpdateLoadingBar(player.Score / maxScore);

                result.claimedCoin += myData.normalPlusCoin;
                if (ui.TotalStarClaimed > 0 && player.Score == myData.timelineScore[ui.TotalStarClaimed - 1])
                {
                    result.claimedCoin += myData.milestoneCoin;
                }

                if (player.Score > maxScore)
                {
                    ui.UpdateLoadingBar(1);
                    OnGameWining();
                }
            }
            else
            {
                ui.UpdateLoadingBar2(bot.Score / maxScore);
                if (player.Score > maxScore)
                {
                    ui.UpdateLoadingBar(1);
                    OnGameLosing();
                }
            }
        }

        private void CreateBasket()
        {
            //    totalBasket = UnityEngine.Random.Range(1, 5);
            totalBasket = curLevel.basketRecords.Length;
            myBaskets = new Basket[totalBasket];
            var range = (config.movingXRange.y - config.movingXRange.x) / 2 / totalBasket + config.spawnRange;
            for (var i = 0; i < totalBasket; i++)
            {
                var yPos = UnityEngine.Random.Range(config.movingYRange.x, config.movingYRange.y);
                var side = 0f;
                if (i % 2 == 0)
                {
                    side = -1 * i / 2;
                    if (totalBasket % 2 == 0) side = -1 * (i + 2) / 2;
                }
                else
                {
                    side = 1 * (i + 1) / 2;
                    if (totalBasket % 2 == 0) side = 1 * (i + 2) / 2;
                }
                var xPos = range * side;
                var basketPb = curLevel.basketRecords[i];
                var basket = Instantiate(basketPb, new Vector3(xPos, yPos, 0), basketPb.transform.rotation, basketHolder);
                myBaskets[i] = basket;
                basket.Setup(config, assetConfig);
            }
        }

        private void Init()
        {
            result = new IMinigame.ResultData();
            myData = DataTransporter.GameplayConfig;
            if (myData == null)
            {
                myData = new IMinigame.ConfigData()
                {
                    playTime = 90,
                    timelineScore = new float[] { 10, 20, 40 },
                    level = 0,
                };
            }
            else
            {
                myData.level = DataTransporter.Level;
            }
            if (myData.level == Constant.LEVEL.LEVEL_1) curLevel = levelConfig.mode1;
            else if (myData.level == Constant.LEVEL.LEVEL_2) curLevel = levelConfig.mode2;
            else if (myData.level == Constant.LEVEL.LEVEL_3) curLevel = levelConfig.mode3;
            else curLevel = levelConfig.modeTest;

            myData.timelineScore = curLevel.timelineScores;
            maxScore = myData.timelineScore[curLevel.timelineScores.Length - 1];
            ui = FindAnyObjectByType<MultiplayerGameUI>(FindObjectsInactive.Include);
            ui.gameObject.SetActive(true);
            ui.Setup(myData.playTime, curLevel.timelineScores);
            if (!HasBot) ui.SetupSinglePlayer();

            foreach (var character in characterData)
            {
                if (character.Name == CharacterWorldAnimation.CharacterName.Wolfoo)
                {
                    player.Setup(config, curLevel, this);
                    player.Create(character);
                    player.Init();
                }

                if (HasBot && character.Name == curLevel.botName)
                {
                    bot.Setup(config, curLevel, this);
                    bot.Create(character);
                    bot.Init();
                }
            }

            CreateBasket();
            scoreAnimManager.CreateAnim(totalBasket * 2 + 2); // 2: is Total Player
        }

        public void OnGameLosing()
        {
            player.Pause(true);
            ui.PauseTime();
            foreach (var basket in myBaskets)
            {
                basket.Pause();
            }
            OnEndgame(IMinigame.MatchResult.Lose);
        }

        public void OnGamePause()
        {
            player.Pause(true);
            ui.PauseTime();
            foreach (var basket in myBaskets)
            {
                basket.Pause();
            }
        }

        public void OnGameResume()
        {
            ui.PlayTime();
            foreach (var basket in myBaskets) { basket.Play(); }
            player.Play();
            if (HasBot) bot.Play();
        }

        public void OnGameStart()
        {
            ui.OpenCountingToStart(() =>
            {
                ui.PlayTime();
                foreach (var basket in myBaskets) { basket.Play(); }
                player.Play();
                if (HasBot) bot.Play();
            });
        }

        public void OnGameStop()
        {
            player.Pause(true);
            ui.PauseTime();
            foreach (var basket in myBaskets)
            {
                basket.Pause();
            }
            OnEndgame(IMinigame.MatchResult.Lose);
        }

        public void OnGameWining()
        {
            player.Pause(true);
            ui.PauseTime();
            foreach (var basket in myBaskets)
            {
                basket.Pause();
            }
            OnEndgame(IMinigame.MatchResult.Win);
        }

        private void OnEndgame(IMinigame.MatchResult matchResult)
        {
            result.gamestate = matchResult;
            EventDispatcher.Instance.Dispatch(new EventKey.OnGameStop { data = result });
        }
    }
}
