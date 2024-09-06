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

        [Header("=============== BASKET ===============")]
        public float height;
    }
}
