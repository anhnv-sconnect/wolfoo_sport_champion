using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhNV.GameBase
{
    public interface ITutorialStep
    {
        public void Play();
        public void Stop();
        public void Setup(Transform highlightTarget);
    }
}
