using SCN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnhNV.GameBase.PopupManager;
using static WFSport.Base.ConfigDataManager;

namespace WFSport.Base
{
    public class EventKeyBase : IEventParams
    {
        public struct OpenDialog : IEventParams
        {
            public DialogName dialog;
        }
        public struct CloseDialog : IEventParams
        {
            public DialogName dialog;
        }
        public struct Purchase : IEventParams
        {
            public int id;
            public Gameplay.CreateEnergyMode.FruitScrollItem fruit;
            public Gameplay.CreateEnergyMode.StrawScrollItem straw;
            public Gameplay.FurnitureMode.ToyScrollItem toy;
            public Gameplay.FurnitureMode.OtherScrollItem other;
            public Gameplay.FurnitureMode.ChairScrollItem chair;
        }
        public struct OnWatchAds : IEventParams
        {
            public int id;
        }
        public struct ChangeScene: IEventParams
        {
            public bool home;
            public bool loading;
            public bool gameplay;

            public GameplayConfigData gameplayConfig;
        }   
    }
}
