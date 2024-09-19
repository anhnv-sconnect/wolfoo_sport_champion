using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhNV.GameBase
{
    public interface ITutorial
    {
        public void Register(TutorialStep step);
        public void Remove(TutorialStep step);
        public void PlayNextStep();
        public System.Action<TutorialStep> OnCompleteStep { get; set; }
    }
}
