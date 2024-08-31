using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.ArcheryMode
{
    [CreateAssetMenu(fileName = "Archery Config", menuName = "Gameplay Data/Archery", order = 1)]
    public class GameplayConfig : ScriptableObject
    {
        [Header("==================== PLAYER ====================")]
        public float reloadTime;
        public float flySpeed;
        public float bowDrawingTime;

        [Header("==================== MARKER ====================")]
        public float randomSpawnTime;
        public float delayHideTime;
        public int randomRange;
        public int movingSpeed;
        public int movingMarkerSpacing;
        public float[] movingMarkerYPos;
        public MovingMarkerTime[] movingMarkerSpawnTimes;
        public float[] specialItemSpawnTimelines;
        public float specialAliveTime;
        public float[] bombSpawnTimelines = new float[] { 15, 30, 45, 60, 75 };
        public float bombUsedTime = 3;

        [System.Serializable]
        public struct MovingMarkerTime
        {
            public float spawnTime;
            public int totalSpawningMarker;
        }
    }
}
