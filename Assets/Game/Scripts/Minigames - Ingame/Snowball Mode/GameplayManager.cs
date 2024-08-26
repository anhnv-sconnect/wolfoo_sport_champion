using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.SnowballMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] private GameplayConfig config;
        [SerializeField] private SnowmanStage stage;
        [SerializeField] private Player player;

        private IMinigame.Data myData;
        private MinigameUI ui;
        private float totalSnowballClaimed;
        private float maxScore;

        public IMinigame.Data ExternalData { get => myData; set => myData = value; }

        // Start is called before the first frame update
        void Start()
        {
            EventManager.OnFinishStage += OnSnowballCreated;
            EventManager.OnTimeOut += OnGameLosing;

            Init();
            OnGameStart();
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
                myData = new IMinigame.Data()
                {
                    coin = 0,
                    playTime = 60,
                    timelineScore = new float[] { 2, 4, 6 },
                };
            }

            ui = FindAnyObjectByType<MinigameUI>();
            ui.Setup(myData.playTime, myData.timelineScore);

            var length = myData.timelineScore.Length;
            maxScore = myData.timelineScore[length - 1];
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

        public void OnGameLosing()
        {
            ui.PauseTime();
            player.Pause(true);
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
        }

        public void OnGameWining()
        {
            ui.PauseTime();
            player.Pause();
        }
    }
}
