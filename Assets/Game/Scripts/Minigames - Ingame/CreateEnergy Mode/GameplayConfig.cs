using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CreateEnergyMode
{
    [CreateAssetMenu(fileName = "CreateEnergy Config", menuName = "Gameplay Data/CreateEnergy", order = 1)]
    public class GameplayConfig : ScriptableObject
    {
        [Header("====================== Blender ====================")]
        public int maxFruitInBlender = 3;
        public float pouringTime;
    }
}
