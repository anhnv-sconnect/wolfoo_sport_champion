namespace WFSport
{
    namespace Base
    {
        public static class Constant
        {
            public struct TAG
            {
                public const string GROUND      = "Ground";
                public const string OBSTACLE    = "Obstacle";
                public const string PLAYER      = "Player";
                public const string BONUSITEM   = "BonusItem";
                public const string FINISH      = "Finish";
                public const string DEATHZONE   = "Death";
                public const string STAR        = "Star";
                public const string SNOW        = "Snow";
                public const string STAGE       = "Stage";
            }
            public struct LAYER
            {
                public const int UI = 5;
                public const int CHARACTER = 6;
            }
            public struct SCENE
            {
                public const string GAMEPLAY = "Gameplay Scene";
                public const string HOME = "Home Scene";
                public const string LOADING = "Loading Scene";
            }
        }
    }

    namespace Gameplay.RelayMode
    {
        public static class Constant
        {
            public const float LINE1 = 0.2f;
            public const float LINE2 = -2;
            public const float LINE3 = -4;
            public const float CONE_LINE1 = 1.2f;
            public const float CONE_LINE2 = -0.9f;
            public const float CONE_LINE3 = -2.97f;
            public const float BELOW_LIMIT = -3.2f;
            public const float ABOVE_LIMIT = 1.2f;
            public const float MID = (BELOW_LIMIT + ABOVE_LIMIT) / 2;

            public const float DELAY_FINISHED_TIME = 2;
        }
    }
}
