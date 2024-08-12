using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhNV.GameBase
{
    public class TutorialExample : MonoBehaviour
    {
        [SerializeField] Transform testObj;
        // Start is called before the first frame update
        void Start()
        {
            var tutorialSaveLoad = new SaveLoadDataTutorial();
            if (tutorialSaveLoad.HasTutorial1)
            {
                var tutorialController = TutorialController.Instance;
                var tutorial = tutorialController.CreateTutorial("Testing");
                var step1 = tutorialController.CreateStep<TutorialWithBG>(tutorial);
                step1.Setup(testObj);

                tutorial.PlayNextStep();
            }
        }
    }
}
