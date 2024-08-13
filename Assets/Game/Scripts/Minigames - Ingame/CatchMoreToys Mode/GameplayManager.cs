using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        private IMinigame.Data myData;
        private MinigameUI ui;

        public IMinigame.Data ExternalData { get => myData; set => myData = value; }

        private void Awake()
        {
            ui = FindAnyObjectByType<MinigameUI>();
        }
        private void Start()
        {
            OnGameStart();
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
    }
}
