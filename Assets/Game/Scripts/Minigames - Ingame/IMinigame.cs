using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport
{
    public interface IMinigame
    {
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

        public enum ResultState
        {
            Win,
            Lose,
            Draw
        }

        /// <summary>
        /// Store Data Game
        /// </summary>
        internal class Data
        {
            public int coin;
            public int score;
            public ResultState completeGState;
        }
    }
}
