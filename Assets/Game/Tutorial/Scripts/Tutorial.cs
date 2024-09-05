using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhNV.GameBase
{
    public class Tutorial : ITutorial
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public bool IsAllStepCompleted
        {
            get
            {
                var allStepisDone = mySteps.TrueForAll(a => a.IsCompleted);
                return allStepisDone;
            }
        }

        private int countStep;
        private List<TutorialStep> mySteps;
        private TutorialStep currentNextStep;

        public Action<TutorialStep> OnCompleteStep { get; set; }

        public Tutorial(string name)
        {
            ID = System.Guid.NewGuid().ToString();
            Name = name;
        }

        public T GetStep<T>(int idx) where T : TutorialStep
        {
            if (idx >= mySteps.Count) return null;

            return mySteps[idx] as T;
        }

        public TutorialStep GetNextStep()
        {
            if (countStep >= mySteps.Count)
            {
                Debug.Log("All tutorial Step is COMPLETED !!");
                return null;
            }

            var step = mySteps[countStep];
            countStep++;

            if (countStep >= mySteps.Count)
            {
                Debug.Log("All tutorial Step is COMPLETED !!");
            }

            return step;
        }

        public void PlayNextStep()
        {
            if (IsAllStepCompleted) return;

            currentNextStep = GetNextStep();
            if (currentNextStep != null) currentNextStep.Play();
        }
        public void PlayCurrentStep()
        {
            if (countStep >= mySteps.Count)
            {
                mySteps[mySteps.Count - 1].Play();
            }
            else
            {
                mySteps[countStep].Play();
            }
        }
        public void Stop()
        {
            if (currentNextStep != null)
                currentNextStep.Stop();
        }

        public void ReleaseAll()
        {
            for (int i = 0; i < mySteps.Count; i++)
            {
                mySteps[i].Release();
                mySteps.RemoveAt(i);
            }
        }

        public void SetCompletedCurrentStep()
        {
            if (currentNextStep != null)
                currentNextStep.Completed();
        }

        public void Register(TutorialStep step)
        {
            if (mySteps == null) mySteps = new List<TutorialStep>();
            mySteps.Add(step);
        }

        public void Remove(TutorialStep step)
        {
            if (step.TutorialID == ID)
            {
                mySteps.Remove(step);
            }
            else
            {
                Debug.LogWarning($"This step {step} is NOT EXIST in Tutorial {this}");
            }
        }
    }
}
