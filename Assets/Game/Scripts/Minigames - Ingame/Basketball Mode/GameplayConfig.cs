using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    [CreateAssetMenu(fileName = "Basketball Config", menuName = "Gameplay Data/Basketball", order = 1)]
    public class GameplayConfig : ScriptableObject
    {
        [Header("=============== BALL ===============")]
        public float flyTime = 0.5f;
        public float rotateSpeed = 1f;
        public float flyingPower = 2;

        [Header("=============== PLAYER ===============")]
        public float reloadTime = 0.5f;
        public float scaleRange = 0.5f;

        [Header("=============== BASKET ===============")]
        public float height;
        public float insideDistance = 1;
        public Vector2 movingYRange;
        public Vector2 movingXRange;
        public Vector2 movingSpeed;
        public float spawnRange;
        public bool canMoveX;
        public bool canMoveY;

        [Header("=============== BOMB ===============")]
        public float aliveTime;
        public int effectBombScore;


    }
}
