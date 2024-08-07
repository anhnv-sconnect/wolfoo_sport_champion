using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.RelayMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] Transform[] maps;

        private IMinigame.Data myData;
        private MinigameUI ui;
        private int levelScore;
        private int playerScore;

        private int mapCount;
        private (Transform Current, Transform Next) map;

        public Transform GameplayHolder { get => transform; }
        public IMinigame.Data ExternalData { get => myData; set => myData = value; }

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

            // Set Map
            SetMap();

            EventManager.OnInitGame?.Invoke();

            EventManager.OnPlayerClaimNewStar += OnClaimExperience;
            EventManager.OnPlayerIsMoving += OnClaimExperience;
        }
        private void OnDestroy()
        {
            EventManager.OnPlayerClaimNewStar -= OnClaimExperience;
            EventManager.OnPlayerIsMoving -= OnClaimExperience;
        }

        void SetMap()
        {
            if (mapCount >= maps.Length) mapCount = 0;
            map = (maps[mapCount], maps[mapCount + 1 >= maps.Length ? 0 : mapCount + 1]);
            mapCount++;

            map.Next.position = map.Current.position + Vector3.right * 82;
        }

        internal void OnPlayerIsMoving(Base.Player player)
        {
            if(player.transform.position.x >= map.Next.position.x)
            {
                SetMap();
            }
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
