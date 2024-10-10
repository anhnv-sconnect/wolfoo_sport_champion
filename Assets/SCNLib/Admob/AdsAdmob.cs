using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using DG.Tweening;
namespace SCN.Ads
{
    public class AdsAdmob : MonoBehaviour
    {
        private BannerView bannerView;

        private InterstitialAd inter;

        private RewardedAd rewardVideo;
        private RewardedInterstitialAd rewardInter;

        private Queue<Action> safeCallback = new Queue<Action>();



        private Action onRewardSuccess;
        private Action onRewardInterSuccess;
        private Action onRewardInterFailed;
        private Action onRewardClose;

        private Action onInterSuccess;

        private bool isRequestingBanner;

        private bool isRequestingInter;

        private bool isRequestingRewardVideo;
        private bool isRequestingRewardInter;

        private bool waitToShowBanner;

        private bool isBannerShowing;
        private bool isRewardInterReady;

        private readonly int[] retryTimes = new int[14]
        {
            0,
            2,
            5,
            10,
            20,
            60,
            60,
            120,
            120,
            240,
            240,
            400,
            400,
            600
        };

        protected int retryRewardItem = 0;

        protected int retryInters = 0;

        protected int retryBanner = 0;
        protected int retryRewardInter = 0;

        private IEnumerator ieWaitInternet;

        public static AdsAdmob Instance
        {
            get;
            private set;
        }

        private bool IsInternetAvailable => (int)Application.internetReachability > 0;

        public float BannerHeight => (bannerView != null) ? bannerView.GetHeightInPixels() : 0f;

        public bool HasInter
        {
            get
            {
                if (AdmobConfig.Instance.IsBlockRequest) return true;
                if (inter != null && inter.CanShowAd())
                {
                    return true;
                }
                RequestInterstitial();
                return false;
            }
        }

        public bool HasRewardVideo
        {
            get
            {
                if (AdmobConfig.Instance.IsBlockRequest) return true;
                if (rewardVideo != null && rewardVideo.CanShowAd())
                {
                    return true;
                }
                RequestRewardVideo();
                return false;
            }
        }
        public event Action OnRewardAdLoaded;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                LogError("", "Initialize multiple times!");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            AdmobConfig instance = AdmobConfig.Instance;
            List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };
            deviceIds.Add("2260541b-7e6b-469b-80fc-0352a2f41290");
            deviceIds.Add("27790ac2-0d7d-4ec3-a348-3ddaac5d1763");
            deviceIds.Add("bff9b024-7aa6-4e33-a06b-10ca1ad1abe1");
            deviceIds.Add("1a9c8aa8-d951-4475-a40e-d676b96bc99c");
            deviceIds.Add("b8669861-8a31-4a86-a246-a5b250561b0d");
            deviceIds.Add("38fa9b21-ac94-4451-a2c8-6d7651bba1b2");
            deviceIds.Add("7a640903-6fa8-4efa-8151-5e678c066fbb");
            deviceIds.Add("03aa4d67-b106-4341-a121-8b5c25cb6a10");
            deviceIds.Add("a034597d-3d59-4db5-ad90-ade5e365d8e1");
            deviceIds.Add("d2a7c3a5-d285-4541-86c4-3f36fcf14ddb");
            deviceIds.Add("cf24748a-cc6c-4fa8-96f3-2ffa11aa832c");
            deviceIds.Add("f2585739-83b4-40c6-aa43-752b5deca011");
            deviceIds.Add("d7454b2a-d4ef-45c4-9bba-8d37741c13bc");
            deviceIds.Add("11e855c9-5053-4830-aaed-4e3e80c6f0e1");
            deviceIds.Add("ac4266c1-8f4e-4e83-8a09-b72c540dd58e");
            deviceIds.Add("19e69077-d71b-48f1-8c52-415fe9b20778");
            deviceIds.Add("cea7a4d6-7296-4854-a0ec-86d49d4d03ae");
            deviceIds.Add("bbf3181f-6f5c-464e-8444-73b2b7eafb6c");
            
