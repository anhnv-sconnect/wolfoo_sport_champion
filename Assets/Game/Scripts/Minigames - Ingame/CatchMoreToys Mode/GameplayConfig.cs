using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    [CreateAssetMenu(fileName = "CatchMoreToy Config", menuName = "Gameplay Data/Catch More Toy", order = 1)]
    public class GameplayConfig : ScriptableObject
    {
        [Header("=============== SCREEN ===============")]
        public Vector3 ipadSize;
        public Vector3 normalSize;

        [Header("=============== PLAYER ===============")]
        public float limitLeft;
        public float limitRight;
        public float velocity;
        public float stunningTime;

        [Header("=============== BOT ===============")]
        public Vector2 amplitudeRange = new Vector2(2, 2.5f);
        public Vector2 speed = new Vector2(3, 4);
        public Vector2 torqueForceRange = new Vector2(0.5f, 1.5f);
        [Range(0, 100)] public float itemRandomPercentage;
        [Range(0, 100)] public float bonusitemRandomPercentage;
        [Range(0, 100)] public float obstacleRandomPercentage;
        public float[] bonusItemSpawnTime;
        public float[] obstacleSpawnTime;
        [Range(0, 100)] public float reloadTime;
        public string[] rightHandBoneNames;

        [Header("=============== TUTORIAl ===============")]
        public Vector2 tempurareSpeed;
        public int readyMachineIdx ;

    }
}
