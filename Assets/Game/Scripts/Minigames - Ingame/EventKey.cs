using SCN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay
{
    public class EventKey : IEventParams
    {
        public struct OnGameResume : IEventParams
        {
            public CatchMoreToysMode.GameplayManager catchMoreToys;
        }
        public struct OnGameStop : IEventParams
        {
            public CatchMoreToysMode.GameplayManager catchMoreToys;
        }
        public struct OnGamePause : IEventParams
        {
            public CatchMoreToysMode.GameplayManager catchMoreToys;
        }
    }
}
