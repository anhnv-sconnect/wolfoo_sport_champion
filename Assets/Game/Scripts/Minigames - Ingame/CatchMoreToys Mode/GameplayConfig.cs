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
    }
}
