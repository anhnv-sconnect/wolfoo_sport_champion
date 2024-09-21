using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.GameController;
using static WFSport.Gameplay.IMinigame;

namespace WFSport.Base
{
    [CreateAssetMenu(fileName = "DataCore Config", menuName = "Core Data /ConfigData Minigames", order = 1)]
    public class ConfigDataManager : ScriptableObject
    {
        public GameplayConfigData[] GameplayConfig;

        [System.Serializable]
        public struct GameplayConfigData
        {
            public string Name;
            public Minigame Mode;
            public GameObject icon;
            public ConfigData data;
        }
    }
}
