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

        public enum GameState
        {
            Playing,
            Pausing,
            Stopping,
        }

        public enum GameMode
        {
            Solo,
            Bot
        }

        public enum ResultState
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
            public int score;

            public Data()
            {
                coin = 0;
                score = 0;
            }
        }
    }
}
