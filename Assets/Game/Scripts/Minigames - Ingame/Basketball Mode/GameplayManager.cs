using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] private GameplayConfig config;
        [SerializeField] private Player player;
        [SerializeField] private Basket basketPb;
        [SerializeField] private Transform basketHolder;
        [SerializeField] private int totalBasket;
        [SerializeField] private ScoreManager scoreAnimManager;
        [SerializeField] private Bomb bombPb;

        private IMinigame.Data myData;
        private Basket[] myBaskets;
        private float maxScore;
        private MultiplayerGameUI ui;

        public IMinigame.Data ExternalData { get => myData; set => myData = value; }
        private void Start()
        {
            EventManager.OnGetScore += OnPlayerGetScore;
            EventManager.OnShootingBomb += OnShootingBomb;
            Init();
            OnGameStart();
        }
        private void OnDestroy()
        {
            EventManager.OnGetScore -= OnPlayerGetScore;
            EventManager.OnShootingBomb -= OnShootingBomb;
        }

        private void OnShootingBomb(Base.Player basePlayer)
        {
            ui.UpdateLoadingBar(player.Score / maxScore);
        }

        [NaughtyAttributes.Button]
        private void PlayBasketMoving()
        {
            foreach (var item in myBaskets)
            {
                item.PlayMoveAround();
            }
        }
        [NaughtyAttributes.Button]
        private void StopBasketMoving()
        {
            foreach (var item in myBaskets)
            {
                item.StopMoveAround();
            }
        }
        private void OnPlayerGetScore(Base.Player basePlayer, Vector3 endPos)
        {
            scoreAnimManager.Play(endPos);
            ui.UpdateLoadingBar(player.Score / maxScore);
        }

        private void CreateBasket()
        {
        //    totalBasket = UnityEngine.Random.Range(1, 5);
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
                var basket = Instantiate(basketPb, new Vector3(xPos, yPos, 0), basketPb.transform.rotation, basketHolder);
                myBaskets[i] = basket;
                basket.Setup(config, bombPb);
            }
        }

        private void Init()
        {
            if (myData == null)
            {
                ExternalData = new IMinigame.Data()
                {
                    coin = 0,
                    playTime = 90,
                    timelineScore = new float[] { 10, 20, 40 },
                };
            }
            maxScore = myData.timelineScore[myData.timelineScore.Length - 1];
            ui = FindAnyObjectByType<MultiplayerGameUI>();
            ui.Setup(myData.playTime, myData.timelineScore);

            player.Setup(config);
            CreateBasket();
            scoreAnimManager.CreateAnim(totalBasket);
        }

        public void OnGameLosing()
        {
        }

        public void OnGamePause()
        {
        }

        public void OnGameResume()
        {
        }

        public void OnGameStart()
        {
            player.Play();
            ui.PlayTime();
        }

        public void OnGameStop()
        {
        }

        public void OnGameWining()
        {
        }
    }
}
