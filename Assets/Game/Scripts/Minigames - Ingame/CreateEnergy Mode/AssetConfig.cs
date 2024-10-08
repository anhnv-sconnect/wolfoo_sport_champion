using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CreateEnergyMode
{
    [CreateAssetMenu(fileName = "CreateEnergy_Asset Config", menuName = "Gameplay Data/CreateEnergy_Asset", order = 1)]
    public class AssetConfig : ScriptableObject, IAsset
    {
        public Sprite[] fruitData;
        public Sprite[] strawData;
    }
}
