using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhNV.GameBase
{
    public class TutorialController : SingletonBind<TutorialController>
    {
        [SerializeField]private TutorialConfigData data;
        private bool isOrdered;
        private List<Tutorial> orderTutorials = new List<Tutorial>();

        private void Start()
        {
            InitData();
        }
        private void InitData()
        {
            if (isOrdered) return;
            isOrdered = true;

            // INit Config Data
        }
        public void ReleaseAll()
        {
            for (int i = 0; i < orderTutorials.Count; i++)
            {
                orderTutorials[i].ReleaseAll();
                orderTutorials.RemoveAt(i);
            }
        }
        public Tutorial CreateTutorial(string Name)
        {
            var tutorial = new Tutorial(Name);
            orderTutorials.Add(tutorial);
            return tutorial;
        }
        public T CreateStep<T>(Tutorial tutorial) where T : TutorialStep
        {
            if(data == null)
            {
                Debug.LogError("Create your DATA Setting at First !!!");
                return null;
            }
            var tutPb = data.GetData<T>();
            if(tutPb == null)
            {
                Debug.LogError("Your Tutorial Step is Not Declare in DATA setting !!!");
                return null;
            }

            var tutStep = Instantiate(tutPb);
            tutorial.Register(tutStep);
            tutStep.TutorialID = tutorial.ID;

            return tutStep.GetComponent<T>();
        }
    }
}
