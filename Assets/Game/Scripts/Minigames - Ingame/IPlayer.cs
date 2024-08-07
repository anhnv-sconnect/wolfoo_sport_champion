using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay
{
    public interface IPlayer
    {
        public void Init();
        public void OnUpdate();

        public enum DetectType
        {
            Swiping,
            Dragging
        }
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
