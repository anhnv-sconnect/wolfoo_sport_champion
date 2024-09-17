using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    [CreateAssetMenu(fileName = "Basketball_Level Config", menuName = "Gameplay Data/Basketball_Level", order = 1)]
    public class LevelConfig : ScriptableObject
    {
        public Mode modeTest;
        public Mode mode1;
        public Mode mode2;
        public Mode mode3;

        [System.Serializable]
        public struct Mode
        {
            [Header("===== BASKET =====")]
            public Basket[] basketRecords;

            [Header("===== BOT =====")]
            public CharacterWorldAnimation.CharacterName botName;
            public float botReloadTime;
            [Range(0, 100)] public int botShootingTargetPercent;

            [Header("===== SCORE ====")]
            public float[] timelineScores;
        }
    }
}
