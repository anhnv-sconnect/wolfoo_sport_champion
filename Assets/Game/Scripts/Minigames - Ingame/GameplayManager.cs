using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.RelayMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        public IMinigame.Data ExternalData { get => myData; set => myData = value; }
        private IMinigame.Data myData;

        public Transform GameplayHolder { get => transform; }

        private int levelScore;
        private int playerScore;
        private MinigameUI ui;

        private void Awake()
        {
            ui = FindAnyObjectByType<MinigameUI>();
        }

        private void Start()
        {
            if(myData == null)
            {
                myData = new IMinigame.Data()
                {
                    coin = 0,
                    score = 0,
                };
            }
            levelScore = 10;
            EventManager.OnInitGame?.Invoke();
        }
        private void OnEnable()
        {
            EventManager.OnPlayerClaimNewStar += OnClaimExperience;
        }
        private void OnDisable()
        {
            EventManager.OnPlayerClaimNewStar -= OnClaimExperience;
        }

        internal void OnClaimExperience(Base.Player player)
        {
            playerScore++;
            ui.UpdateLoadingBar((float)playerScore / levelScore);
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
