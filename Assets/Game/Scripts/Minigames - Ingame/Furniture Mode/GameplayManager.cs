using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.FurnitureMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        private IMinigame.ResultData result;
        private IMinigame.ConfigData myData;

        public IMinigame.ConfigData InternalData { get =>  myData; set => myData = value; }
        IMinigame.ResultData IMinigame.ExternalData { get => result; set => result = value; }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
        }

        public void OnGameLosing()
        {
        }

        public void OnGamePause()
        {
        }

        public void OnGameResume()
        {
        }

        public void OnGameStart()
        {
        }

        public void OnGameStop()
        {
        }

        public void OnGameWining()
        {
        }
    }
}
