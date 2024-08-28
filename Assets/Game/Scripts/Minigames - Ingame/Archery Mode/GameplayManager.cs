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
        [SerializeField] private Marker[] markers;
        [SerializeField] private GameplayConfig config;

        private IMinigame.Data myData;
        private MinigameUI ui;
        private Marker curMarker;

        public IMinigame.Data ExternalData { get => myData; set => myData = value; }

        [NaughtyAttributes.Button]
        private void SortingLayer()
        {
            foreach (var item in sortingLayerHolder.GetComponentsInChildren<SpriteRenderer>())
            {
                item.sortingOrder = Base.Player.SortingLayer(item.transform.position);
            }
            var records = markerHolder.GetComponentsInChildren<Marker>();
            markers = new Marker[records.Length];
            for (int i = 0; i < records.Length; i++)
            {
                records[i].GetComponent<SortingGroup>().sortingOrder = Base.Player.SortingLayer(records[i].transform.position);
                markers[i] = records[i];

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
        }

        private void OnArrowShooting(Arrow obj)
        {
            foreach (var marker in markers)
            {
                if (marker.IsInside(obj.transform.position))
                {
                    SpawnNextMarker();
                }
            }
        }

        private void Init()
        {
            ui = FindAnyObjectByType<MinigameUI>();

            player.Init();
        }
        private void SpawnNextMarker()
        {
            StopCoroutine("SetupNextMarker");
            StartCoroutine("SetupNextMarker");
        }
        private IEnumerator SetupNextMarker()
        {
            if (curMarker != null) curMarker.Hide();

            int idx = UnityEngine.Random.Range(0, markers.Length);
            while (markers[idx].IsShowing)
            {
                idx = UnityEngine.Random.Range(0, markers.Length);
            }
            markers[idx].Setup(config.delayHideTime);
            markers[idx].Show();
            curMarker = markers[idx];

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
