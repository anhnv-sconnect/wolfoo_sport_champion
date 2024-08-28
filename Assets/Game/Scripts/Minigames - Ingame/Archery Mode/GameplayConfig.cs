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
    }
}
