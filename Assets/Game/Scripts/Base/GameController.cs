using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Gameplay;

namespace WFSport.Base
{
    public class GameController : SingletonResourcesAlive<GameController>
    {
        private LoadSceneManager loadSceneManager;

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
            }
        }

        public T OrderPanel<T>() where T : class
        {
            return null;
        }
    }
}
