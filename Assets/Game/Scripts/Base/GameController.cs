using AnhNV.Helper;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Helper;
using static WFSport.Base.ConfigDataManager;

namespace WFSport.Base
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
    public enum PurchaseType
    {
        Ads,
        Coin
    }

    public class GameController : SingletonResourcesAlive<GameController>
    {
        private LoadSceneManager loadSceneManager;
        private GameObject curMinigame;
        private PlayerMe playerMe;
        private MinigameSystemUI systemUI;
        private LocalDataManager localData;

        public Stack<Action> purchaseActions;
        private Gameplay.EventKey.UnlockLocalData purchaseData;

        public MinigameSystemUI SystemUI { get => systemUI; }

        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 60;

            loadSceneManager = GetComponentInChildren<LoadSceneManager>();
            playerMe = DataManager.instance.localSaveloadData.playerMe;
            systemUI = GetComponentInChildren<MinigameSystemUI>();
            systemUI.ClickBackBtn = OnClickBackBtn;

            localData = DataManager.instance.localSaveloadData;
            localData.Load();

            StartCoroutine("Play");

            EventDispatcher.Instance.RegisterListener<EventKeyBase.ChangeScene>(OnChangeScene);
            EventDispatcher.Instance.RegisterListener<Gameplay.EventKey.OnGameStop>(OnGameplayComplete);
            EventDispatcher.Instance.RegisterListener<Gameplay.EventKey.UnlockLocalData>(OnUnlockItem);
            EventDispatcher.Instance.RegisterListener<EventKeyBase.OnWatchAds>(OnWatchAds);
        }

        private IEnumerator Play()
        {
            yield return new WaitForSeconds(1);
            systemUI.UpdateCoin();
        }

        protected override void OnDestroy()
        {
            playerMe.LastOpenTime = DateTime.Now;
            playerMe.Save();

            EventDispatcher.Instance.RemoveListener<EventKeyBase.ChangeScene>(OnChangeScene);
            EventDispatcher.Instance.RemoveListener<Gameplay.EventKey.OnGameStop>(OnGameplayComplete);
            EventDispatcher.Instance.RemoveListener<Gameplay.EventKey.UnlockLocalData>(OnUnlockItem);
            EventDispatcher.Instance.RemoveListener<EventKeyBase.OnWatchAds>(OnWatchAds);
        }

        private void OnWatchAds(EventKeyBase.OnWatchAds data)
        {
            purchaseActions.Pop()?.Invoke();
        }

        private void OnUnlockItem(Gameplay.EventKey.UnlockLocalData data)
        {
            if (data.purchaseType == PurchaseType.Ads)
            {
                purchaseData = data;
                Holder.PlayAdvertisement?.Invoke();

                if (purchaseActions == null) purchaseActions = new Stack<Action>(1);
                purchaseActions.Push(OnPurchasingScrollItem);
            }
            else if (data.purchaseType == PurchaseType.Coin)
            {
                if (data.amount <= playerMe.totalCoin)
                {
                    purchaseData = data;
                    playerMe.totalCoin -= data.amount;
                    playerMe.Save();
                    systemUI.UpdateCoin();
                    OnPurchasingScrollItem();
                }
            }
        }
        private void OnPurchasingScrollItem()
        {
            var data = purchaseData;
            if (data.isFruit)
            {
                localData.createEnergyData.UnlockFruit(data.id);
                EventDispatcher.Instance.Dispatch(new EventKeyBase.Purchase
                {
                    id = data.id,
                    fruit = data.obj.GetComponent<Gameplay.CreateEnergyMode.FruitScrollItem>()
                });
            }
            else if (data.isStraw)
            {
                localData.createEnergyData.UnlockStraw(data.id);
                EventDispatcher.Instance.Dispatch(new EventKeyBase.Purchase
                {
                    id = data.id,
                    straw = data.obj.GetComponent<Gameplay.CreateEnergyMode.StrawScrollItem>()
                });
            }
            else if (data.isToy)
            {
                localData.furnitureData.UnlockToy(data.id);
                EventDispatcher.Instance.Dispatch(new EventKeyBase.Purchase
                {
                    id = data.id,
                    toy = data.obj.GetComponent<Gameplay.FurnitureMode.ToyScrollItem>()
                });
            }
            else if (data.isChair)
            {
                localData.furnitureData.UnlockChair(data.id);
                EventDispatcher.Instance.Dispatch(new EventKeyBase.Purchase
                {
                    id = data.id,
                    chair = data.obj.GetComponent<Gameplay.FurnitureMode.ChairScrollItem>()
                });
            }
        }

        private void OnClickBackBtn()
        {

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
