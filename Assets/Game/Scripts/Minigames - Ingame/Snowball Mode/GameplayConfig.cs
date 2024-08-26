using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.SnowballMode
{
    [CreateAssetMenu(fileName = "Snowball Config", menuName = "Gameplay Data/Snowball", order = 1)]
    public class GameplayConfig : ScriptableObject
    {
        [Header("================ BALL SETTING ================")]
        public float rotateVelocity;
        public float maxSize;
        public float growthSpeed;

        [Header("================ PLAYER SETTING ================")]
        public float velocity = 0.01f;
        /// <summary>
        /// x: Left direct, y: Up direct, z: Right direct, w: Down direct
        /// </summary>
        public Vector4 limitPosition;
    }
}
