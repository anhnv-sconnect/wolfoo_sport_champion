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
        public static Action<RelayMode.TrafficCone> OnTriggleWithCone;
        public static Action<RelayMode.TrafficCone> OnTracking;
        public static Action<Base.Player, IPlayer.Direction> OnPlayerSwiping;
        public static Action<Base.Player> OnPlayerClaimNewStar;
        public static Action<Base.Player> OnPlayerIsMoving;
        public static Action<Base.Player> OnPlayerIsPassedHalfStage;
        public static Action OnTimeOut;
        public static Action<RelayMode.Barrier, float> OnBarrierCompareDistanceWithPlayer;
        public static Action<CatchMoreToysMode.Item> OnToyIsFlying;
        public static Action<LatinDanceMode.BonusItem> OnHide;
        /// <summary>
        /// Param1 : Target, Param2: isFocusing (Black Screen -> highlight Target)
        /// </summary>
        public static Action<Transform, bool> OnHighlight;
        public static Action<Transform> OnStopHighlight;
        public static Action OnScratching;
        public static Action<ArcheryMode.Marker> OnMarkerIsSpawing;
        public static Action<ArcheryMode.Marker> OnMarkerIsHiding;
        public static Action<ArcheryMode.Arrow> OnShooting;
        public static Action<BasketballMode.Player, BasketballMode.Basket> OnBallTracking;
        public static Action<BasketballMode.Player, Vector3> OnThrow;
        public static Action<BasketballMode.Basket> DelayAll;
        public static Action<Base.Player, Vector3> OnGetScore;

        public static Action<CreateEnergyMode.Fruit> OnFruitJumpIn;
    }
}
