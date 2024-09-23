using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Base
{
    [CreateAssetMenu(fileName = "Asset Manager", menuName = "Core Data /Asset Manager", order = 1)]
    public class AssetDataManager : ScriptableObject
    {
        public Gameplay.FurnitureMode.Asset FurnitureAsset;
    }
}
