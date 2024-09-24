using AnhNV.Helper;
using SCN;
using SCN.HUD;
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
        Snowball,
        Furniture
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
        private GameplayConfigData[] gameplayData;
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

            gameplayData = DataManager.Instance.configDataManager.GameplayConfig;

            localData = DataManager.instance.localSaveloadData;
            localData.Load();

            StartCoroutine("Play");

            EventDispatcher.Instance.RegisterListener<EventKeyBase.ChangeScene>(OnChangeScene);
            EventDispatcher.Instance.RegisterListener<Gameplay.EventKey.OnGameStop>(OnGameplayComplete);
            EventDispatcher.Instance.RegisterListener<Gameplay.EventKey.UnlockLocalData>(OnUnlockItem);
            EventDispatcher.Instance.RegisterListener<EventKeyBase.OnWatchAds>(OnWatchAds);
            EventDispatcher.Instance.RegisterListener<EventKeyBase.OpenDialog>(OnOpenDialog);
        }

        private IEnumerator Play()
        {
            yield return new WaitForSeconds(1);
            GetSystemUI();
        }

        protected override void OnDestroy()
        {
            playerMe.LastOpenTime = DateTime.Now;
            playerMe.Save();

            EventDispatcher.Instance.RemoveListener<EventKeyBase.ChangeScene>(OnChangeScene);
            EventDispatcher.Instance.RemoveListener<Gameplay.EventKey.OnGameStop>(OnGameplayComplete);
            EventDispatcher.Instance.RemoveListener<Gameplay.EventKey.UnlockLocalData>(OnUnlockItem);
            EventDispatcher.Instance.RemoveListener<EventKeyBase.OnWatchAds>(OnWatchAds);
            EventDispatcher.Instance.RemoveListener<EventKeyBase.OpenDialog>(OnOpenDialog);
        }

        private void OnOpenDialog(EventKeyBase.OpenDialog obj)
        {
            switch (obj.dialog)
            {
                case AnhNV.GameBase.PopupManager.DialogName.Pause:
                    HUDSystem.Instance.Show<DialogPause>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                    break;
                case AnhNV.GameBase.PopupManager.DialogName.Setting:
                    HUDSystem.Instance.Show<DialogSetting>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                    break;
            }
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
                    systemUI.PlayAnimPurchasingCoin(data.obj.transform, () =>
                    {
                        OnPurchasingScrollItem();
                    });
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
            GotoHomeScene();
        }

        private void GotoHomeScene()
        {
            loadSceneManager.LoadScene(Constant.SCENE.HOME);
            loadSceneManager.OnLoadComplete = () =>
            {
                loadSceneManager.OnLoadSuccess = null;
                GetSystemUI();
                if(playerMe.totalEnergy <= 0)
                {
                    systemUI.PlayAnimOutOfEnergy();
                }
            };
        }
        private void GotoGameplayScene(Minigame minigame)
        {
            loadSceneManager.LoadScene(Constant.SCENE.GAMEPLAY);
            loadSceneManager.OnLoadSuccess = () =>
            {
                Debug.Log("ONLoad Complete");
                OnGotoGameplay(minigame);
                loadSceneManager.OnLoadSuccess = null;
                GetSystemUI();
            };
        }
        private void GotoLoadScene()
        {
            loadSceneManager.LoadScene(Constant.SCENE.LOADING);
            loadSceneManager.OnLoadComplete = () =>
            {
                loadSceneManager.OnLoadSuccess = null;
                GetSystemUI();
            };
        }

        private void OnGameplayComplete(Gameplay.EventKey.OnGameStop obj)
        {
            loadSceneManager.LoadScene(Constant.SCENE.HOME);
        }

        private void OnChangeScene(EventKeyBase.ChangeScene obj)
        {
            if(obj.home)
            {
                GotoHomeScene();
            }
            else if (obj.loading)
            {
                GotoLoadScene();
            }
            else if (obj.gameplay)
            {
                if(!obj.isMainMode)
                {
                    GotoGameplayScene(obj.minigame);
                }
                else
                {
                    if (playerMe.totalEnergy > 0)
                    {
                        if (playerMe.totalEnergy > 0)
                        {
                            playerMe.totalEnergy -= 1;
                            playerMe.Save();
                        }
                        GotoGameplayScene(obj.minigame);
                    }
                }
            }
        }

        internal void UpdateEnergy(bool isInCrease, System.Action OnComplete)
        {
            if(isInCrease)
            {
                var total = playerMe.totalEnergy;
                total++;
                if (total > playerMe.maxEnergy) total = playerMe.maxEnergy;
                playerMe.totalEnergy = total;
                playerMe.Save();

                systemUI.PlayAnimInCreaseEnergy(OnComplete);
            }
            else
            {
                var total = playerMe.totalEnergy;
                total--;
                if (total < 0) total = 0;
                playerMe.totalEnergy = total;
                playerMe.Save();

                systemUI.PlayAnimDecreaseEnergy(OnComplete);
            }
        }

        private void GetSystemUI()
        {
            systemUI = FindAnyObjectByType<MinigameSystemUI>();
            systemUI.ClickBackBtn = OnClickBackBtn;
            systemUI.Setup();
        }

        public IEnumerable<GameplayConfigData> OrderAllMainGameplay()
        {
            foreach (var item in gameplayData)
            {
                if(item.IsMainMode)
                {
                    yield return item;
                }
            }
        }

        private void OnGotoGameplay(Minigame minigame)
        {
            // Create Energy Mode
            GameplayConfigData gameplayConfig = default(GameplayConfigData);
            foreach (var item in gameplayData)
            {
                if (item.Mode == minigame)
                {
                    gameplayConfig = item;
                }
            }
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
