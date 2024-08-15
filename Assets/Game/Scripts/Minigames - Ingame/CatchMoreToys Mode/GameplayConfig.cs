using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    [CreateAssetMenu(fileName = "CatchMoreToy Config", menuName = "Gameplay Data/Catch More Toy", order = 1)]
    public class GameplayConfig : ScriptableObject
    {
        [Header("=============== PLAYER ===============")]
        public float limitLeft;
        public float limitRight;
        public float velocity;
        public float stunningTime;

        [Header("=============== BOT ===============")]
        [Range(0, 100)] public float itemRandomPercentage;
        [Range(0, 100)] public float bonusitemRandomPercentage;
        [Range(0, 100)] public float obstacleRandomPercentage;
        public float[] bonusItemSpawnTime;
        public float[] obstacleSpawnTime;
        [Range(0, 100)] public float reloadTime;
        public string[] rightHandBoneNames;
    }
}