            RequestConfiguration requestConfiguration =
              new RequestConfiguration();
            requestConfiguration.TagForChildDirectedTreatment = TagForChildDirectedTreatment.True;
            requestConfiguration.TestDeviceIds = (deviceIds);

            if (instance.UseRewardAd && string.IsNullOrEmpty(instance.RewardID))
            {
                LogError("RewardedVideo", "slot id was not config!");
            }
            if (instance.UseInterstitialAd && string.IsNullOrEmpty(instance.InterID))
            {
                LogError("Interstitial", "slot id was not config!");
            }
            if (instance.UseBannerAd && string.IsNullOrEmpty(instance.BannerID))
            {
                LogError("Banner", "slot id was not config!");
            }
            Debug.Log((object)"[Ads.Admob] SDK Initializing");
            Log("", $"Start Initializing: appID = {instance.AppID}, useBanner={instance.UseBannerAd}, useInterstitial={instance.UseInterstitialAd}, useRewardVideo={instance.UseRewardAd}");
            MobileAds.SetRequestConfiguration(requestConfiguration);
            MobileAds.Initialize((Action<InitializationStatus>)delegate (InitializationStatus status)
            {
                //IL_0022: Unknown result type (might be due to invalid IL or missing references)
                //IL_0028: Invalid comparison between Unknown and I4
                Dictionary<string, AdapterStatus> adapterStatusMap = status.getAdapterStatusMap();
                foreach (KeyValuePair<string, AdapterStatus> item in adapterStatusMap)
                {
                    if ((int)item.Value.InitializationState == 1)
                    {
                        Debug.Log((object)("[Ads.Admob] Adapter: " + item.Key + " initialized."));
                    }
                    else
                    {
                        Debug.LogError((object)("[Ads.Admob] Adapter: " + item.Key + " not ready."));
                    }
                }
                if (AdmobConfig.Instance.AutoShowBanner)
                {
                    RequestBanner();
                }
                RequestRewardVideo();
                RequestInterstitial();
            });
            MobileAds.SetiOSAppPauseOnBackground(true);
        }
        private void Update()
        {
            while (safeCallback.Count > 0)
            {
                Action action = null;
                lock (safeCallback)
                {
                    action = safeCallback.Dequeue();
                }
                action?.Invoke();
            }
        }

        private void SafeCallback(Action callback)
        {
            if (callback != null)
            {
                safeCallback.Enqueue(callback);
            }
        }

        private void DelayCallback(float delayTime, Action callback)
        {
            if (callback != null)
            {
                if (delayTime == 0f)
                {
                    SafeCallback(callback);
                }
                else
                {
                    ((MonoBehaviour)this).StartCoroutine(IEDelayCallback(delayTime, callback));
                }
            }
        }

        private IEnumerator IEDelayCallback(float delayTime, Action callback)
        {
            yield return (object)new WaitForSecondsRealtime(delayTime);
            callback?.Invoke();
        }

        private void WaitInternet(Action callback)
        {
            if (callback != null)
            {
                ((MonoBehaviour)this).StartCoroutine(IEWaitInternet(callback));
            }
        }

        private IEnumerator IEWaitInternet(Action callback)
        {
            if (ieWaitInternet == null)
            {
                ieWaitInternet = (IEnumerator)new WaitUntil((Func<bool>)(() => IsInternetAvailable));
            }
            yield return ieWaitInternet;
            callback?.Invoke();
        }

        private AdRequest CreateAdRequest()
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            return new AdRequest();
        }

        private void Log(string adType, string msg)
        {
            if (AdmobConfig.Instance.EnableLog)
            {
                Debug.Log((object)("[Ads.Admob." + adType + "] " + msg));
            }
        }

        private void LogError(string adType, string msg)
        {
            Debug.LogError((object)("[Ads.Admob." + adType + "] " + msg));
        }

        private void RequestBanner()
        {
            if (AdmobConfig.Instance.IsBlockRequest) return;
            if (!AdmobConfig.Instance.UseBannerAd || isRequestingBanner)
            {
                return;
            }
            if (retryBanner >= retryTimes.Length)
            {
                retryBanner = retryTimes[retryTimes.Length - 1];
            }
            int num = retryTimes[retryBanner];
            isRequestingBanner = true;
            Log("Banner", $"Will Request after {num}s, retry={retryBanner}");
            DelayCallback(num, delegate
            {
                if (IsInternetAvailable)
                {
                    DoRequestBanner();
                }
                else
                {
                    LogError("Banner", "Request: Waiting for internet...");
                    WaitInternet(DoRequestBanner);
                }
            });
        }

        private void DoRequestBanner()
        {

            //IL_003a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0044: Expected O, but got Unknown
            Log("Banner", "Request starting...");
            DestroyBanner();
            bannerView = new BannerView(AdmobConfig.Instance.BannerID, GetAdsize(), (AdPosition)(AdmobConfig.Instance.ShowBannerOnBottom ? 1 : 0));
            bannerView.OnBannerAdLoaded += () => OnBannerAdLoaded();
            bannerView.OnBannerAdLoadFailed += (LoadAdError error) => OnBannerAdFailedToLoad(error);
            bannerView.OnAdImpressionRecorded += () => OnBannerAdOpened();

            bannerView.LoadAd(CreateAdRequest());
        }

        private AdSize GetAdsize()
        {
            var configBanner = AdmobConfig.Instance.BannerSize;
            switch (configBanner)
            {
                case BannerSize.SmartBanner:
                    return AdSize.SmartBanner;
                case BannerSize.Banner_320x50:
                    return AdSize.Banner;
                case BannerSize.IABBanner_468x60:
                    return AdSize.IABBanner;
                case BannerSize.Leaderboard_728x90:
                    return AdSize.Leaderboard;
                default: 
                    return AdSize.SmartBanner;
            }
        }

        public void ShowBanner()
        {
            if (!isBannerShowing)
            {
                if (bannerView != null)
                {
                    waitToShowBanner = false;
                    isBannerShowing = true;
                    bannerView.Show();
                    Log("Banner", "Show Start.");
                }
                else
                {
                    waitToShowBanner = true;
                    RequestBanner();
                }
            }
        }

        public void HideBanner()
        {
            if (bannerView != null)
            {
                waitToShowBanner = false;
                isBannerShowing = false;
                bannerView.Hide();
            }
        }

        public void DestroyBanner()
        {
            if (bannerView != null)
            {
                waitToShowBanner = false;
                isBannerShowing = false;
                bannerView.Destroy();
                bannerView = null;
            }
        }

        private void RequestInterstitial()
        {
            if (AdmobConfig.Instance.IsBlockRequest) return;
            if (!AdmobConfig.Instance.UseInterstitialAd || isRequestingInter)
            {
                return;
            }
            if (retryInters >= retryTimes.Length)
            {
                retryInters = retryTimes[retryTimes.Length - 1];
            }
            int num = retryTimes[retryInters];
            isRequestingInter = true;
            Log("Interstitial", $"Will Request after {num}s, retry={retryInters}");
            DelayCallback(num, delegate
            {
                if (IsInternetAvailable)
                {
                    DoRequestInterstitial();
                }
                else
                {
                    LogError("Interstitial", "Request: Waiting for internet...");
                    WaitInternet(DoRequestInterstitial);
                }
            });
        }


        private void DoRequestInterstitial()
        {
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            //IL_002e: Expected O, but got Unknown
            Log("Interstitial", "Request starting...");
            //DestroyInter();
            InterstitialAd.Load(AdmobConfig.Instance.InterID, CreateAdRequest(), (InterstitialAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    OnInterFailedToLoad(loadError);
                    return;
                }
                else if (ad == null)
                {
                    Log("Interstitial", "Interstitial ad failed to load.");
                    return;
                }
                Log("Interstitial", "Interstitial ad loaded.");
                inter = ad;
                OnInterLoaded();
                ad.OnAdFullScreenContentOpened += () =>
                {
                    Log("Interstitial", "Interstitial ad opening.");
                    OnInterstitialAdOpened();
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    Log("Interstitial", "Interstitial ad closed.");
                    OnInterClosed();
                };
                ad.OnAdImpressionRecorded += () =>
                {
                    Log("Interstitial", "Interstitial ad recorded an impression.");
                    OnInterstitialAdImpressionRecorded();
                };
                ad.OnAdClicked += () =>
                {
                    Log("Interstitial", "Interstitial ad recorded a click.");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Log("Interstitial", "Interstitial ad failed to show with error: " +
                                error.GetMessage());
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "Interstitial ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);
                    Log("Interstitial", msg);
                };
            });
        }

        public void ShowInterstitial(Action callback = null)
        {
            if (AdmobConfig.Instance.IsBlockRequest)
            {
                callback?.Invoke();
                return;
            }
            if (HasInter)
            {
                onInterSuccess = callback;
                Log("Interstitial", "Show start..");
                inter.Show();
            }
            else
            {
                Log("Interstitial", "Show failed: ad not ready. Invoke callback.");
                callback?.Invoke();
                RequestInterstitial();
            }
        }
        public void DestroyInter()
        {
            if (inter != null)
            {
                inter.Destroy();
                inter = null;
            }
        }

        #region RewardVideo
        private void RequestRewardVideo()
        {
            if (AdmobConfig.Instance.IsBlockRequest) return;
            if (!AdmobConfig.Instance.UseRewardAd || isRequestingRewardVideo)
            {
                return;
            }
            if (retryRewardItem >= retryTimes.Length)
            {
                retryRewardItem = retryTimes[retryTimes.Length - 1];
            }
            int num = retryTimes[retryRewardItem];
            isRequestingRewardVideo = true;
            Log("RewardedVideo", $"Request after {num}s, retry={retryRewardItem}");
            DelayCallback(num, delegate
            {
                if (IsInternetAvailable)
                {
                    DoRequestReward();
                }
                else
                {
                    LogError("RewardedVideo", "Request: Waiting for internet...");
                    WaitInternet(DoRequestReward);
                }
            });
        }

        private void DoRequestReward()
        {
            //IL_001d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0027: Expected O, but got Unknown
            Log("RewardedVideo", "Request starting...");
            RewardedAd.Load(AdmobConfig.Instance.RewardID, CreateAdRequest(), (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Log("RewardedVideo", "Rewarded ad failed to load with error: " +
                                loadError.GetMessage());
                    OnRewardFailedToLoad(loadError);
                    return;
                }
                else if (ad == null)
                {
                    Log("RewardedVideo", "Rewarded ad failed to load.");
                    return;
                }

                Log("RewardedVideo", "Rewarded ad loaded.");
                OnRewardLoaded();
                rewardVideo = ad;

                ad.OnAdFullScreenContentOpened += () =>
                {

                    Log("RewardedVideo", "Rewarded ad opening.");
                    OnRewardOpening();
                };
                ad.OnAdFullScreenContentClosed += () =>
                {

                    Log("RewardedVideo", "Rewarded ad closed.");
                    OnRewardClosed();
                };
                ad.OnAdImpressionRecorded += () =>
                {

                    Log("RewardedVideo", "Rewarded ad recorded an impression.");
                };
                ad.OnAdClicked += () =>
                {
                    OnRewardClick();
                    Log("RewardedVideo", "Rewarded ad recorded a click.");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    OnRewardFailedToShow(error);
                    Log("RewardedVideo", "Rewarded ad failed to show with error: " +
                               error.GetMessage());
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "Rewarded ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);

                    Log("RewardedVideo", (msg));
                };
            });
        }

        public void ShowRewardVideo(Action onSuccess, Action onClosed = null)
        {
            if (AdmobConfig.Instance.IsBlockRequest)
            {
                onSuccess?.Invoke();
                Debug.Log("Here 1");
                return;
            }
            if (HasRewardVideo)
            {
                onRewardSuccess = onSuccess;
                onRewardClose = onClosed;
                Log("RewardedVideo", "Show start...");
                rewardVideo.Show((Reward reward) => {
                    Log("RewardedVideo", "Rewarded ad granted a reward: " + reward.Amount);
                    OnRewardEarnedReward();

                });
            }
            else
            {
                Log("RewardedVideo", "Show failed: ad not ready. Invoke onClosed callback.");
                onClosed?.Invoke();
                RequestRewardVideo();
            }
        }
        private void OnRewardLoaded()
        {
            Log("RewardedVideo", "OnAdLoaded.");
            retryRewardItem = 0;
            isRequestingRewardVideo = false;
            SafeCallback(this.OnRewardAdLoaded);
        }

        private void OnRewardFailedToLoad(LoadAdError loadError)
        {
            Debug.LogError("RewardedVideo" + "OnAdFailedToLoad: " + loadError.GetMessage());
            isRequestingRewardVideo = false;
            if (AdmobConfig.Instance.RequestOnLoadFailed)
            {
                retryRewardItem++;
                RequestRewardVideo();
            }
        }

        private void OnRewardOpening()
        {
            Log("RewardedVideo", "OnAdOpening...");
            Debug.Log("Here OnRewardOpening");
        }

        private void OnRewardFailedToShow(AdError loadError)
        {
            LogError("RewardedVideo", "OnAdFailedToShow: " + loadError.GetMessage() + ".");
            SafeCallback(onRewardClose);
            RequestRewardVideo();
            Debug.Log("Here OnRewardFailedToShow");
        }

        private void OnRewardClosed()
        {
            Log("RewardedVideo", "OnAdClosed.");
            SafeCallback(onRewardClose);
            RequestRewardVideo();
            Debug.Log("Here OnRewardClosed");
        }

        private void OnRewardEarnedReward()
        {
            Log("RewardedVideo", "OnEarnedReward successfully.");
            SafeCallback(onRewardSuccess);
            Debug.Log("Here OnRewardEarnedReward");
        }
        private void OnRewardClick()
        {

        }      
        #endregion
        #region InterBanner
        private void OnBannerAdLoaded()
        {
            Log("Banner", "OnAdLoaded.");
            retryBanner = 0;
            isRequestingBanner = false;
            if (AdmobConfig.Instance.AutoShowBanner || waitToShowBanner)
            {
                ShowBanner();
            }
        }

        private void OnBannerAdFailedToLoad(LoadAdError error)
        {
            LogError("Banner", "OnAdFailedToLoad: " + error.GetMessage() + ".");
            isRequestingBanner = false;
            retryBanner++;
            RequestBanner();
        }

        private void OnBannerAdOpened()
        {
            Log("Banner", "OnAdOpened...");
        }

        private void OnBannerAdClosed(object sender, EventArgs args)
        {
            Log("Banner", "OnAdClosed.");
            isBannerShowing = false;
            if (AdmobConfig.Instance.AutoShowBanner)
            {
                RequestBanner();
            }
        }

        private void OnBannerAdLeftApplication(object sender, EventArgs args)
        {
            Log("Banner", "OnAdLeftApplication.");
        }

        private void OnInterLoaded()
        {
            Log("Interstitial", "OnAdLoaded.");
            retryInters = 0;
            isRequestingInter = false;
        }


        private void OnInterFailedToLoad(LoadAdError loadError)
        {
            LogError("Interstitial", "OnAdFailedToLoad: " + loadError.GetMessage() + ".");
            isRequestingInter = false;
            if (AdmobConfig.Instance.RequestOnLoadFailed)
            {
                retryInters++;
                RequestInterstitial();
            }
        }

        private void OnInterstitialAdOpened()
        {
            Log("Interstitial", "OnAdOpened...");
        }

        private void OnInterClosed()
        {
            Time.timeScale = 1;
            Log("Interstitial", "OnAdClosed.");
            SafeCallback(onInterSuccess);
            RequestInterstitial();
        }
        private void OnInterstitialAdLeftApplication(object sender, EventArgs args)
        {
            Log("Interstitial", "OnAdLeftApplication.");
        }
        private void OnInterstitialAdImpressionRecorded()
        {
            Time.timeScale = 1;
            Log("Interstitial", "OnAdClosed.");
           // SafeCallback(onInterSuccess);
            RequestInterstitial();
        }
        #endregion
    }
}

