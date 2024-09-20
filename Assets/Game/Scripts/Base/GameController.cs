using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Gameplay;
using WFSport.Helper;
using static WFSport.Base.ConfigDataManager;

namespace WFSport.Base
{
    public class GameController : SingletonResourcesAlive<GameController>
    {
        public enum Minigame
        {
            Archery,
            BasketBall,
            CatchMoreToys,
            CreateEnergy,
            Latin,
            Relay,
            Snowball
        }

        private LoadSceneManager loadSceneManager;
        private GameObject curMinigame;

        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 60;

            loadSceneManager = GetComponentInChildren<LoadSceneManager>();
            EventDispatcher.Instance.RegisterListener<EventKeyBase.ChangeScene>(OnChangeScene);
            EventDispatcher.Instance.RegisterListener<Gameplay.EventKey.OnGameStop>(OnGameplayComplete);
        }

        protected override void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener<EventKeyBase.ChangeScene>(OnChangeScene);
            EventDispatcher.Instance.RemoveListener<Gameplay.EventKey.OnGameStop>(OnGameplayComplete);
        }

        private void OnGameplayComplete(Gameplay.EventKey.OnGameStop obj)
        {
            loadSceneManager.LoadScene(Constant.SCENE.HOME);
        }

        private void OnChangeScene(EventKeyBase.ChangeScene obj)
        {
            if(obj.home)
            {
                loadSceneManager.LoadScene(Constant.SCENE.HOME);
            }
            else if (obj.loading)
            {
                loadSceneManager.LoadScene(Constant.SCENE.LOADING);
            }
            else if (obj.gameplay)
            {
                loadSceneManager.LoadScene(Constant.SCENE.GAMEPLAY);
                loadSceneManager.OnLoadSuccess = () =>
                {
                    Debug.Log("ONLoad Complete");
                    OnGotoGameplay(obj.gameplayConfig);
                    loadSceneManager.OnLoadSuccess = null;
                };
            }
        }

        private void OnGotoGameplay(GameplayConfigData gameplayConfig)
        {
            DataTransporter.GameplayConfig = gameplayConfig.data;
            var data = DataManager.instance.OrderMinigame(gameplayConfig.Mode);
            if (data != null)
            {
                curMinigame = Instantiate(data);
            }
        }

        public T OrderPanel<T>() where T : class
        {
            return null;
        }
    }
}
