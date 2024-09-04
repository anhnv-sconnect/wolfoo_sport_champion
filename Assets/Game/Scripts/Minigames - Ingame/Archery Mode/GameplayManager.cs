using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace WFSport.Gameplay.ArcheryMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] private GameplayConfig config;
        [SerializeField] private Bot bot;
        [SerializeField] private Player player;
        [SerializeField] private Transform specialArrowIcon;
        [SerializeField] private Transform sortingLayerHolder;
        [SerializeField] private Transform markerHolder;
        [SerializeField] private Transform bombHolder;
        [SerializeField] private IdleMarker[] idleMarkers;
        [SerializeField] private MovingMarker[] movingMarkerPbs;
        [SerializeField] private BombController bombController;

        private IMinigame.Data myData;
        private MinigameUI ui;
        private GameplayManager gameManager;
        private IdleMarker[] curRandomMarkers;
        private MovingMarker[] poolingMovingMarkers;
        private int markedCount;
        private float countTime;
        private int movingMarkedCount;
        private int totalSpecialMarker;
        private Sequence _tweenSpecialArrow;

        public IMinigame.Data ExternalData { get => myData; set => myData = value; }
        public MovingMarker[] MovingMarkers { get => poolingMovingMarkers; }
        public IdleMarker[] IdleMarkers { get => curRandomMarkers; }
        public Vector2 ScreenWidthRange { get; private set; }
        public Vector2 ScreenHeightRange { get; private set; }

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
            foreach (var item in bombHolder.GetComponentsInChildren<SpriteRenderer>())
            {
                item.sortingOrder = Base.Player.SortingLayer(item.transform.position);
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
            _tweenSpecialArrow?.Kill();
        }

        private void PlayAnimPlayerGetSpecialArrow(Vector3 startPos, Vector3 endPos, System.Action OnComplete = null)
        {
            specialArrowIcon.transform.position = startPos;
            specialArrowIcon.transform.localScale = Vector3.one * 2;

            _tweenSpecialArrow?.Kill();
            _tweenSpecialArrow = DOTween.Sequence()
                .AppendInterval(0.2f)
                .AppendCallback(() => specialArrowIcon.gameObject.SetActive(true))
                .Append(specialArrowIcon.DOMoveY(startPos.y + 0.5f, 0.5f))
                .Append(specialArrowIcon.DOJump(endPos, 4, 1, 0.5f))
                .Join(specialArrowIcon.DOScale(Vector3.one * 1, 0.5f));
            _tweenSpecialArrow.OnComplete(() =>
            {
                specialArrowIcon.gameObject.SetActive(false);
                OnComplete?.Invoke();
            });
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

            for (int i = 0; i < config.specialItemSpawnTimelines.Length; i++)
            {
                var item = config.specialItemSpawnTimelines[i];
                if (item == countTime)
                {
                    SpawnSpecialMarker();
                    break;
                }
            }

            for(int i = 0; i< config.bombSpawnTimelines.Length; i++)
            {
                var item = config.bombSpawnTimelines[i];
                if (item == countTime)
                {
                    bombController.CreateBomb();
                }
            }

            if(countTime < myData.playTime) { StartCoroutine("CountingTime"); }
        }

        private void SpawnSpecialMarker()
        {
            totalSpecialMarker++;
        }

        private void SpawnMovingMarker(int totalMarker)
        {
            Vector3 lastMarkerPos = Vector3.zero;
            MovingMarker movingMarker;
            for (int i = 0; i < totalMarker; i++)
            {
                var rdPosIdx = UnityEngine.Random.Range(0, config.movingMarkerYPos.Length);
                var rdMarkerIdx = UnityEngine.Random.Range(0, movingMarkerPbs.Length);
                if (poolingMovingMarkers[movingMarkedCount] == null)
                {
                    movingMarker = Instantiate(movingMarkerPbs[rdMarkerIdx], markerHolder);
                    poolingMovingMarkers[movingMarkedCount] = movingMarker;
                }
                else
                {
                    movingMarker = poolingMovingMarkers[movingMarkedCount];
                }
                movingMarker.Setup(config.delayHideTime,config.movingMarkerYPos[rdPosIdx], config.movingSpeed);
                if (i > 0) { movingMarker.SetupNext(lastMarkerPos, config.movingMarkerSpacing); }
                lastMarkerPos = movingMarker.transform.position;
                movingMarker.Show();

                movingMarkedCount++;
                movingMarkedCount = movingMarkedCount >= poolingMovingMarkers.Length ? 0 : movingMarkedCount;
            }
        }

        private void OnArrowShooting(Arrow obj)
        {
            foreach (var marker in idleMarkers)
            {
                if (!obj.IsAttached && marker.IsInside(obj.transform.position))
                {
                    obj.IsAttached = true;
                    marker.OnHitCorrect(obj.transform.position);

                    markedCount++;
                    if(curRandomMarkers != null && markedCount >= curRandomMarkers.Length)
                    {
                        SpawnNextMarker();
                    }

                    if(marker.IsSpecial)
                    {
                        PlayAnimPlayerGetSpecialArrow(obj.transform.position, player.BowPos, () =>
                        {
                            player.PlayWithSpecialItem();
                        });
                    }

                    return;
                }
            }

            if(poolingMovingMarkers != null)
            {
                for (int i = 0; i < poolingMovingMarkers.Length; i++)
                {
                    if (!obj.IsAttached && poolingMovingMarkers[i] != null && poolingMovingMarkers[i].IsInside(obj.transform.position))
                    {
                        obj.IsAttached = true;
                        poolingMovingMarkers[i].OnHitCorrect(obj.transform.position);
                        return;
                    }
                }
            }

            var isShootedBomb = bombController.IsArrowShooted(obj);
            if (isShootedBomb) {
                // Decrease Score
                StopCoroutine("OnShootedBomb");
                StartCoroutine("OnShootedBomb");
            }
        }
        private IEnumerator OnShootedBomb()
        {
            player.Pause(false);

            yield return new WaitForSeconds(config.bombUsedTime);

            player.Play();
        }
        private void InitMovingMarker()
        {
            var maxWidth = 0f;
            var deltaTime = 0f;
            var maxSpawningObject = 0;
            var data = config.movingMarkerSpawnTimes;
            var rangeTime = data.Length > 0 ? data[data.Length - 1].spawnTime - data[0].spawnTime : 0f;
            var totalObject = 0;

            foreach (var item in movingMarkerPbs)
            {
                maxWidth = Mathf.Max(item.Width, maxWidth);
                deltaTime = (config.movingMarkerSpacing + maxWidth) / config.movingSpeed;
            }
            var distance = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth + maxWidth, 0, Camera.main.transform.position.z));

            for (int i = 0; i < config.movingMarkerSpawnTimes.Length; i++)
            {
                var item = config.movingMarkerSpawnTimes[i];
                maxSpawningObject = Mathf.Max(item.totalSpawningMarker, maxSpawningObject);
                totalObject += config.movingMarkerSpawnTimes[i].totalSpawningMarker;
            }

            var totalMovingTime = (maxSpawningObject) * (deltaTime) + (distance.x / config.movingSpeed);
            var totalReloadObject = rangeTime / totalMovingTime;
            totalObject -= (int) totalReloadObject;

            poolingMovingMarkers = new MovingMarker[totalObject];
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

            var maxRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, Camera.main.transform.position.z));
            var maxUp = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, Camera.main.transform.position.z));
            ScreenWidthRange = new Vector2(-maxRight.x, maxRight.x);
            ScreenHeightRange = new Vector2(-maxUp.y, maxUp.y);

            InitMovingMarker();
            bombController.Setup(config);

            specialArrowIcon.gameObject.SetActive(false);
            player.Init();
            bot.Init();
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
                if(totalSpecialMarker > 0)
                {
                    idleMarkers[idx].SetupSpecial();
                    totalSpecialMarker--;
                }
                idleMarkers[idx].Show();
                curRandomMarkers[i] = idleMarkers[idx];
            }

            yield return new WaitForSeconds(config.randomSpawnTime);

            StartCoroutine("SetupNextMarker");
        }

        public void OnGameLosing()
        {
            player.Pause(false);
            bot.Pause(false);
        }

        public void OnGamePause()
        {
            player.Pause(true);
            bot.Pause(false);
        }

        public void OnGameResume()
        {
            player.Play();
            bot.Play();
        }

        public void OnGameStart()
        {
            SpawnNextMarker();
            player.Play();
            bot.Play();
            StartCoroutine("CountingTime");
        }

        public void OnGameStop()
        {
            player.Pause(false);
            bot.Pause(false);
        }

        public void OnGameWining()
        {
            player.Pause(false);
            bot.Pause(false);
        }
    }
}
