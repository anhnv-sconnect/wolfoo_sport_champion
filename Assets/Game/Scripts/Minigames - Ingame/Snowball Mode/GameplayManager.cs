using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.SnowballMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] Snow[] snows;

        private IMinigame.Data myData;

        public IMinigame.Data ExternalData { get => myData; set => myData = value; }

        // Start is called before the first frame update
        void Start()
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
