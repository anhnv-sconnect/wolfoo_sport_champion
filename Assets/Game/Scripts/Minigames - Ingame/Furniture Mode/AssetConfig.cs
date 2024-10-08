using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.FurnitureMode
{
    [CreateAssetMenu(fileName = "Furniture_Asset Config", menuName = "Gameplay Data/Furniture_Asset", order = 1)]
    public class AssetConfig : ScriptableObject, IAsset
    {
        public Sprite[] toyData;
        public Sprite[] chairData;
        public Sprite[] otherData;
    }
}
