using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay
{
    public interface IPlayer
    {
        public void Init();
        public void OnUpdate();

        public enum Mode
        {
            Swiping,
            Running
        }
        public enum SwipingDirection
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
