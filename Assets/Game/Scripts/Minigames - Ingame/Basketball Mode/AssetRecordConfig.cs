using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    [CreateAssetMenu(fileName = "Basketball_AssetRecord Config", menuName = "Gameplay Data/Basketball_AssetRecord", order = 1)]
    public class AssetRecordConfig : ScriptableObject
    {
        public BasketMode modeTest;
        public BasketMode mode1;
        public BasketMode mode2;
        public BasketMode mode3;

        [System.Serializable]
        public struct BasketMode
        {
            public Basket[] records;
        }
    }
}
