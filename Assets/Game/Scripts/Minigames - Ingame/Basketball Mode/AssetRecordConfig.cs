using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    [CreateAssetMenu(fileName = "Basketball_AssetRecord Config", menuName = "Gameplay Data/Basketball_AssetRecord", order = 1)]
    public class AssetRecordConfig : ScriptableObject
    {
        [Header("=========== BONUS ITEM ==========")]
        public Sprite[] claimScoreSprites;
    }
}
