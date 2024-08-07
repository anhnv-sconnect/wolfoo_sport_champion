using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport
{
    namespace Base
    {
        public static class Constant
        {
            public struct TAG
            {
                public const string GROUND = "Ground";
                public const string OBSTACLE = "Obstacle";
                public const string PLAYER = "Player";
                public const string BONUSITEM = "BonusItem";
                public const string FINISH = "Finish";
            }

            public static TAG Tag;
        }
    }

    namespace Gameplay.RelayMode
    {
        public static class Constant
        {
            public const float LINE1 = 0.2f;
            public const float LINE2 = -2;
            public const float LINE3 = -4;
        }
    }
}
