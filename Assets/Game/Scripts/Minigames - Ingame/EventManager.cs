using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay
{
    public class EventManager
    {
        public static Action OnInitGame;
        public static Action OnFinishStage;
        public static Action<Base.Player> OnPlayerClaimNewStar;
    }
}
