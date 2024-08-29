using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace WFSport.Gameplay.ArcheryMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] private Player player;
        [SerializeField] private Transform sortingLayerHolder;
        [SerializeField] private Transform markerHolder;
        [SerializeField] private IdleMarker[] idleMarkers;
        [SerializeField] private MovingMarker[] movingMarkerPbs;
        [SerializeField] private GameplayConfig config;

        private IMinigame.Data myData;
        private MinigameUI ui;
        private IdleMarker[] curRandomMarkers;
        [SerializeField] private MovingMarker[] curMovingMarkers;
        private int markedCount;
        private float countTime;
        private int movingMarkedCount;

        public IMinigame.Data ExternalData { get => myData; set => myData = value; }

        [NaughtyAttributes.Button]
        private void SortingLayer()
        {
            foreach (var item in sortingLayerHolder.GetComponentsInChildren<SpriteRenderer>())
            {
                item.sortingOrder = Base.Player.SortingLayer(item.transform.position);
            }
            var records = markerHolder.GetComponentsInChildren<IdleMarker>();
            idleMarkers = new IdleMarker[records.Length];
            for (int i = 0; i < records.Length; i++)
            {
                records[i].GetComponent<SortingGroup>().sortingOrder = Base.Player.SortingLayer(records[i].transform.position);
                idleMarkers[i] = records[i];
            }
        }
        private void Start()
        {
            EventManager.OnShooting += OnArrowShooting;
            Init();
            OnGameStart();
        }
        private void OnDestroy()
        {
            EventManager.OnShooting -= OnArrowShooting;
            StopCoroutine("SetupNextMarker");
            StopCoroutine("CountingTime");
        }

        private IEnumerator CountingTime()
        {
            yield return new WaitForSeconds(1);

            countTime++;

            for (int i = 0; i < config.movingMarkerSpawnTimes.Length; i++)
            {
                var item = config.movingMarkerSpawnTimes[i];
                if (item.spawnTime == countTime)
                {
                    SpawnMovingMarker(item.totalSpawningMarker);
                    break;
                }
            }

            if(countTime < myData.playTime) { StartCoroutine("CountingTime"); }
        }

        private void SpawnMovingMarker(int totalMarker)
        {
            Vector3 lastMarkerPos = Vector3.zero;
            MovingMarker movingMarker;
            for (int i = 0; i < totalMarker; i++)
            {
                var rdPosIdx = UnityEngine.Random.Range(0, config.movingMarkerYPos.Length);
                var rdMarkerIdx = UnityEngine.Random.Range(0, movingMarkerPbs.Length);
                if (curMovingMarkers[movingMarkedCount] == null)
                {
                    movingMarker = Instantiate(movingMarkerPbs[rdMarkerIdx], markerHolder);
                    curMovingMarkers[movingMarkedCount] = movingMarker;
                }
                else
                {
                    movingMarker = curMovingMarkers[movingMarkedCount];
                }
                movingMarker.Setup(config.delayHideTime,config.movingMarkerYPos[rdPosIdx], config.movingSpeed);
                if (i > 0) { movingMarker.SetupNext(lastMarkerPos, config.movingMarkerSpacing); }
                lastMarkerPos = movingMarker.transform.position;
                movingMarker.Show();

                movingMarkedCount++;
                movingMarkedCount = movingMarkedCount >= curMovingMarkers.Length ? 0 : movingMarkedCount;
            }
        }

        private void OnArrowShooting(Arrow obj)
        {
            foreach (var marker in idleMarkers)
            {
                if (marker.IsInside(obj.transform.position))
                {
                    markedCount++;
                    if(curRandomMarkers != null && markedCount >= curRandomMarkers.Length)
                    {
                        SpawnNextMarker();
                    }
                }
            }
            if(curMovingMarkers != null)
            {
                for (int i = 0; i < curMovingMarkers.Length; i++)
                {
                    if (curMovingMarkers[i] != null && curMovingMarkers[i].IsInside(obj.transform.position))
                    {
                  //      curMovingMarkers[i].Hide();
                    }
                }
            }
        }
        private void InitMovingMarker()
        {
            var maxWidth = 0f;
            var deltaTime = 0f;
            var maxSpawningObject = 0;
            var reloadMovingTime = 0f;

            foreach (var item in movingMarkerPbs)
            {
                maxWidth = Mathf.Max(item.Width, maxWidth);
                deltaTime = (config.movingMarkerSpacing + maxWidth) / config.movingSpeed;
            }
            for (int i = 0; i < config.movingMarkerSpawnTimes.Length; i++)
            {
                var item = config.movingMarkerSpawnTimes[i];
                maxSpawningObject = Mathf.Max(item.totalSpawningMarker, maxSpawningObject);
                var range = 0f;
                if (i == 0)
                {
                    reloadMovingTime = item.spawnTime;
                    range = item.spawnTime;
                }
                if(i > 0) { range = item.spawnTime - config.movingMarkerSpawnTimes[i - 1].spawnTime; }
                reloadMovingTime = Mathf.Min(reloadMovingTime, Mathf.Abs(range));
            }

            var distance = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth + maxWidth, 0, Camera.main.transform.position.z));

            var fps = Time.frameCount / Time.time;
            var totalMovingTime = (maxSpawningObject) * (deltaTime) + (distance.x / config.movingSpeed);
            Debug.Log("TOtal TIMe " + totalMovingTime);
            Debug.Log("Reload TIme " + reloadMovingTime);
            int totalObject = (int)(totalMovingTime / reloadMovingTime) + maxSpawningObject;
            curMovingMarkers = new MovingMarker[totalObject];
        }

        private void Init()
        {
            if(myData == null)
            {
                ExternalData = new IMinigame.Data()
                {
                    coin = 0,
                    playTime = 90,
                    timelineScore = new float[] { 10, 20, 40 },
                };
            }
            ui = FindAnyObjectByType<MinigameUI>();

            InitMovingMarker();

            player.Init();
        }
        private void SpawnNextMarker()
        {
            StopCoroutine("SetupNextMarker");
            StartCoroutine("SetupNextMarker");
        }
        private IEnumerator SetupNextMarker()
        {
            var maxRange = config.randomRange;
            if (curRandomMarkers != null)
            {
                foreach (var item in curRandomMarkers)
                {
                    item.Hide();
                }
                maxRange = Mathf.Min(idleMarkers.Length - curRandomMarkers.Length, config.randomRange);
            }

            markedCount = 0;
            var totalRandom = UnityEngine.Random.Range(1, maxRange);
            curRandomMarkers = new IdleMarker[totalRandom];

            for (int i = 0; i < totalRandom; i++)
            {
                int idx = UnityEngine.Random.Range(0, idleMarkers.Length);
                while (idleMarkers[idx].IsShowing)
                {
                    idx = UnityEngine.Random.Range(0, idleMarkers.Length);
                }
                idleMarkers[idx].Setup(config.delayHideTime);
                idleMarkers[idx].Show();
                curRandomMarkers[i] = idleMarkers[idx];
            }

            yield return new WaitForSeconds(config.randomSpawnTime);

            StartCoroutine("SetupNextMarker");
        }

        public void OnGameLosing()
        {
            player.Pause(false);
        }

        public void OnGamePause()
        {
            player.Pause(true);
        }

        public void OnGameResume()
        {
            player.Play();
        }

        public void OnGameStart()
        {
            SpawnNextMarker();
            player.Play();
            StartCoroutine("CountingTime");
        }

        public void OnGameStop()
        {
            player.Pause(false);
        }

        public void OnGameWining()
        {
            player.Pause(false);
        }
    }
}
