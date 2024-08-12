using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhNV.GameBase
{
    public class AnimatorHelper : MonoBehaviour
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }
        public System.Action OnPlayComplete;
        public System.Action OnCloseComplete;

        public void OnPlayingComplete()
        {
            OnPlayComplete?.Invoke();
        }
        public void OnClosingComplete()
        {
            OnCloseComplete?.Invoke();
        }
    }
}
