using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] GameplayConfig config;
        [SerializeField] Player player;
        [SerializeField] Basket[] baskets;

        private IMinigame.Data myData;

        public IMinigame.Data ExternalData { get => myData; set => myData = value; }

        [NaughtyAttributes.Button]
        private void PlayBasketMoving()
        {
            foreach (var item in baskets)
            {
                item.PlayMoveAround();
            }
        }
        [NaughtyAttributes.Button]
        private void StopBasketMoving()
        {
            foreach (var item in baskets)
            {
                item.StopMoveAround();
            }
        }
        private void Start()
        {
            Init();
            OnGameStart();
        }
        private void OnDestroy()
        {
        }

        private void Init()
        {
            player.Setup(config);
            foreach (var basket in baskets)
            {
                basket.Setup(config);
            }
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
        }

        public void OnGameStop()
        {
        }

        public void OnGameWining()
        {
        }
    }
}
