using SCN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnhNV.GameBase.PopupManager;

namespace AnhNV.GameBase
{
    public class EventKeyBase : IEventParams
    {
        public struct OpenDialog : IEventParams
        {
            public DialogName dialog;
        }
        public struct OnClosingDialog : IEventParams
        {
            public DialogName dialog;
        }
    }
}
