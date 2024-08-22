using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay
{
    public interface IMinigame
    {
        public Data ExternalData { get; set; }
        public void OnGameResume();
        public void OnGameStart();
        public void OnGameStop();
        public void OnGamePause();
        public void OnGameWining();
        public void OnGameLosing();

        public enum GameState
        {
            None,
            Playing,
            Pausing,
            Stopping,
        }

        public enum GameMode
        {
            Solo,
            Bot
        }

        public enum MatchResult
        {
            Win,
            Lose,
            Draw
        }

        /// <summary>
        /// Store Data Game
        /// </summary>
        [System.Serializable]
        public class Data
        {
            public int coin;
            public int playTime;
            public int[] timelineScore;

            public Data()
            {
                coin = 0;
                playTime = 180;
            }
        }
    }
}
