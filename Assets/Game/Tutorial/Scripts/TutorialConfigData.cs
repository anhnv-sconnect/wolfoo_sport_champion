using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhNV.GameBase
{
    [CreateAssetMenu(fileName = "Tutorial Data", menuName = "Tutorial Data Config")]
    public class TutorialConfigData : ScriptableObject
    {
        public TutorialStep[] tutorialSteps;

        public TutorialStep GetData<T>() where T: TutorialStep
        {
            for (int i = 0; i < tutorialSteps.Length; i++)
            {
                if(tutorialSteps[i] != null)
                {
                    if (tutorialSteps[i].GetType() == typeof(T))
                    {
                        return tutorialSteps[i];
                    }
                }
            }

            return null;
        }
    }
}
