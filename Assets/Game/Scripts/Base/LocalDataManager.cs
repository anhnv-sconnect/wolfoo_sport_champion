using AnhNV.GameBase;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Gameplay.BasketballMode;

namespace WFSport.Base
{
    public class LocalDataManager: MonoBehaviour
    {
        public PlayerMe playerMe;
        public LocalDataCreateEnergy createEnergyData;
        public LocalDataFurniture furnitureData;
        public TutorialLocalData tutorialData;

        public bool IsLoadCompleted { get; private set; }

        public void Load()
        {
            playerMe.Load();
            if (playerMe == null)
            {
                playerMe.Init();
                playerMe.Save();
            }

            createEnergyData.Load();
            if (createEnergyData == null) createEnergyData.Save();

            furnitureData.Load();
            if (furnitureData == null) furnitureData.Save();

            tutorialData.Load();
            if (tutorialData == null) tutorialData.Save();

            IsLoadCompleted = true;
        }
    }
}
