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
        public struct ChangeScene: IEventParams
        {
            public bool home;
            public bool loading;
            public bool gameplay;

            public GameplayConfigData gameplayConfig;
        }   
    }
}
