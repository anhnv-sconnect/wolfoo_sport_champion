using AnhNV.Helper;
using SCN;
using SCN.HUD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Helper;
using WFSport.UI;
using static AnhNV.GameBase.PopupManager;
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
        private DataManager dataManager;
        private Minigame curMinigame;
        private PlayerMe playerMe;
        private MinigameSystemUI systemUI;
        private GameplayConfigData[] gameplayData;
        private LocalDataManager localData;
        private Panel curDialog;

        public Stack<Action> purchaseActions;
        private Gameplay.EventKey.UnlockLocalData purchaseData;

        public LocalDataCreateEnergy CreateEnergyData { get => localData.createEnergyData; }
        public LocalDataFurniture FurnitureData { get => localData.furnitureData; }
        public TutorialLocalData TutorialData { get => localData.tutorialData; }
        public PlayerMe PlayerMe { get => playerMe; }
        public bool IsLoadLocalDataCompleted => localData.IsLoadCompleted;
        public bool IsLoadSceneCompleted => localData.IsLoadCompleted && loadSceneManager.IsLoadCompleted;

        public MinigameSystemUI SystemUI { get => systemUI; }

        private void Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 60;

            loadSceneManager = GetComponentInChildren<LoadSceneManager>();
            dataManager = GetComponentInChildren<DataManager>();

            playerMe = dataManager.localSaveloadData.playerMe;
            gameplayData = dataManager.configDataManager.GameplayConfig;
            localData = dataManager.localSaveloadData;
            localData.Load();
            GetSystemUI();

            EventDispatcher.Instance.RegisterListener<EventKeyBase.ChangeScene>(OnChangeScene);
            EventDispatcher.Instance.RegisterListener<Gameplay.EventKey.OnGameStop>(OnGameplayComplete);
            EventDispatcher.Instance.RegisterListener<Gameplay.EventKey.UnlockLocalData>(OnUnlockItem);
            EventDispatcher.Instance.RegisterListener<EventKeyBase.OnWatchAds>(OnWatchAds);
            EventDispatcher.Instance.RegisterListener<EventKeyBase.OpenDialog>(OnOpenDialog);
            EventDispatcher.Instance.RegisterListener<EventKeyBase.OnChoosing>(OnChoosingItem);
        }

        protected override void OnDestroy()
        {
            if (!enabled) return;

            playerMe.LastOpenTime = DateTime.Now;
            playerMe.Save();

            EventDispatcher.Instance.RemoveListener<EventKeyBase.ChangeScene>(OnChangeScene);
            EventDispatcher.Instance.RemoveListener<Gameplay.EventKey.OnGameStop>(OnGameplayComplete);
            EventDispatcher.Instance.RemoveListener<Gameplay.EventKey.UnlockLocalData>(OnUnlockItem);
            EventDispatcher.Instance.RemoveListener<EventKeyBase.OnWatchAds>(OnWatchAds);
            EventDispatcher.Instance.RemoveListener<EventKeyBase.OpenDialog>(OnOpenDialog);
            EventDispatcher.Instance.RemoveListener<EventKeyBase.OnChoosing>(OnChoosingItem);
        }
        internal void UpdatSound(float volume)
        {
            playerMe.soundVolume = volume;
        }
        internal void UpdatMusic(float volume)
        {
            playerMe.musicVolume = volume;
        }
        internal T OrderAsset<T>(Minigame game) where T : Gameplay.IAsset
        {
            return dataManager.OrderAsset<T>(game);
        }

        private void OnChoosingItem(EventKeyBase.OnChoosing obj)
        {
            if(obj.dialogName == DialogName.ChoosingLevel)
            {
                DataTransporter.Level = obj.id;
                GotoGameplayScene(curMinigame);
            }
        }

        private void OnOpenDialog(EventKeyBase.OpenDialog obj)
        {
            OnOpenDialog(obj.dialog);
        }

        private void OnOpenDialog(DialogName dialogName)
        {
            switch (dialogName)
            {
                case DialogName.Pause:
                    curDialog = HUDSystem.Instance.Show<DialogPause>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                    break;
                case DialogName.Setting:
                    curDialog = HUDSystem.Instance.Show<DialogSetting>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                    break;
                case DialogName.ChoosingLevel:
                    curDialog = HUDSystem.Instance.Show<DialogChoosingLevel>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                    break;
                case DialogName.Endgame:
                    curDialog = HUDSystem.Instance.Show<DialogEndgame>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                    break;
                case DialogName.Losegame:
                    curDialog = HUDSystem.Instance.Show<DialogLosingGame>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                    break;
                default:
                    curDialog = null;
                    break;
            }

            if(curDialog != null)
            {
                curDialog.OnHide = OnDialogHiding;
            }
        }

        private void OnDialogHiding()
        {
            if(curDialog as DialogEndgame)
            {
                GotoHomeScene();
            }
            else if (curDialog as DialogLosingGame)
            {
                GotoHomeScene();
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
                else
                {
                    systemUI.PlayAnimOutOfCoin();
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

        private void GotoHomeScene(bool isUsingLoading = true)
        {
            HUDSystem.Instance.HideAll();

            Debug.Log("GotoHomeScene ");
            StopCoroutine("DelayToGoHome");
            loadSceneManager.LoadScene(Constant.SCENE.HOME, isUsingLoading);
            loadSceneManager.OnLoadComplete = () =>
            {
                GetSystemUI();
                if (playerMe.totalEnergy <= 0)
                {
                    systemUI.PlayAnimOutOfEnergy();
                }
            };
        }
        private void GotoGameplayScene(Minigame minigame)
        {
            HUDSystem.Instance.HideAll();

            Debug.Log("GotoGameplayScene ");
            StopCoroutine("DelayToGoHome");
            loadSceneManager.LoadScene(Constant.SCENE.GAMEPLAY);
            loadSceneManager.OnLoadComplete = () =>
            {
                OnGotoGameplay(minigame);
                GetSystemUI();

                switch (minigame)
                {
                    case Minigame.CreateEnergy:
                    case Minigame.Furniture:
                        systemUI.Setup(true, false);
                        break;
                }
            };
        }
        private void GotoLoadScene()
        {
            HUDSystem.Instance.HideAll();

            Debug.Log("GotoGameplayScene ");
            StopCoroutine("DelayToGoHome");
            loadSceneManager.LoadScene(Constant.SCENE.LOADING);
            loadSceneManager.OnLoadComplete = () =>
            {
                GetSystemUI();
            };
        }
        private IEnumerator DelayToGoHome(float time)
        {
            yield return new WaitForSeconds(time);
            GotoHomeScene();
        }

        private void OnGameplayComplete(Gameplay.EventKey.OnGameStop obj)
        {
            if (curMinigame == Minigame.CreateEnergy || curMinigame == Minigame.Furniture)
            {
                //  GotoHomeScene();
                if (obj.data.gamestate == Gameplay.IMinigame.MatchResult.Win)
                {
                    StartCoroutine("DelayToGoHome", 2);
                }
            }
            else
            {
                if (obj.data.gamestate == Gameplay.IMinigame.MatchResult.Win)
                {
                    StartCoroutine("OnPlayerWining", obj.data.claimedCoin);
                }
                else
                {
                    StartCoroutine("OnPlayerLosing");
                }
            }
        }
        private IEnumerator OnPlayerWining(int claimedCoin)
        {
            var totalCoin = playerMe.totalCoin;
            yield return new WaitForSeconds(1);

            OnOpenDialog(DialogName.Endgame);

            yield return new WaitForSeconds(0.5f);

            systemUI.PlayAnimEarningCoin(Vector3.zero, () =>
            {
                playerMe.totalCoin++;
                systemUI.UpdateCoin(true);
            },
            () =>
            {
                playerMe.totalCoin = totalCoin + claimedCoin;
                systemUI.UpdateCoin(true);
            });

            StartCoroutine("DelayToGoHome", 3);
        }
        private IEnumerator OnPlayerLosing()
        {
            yield return new WaitForSeconds(1);
            OnOpenDialog(DialogName.Losegame);
            StartCoroutine("DelayToGoHome", 3);
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
                curMinigame = obj.minigame;
                if(!obj.isMainMode)
                {
                    GotoGameplayScene(obj.minigame);
                }
                else
                {
                    if (playerMe.totalEnergy > 0)
                    {
                        if (obj.minigame == Minigame.BasketBall)
                        {
                            OnOpenDialog(DialogName.ChoosingLevel);
                            return;
                        }

                        playerMe.totalEnergy -= 1;
                        playerMe.Save();

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
            Debug.Log("Get System UI " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
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
            var data = dataManager.OrderMinigame(gameplayConfig.Mode);
            if (data != null)
            {
                Instantiate(data);
            }
        }

        public T OrderPanel<T>() where T : class
        {
            return null;
        }
    }
}
