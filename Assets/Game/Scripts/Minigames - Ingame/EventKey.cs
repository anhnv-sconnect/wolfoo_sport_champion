using SCN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Base;

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
            public IMinigame.ResultData data;
        }
        public struct OnGamePause : IEventParams
        {
            public CatchMoreToysMode.GameplayManager catchMoreToys;
        }
        public struct UnlockLocalData : IEventParams
        {
            public int id;
            public bool isFruit;
            public bool isStraw;
            public bool isChair;
            public bool isToy;
            public bool isOther;
            public GameObject obj;
            public PurchaseType purchaseType;
            public int amount;
        }
    }
}
