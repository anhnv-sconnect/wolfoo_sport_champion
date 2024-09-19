using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay
{
    public interface IMinigame
    {
        public ResultData ExternalData { get; protected set; }
        public ConfigData InternalData { get; set; }
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
        public class ConfigData
        {
            public int playTime;
            public float[] timelineScore;
            public int level;
            public int milestoneCoin;
            public int normalPlusCoin;
            public int specialPlusCoin;
        }
        [System.Serializable]
        public class ResultData
        {
            public int claimedCoin;
            public float playTime;
            public MatchResult gamestate;

            public ResultData()
            {
                claimedCoin = 0;
                playTime = 0;
                gamestate = MatchResult.Lose;
            }
        }
    }
}
