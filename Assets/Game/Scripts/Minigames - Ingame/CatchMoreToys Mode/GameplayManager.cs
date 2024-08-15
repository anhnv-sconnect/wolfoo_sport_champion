using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] Canvas bgCanvas;
        [SerializeField] GameplayConfig config;
        [SerializeField] ThrowingMachine[] throwMachines;
        [SerializeField] GameObject[] itemData;
        [SerializeField] GameObject[] bonusItemData;
        [SerializeField] GameObject[] obstacleData;
        [SerializeField] CharacterWorldAnimation[] characters;

        private IMinigame.Data myData;
        private MinigameUI ui;
        private ThrowingMachine curCharacter;
        private (int count, int index) obstacleSpawner;
        private (int count, int index) bonusItemSpawner;

        public IMinigame.Data ExternalData { get => myData; set => myData = value; }

        private void Awake()
        {
            ui = FindAnyObjectByType<MinigameUI>();
            bgCanvas.worldCamera = Camera.main;

            /// Setup Object to Fit screen
        }
        private void Start()
        {
            Init();
            OnGameStart();
        }

        void Init()
        {
            /// Spawn character in ThrowingMachine
            foreach (var machine in throwMachines)
            {
                var idx = UnityEngine.Random.Range(0, characters.Length);
                var character = Instantiate(characters[idx], machine.transform);
                machine.Setup(transform, this, config, character);
            }
        }

        private IEnumerator CountSpawnTime()
        {
            bonusItemSpawner.count++;
            obstacleSpawner.count++;
            yield return new WaitForSeconds(1);
            StartCoroutine("CountSpawnTime");
        }

        internal (GameObject prefab, Sprite sprite) GetRandomItem()
        {
            var rd = UnityEngine.Random.Range(0, 100);
            if(rd <= config.itemRandomPercentage)
            {
                // spawn Item
                var idx = UnityEngine.Random.Range(0, itemData.Length);
                return (itemData[idx], itemData[idx].GetComponentInChildren<SpriteRenderer>().sprite);
            }
            else if(rd <= config.bonusitemRandomPercentage)
            {
                // spawn Bonus Item
                var idx = UnityEngine.Random.Range(0, bonusItemData.Length);
                return (bonusItemData[idx], bonusItemData[idx].GetComponentInChildren<SpriteRenderer>().sprite);
            }
            else
            {
                // spawn Obstacle
                var idx = UnityEngine.Random.Range(0, obstacleData.Length);
                return (obstacleData[idx], obstacleData[idx].GetComponentInChildren<SpriteRenderer>().sprite);
            }
        }
        internal (GameObject prefab, Sprite sprite) GetAutoNextItem()
        {
            int idx;
            if(bonusItemSpawner.index < config.bonusItemSpawnTime.Length &&
                bonusItemSpawner.count >= config.bonusItemSpawnTime[bonusItemSpawner.index])
            {
                bonusItemSpawner.index++;
                // spawn Bonus Item
                idx = UnityEngine.Random.Range(0, bonusItemData.Length);
                return (bonusItemData[idx], bonusItemData[idx].GetComponentInChildren<SpriteRenderer>().sprite);
            }

            if(obstacleSpawner.index < config.obstacleSpawnTime.Length &&
                obstacleSpawner.count >= config.obstacleSpawnTime[obstacleSpawner.index])
            {
                obstacleSpawner.index++;
                // spawn Obstacle
                idx = UnityEngine.Random.Range(0, obstacleData.Length);
                return (obstacleData[idx], obstacleData[idx].GetComponentInChildren<SpriteRenderer>().sprite);
            }

            // spawn Item
            idx = UnityEngine.Random.Range(0, itemData.Length);
            return (itemData[idx], itemData[idx].GetComponentInChildren<SpriteRenderer>().sprite);
        }

        public void OnGamePause()
        {
            foreach (var item in throwMachines)
            {
                item.StopThrow();
            }
            ui.PauseTime();
            StopCoroutine("CountSpawnTime");
        }

        public void OnGameResume()
        {
            ui.PlayTime();
            StartCoroutine("CountSpawnTime");
        }

        private void PlayNextCharacter()
        {
            var idx = UnityEngine.Random.Range(0, throwMachines.Length);
            curCharacter = throwMachines[idx];
            curCharacter.Throw(() =>
            {
                PlayNextCharacter();
            });
        }

        public void OnGameStart()
        {
            PlayNextCharacter();
            ui.PlayTime();
            StartCoroutine("CountSpawnTime");
        }

        public void OnGameStop()
        {
        }

    }
}
