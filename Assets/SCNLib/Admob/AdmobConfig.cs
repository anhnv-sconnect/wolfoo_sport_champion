using UnityEngine;

namespace SCN.Ads
{
	[CreateAssetMenu(fileName = "AdmobConfig", menuName = "Ads/AdmobConfig")]
	public class AdmobConfig : ScriptableObject
	{
		public const string ResourcePath = "AdmobConfig";

		private static AdmobConfig _instance;

		public bool EnableLog = false;

		[Tooltip("if enableTest, you no need to fill any id.")]
		public bool UseTestID = false;

		[Header("Retry to request?")]
		[Tooltip("By default, when the ad request failed, it will retry to request with the increasing delay time. \nIf disable this, it will not retry any one until the method `Show` be called.")]
		public bool RequestOnLoadFailed = true;

		[Header("App id")]
		[SerializeField]
		private string appID_android;

		[SerializeField]
		private string appID_iOS;

		[Header("Banner")]
		[SerializeField]
		private bool useBannerAd = true;

		[SerializeField]
		private string bannerID_android;

		[SerializeField]
		private string bannerID_iOS;

		public BannerSize BannerSize = BannerSize.SmartBanner;

		[SerializeField]
		[Tooltip("Set to false if you want to call method 'ShowBanner' normaly")]
		private bool autoShowBanner = true;

		[SerializeField]
		[Tooltip("Set to false if you want to show banner on top")]
		private bool showBannerOnBottom = true;

		[Header("Interstitial")]
		[SerializeField]
		private bool useInterstitialAd = true;

		[SerializeField]
		private string inter_ID_android;

		[SerializeField]
		private string inter_ID_iOS;

		[Header("RewardVideo")]
		[SerializeField]
		private bool useRewardVideoAd = true;

		[SerializeField]
		private string reward_ID_android;
		[SerializeField]
		private string reward_ID_ios;

		[Header("Rewarded Intertitials")]
		[SerializeField]
		private bool useRewardInterAd = false;

		[SerializeField]
		private string rewardInter_ID_android;
		[SerializeField]
		private string rewardInter_ID_ios;

		[Header("BLOCK REQUEST")]
		[SerializeField] private bool isBlockRequest;

		//[Header("ADS BACKUP")]
		//[Tooltip("When admob has no ad, this backup will be use")]
		//public bool UseAdUnityBackup = true;

		//[SerializeField]
		//private string adUnityAppID_android = "";

		//[SerializeField]
		//private string adUnityAppID_iOS = "";

		public static AdmobConfig Instance
		{
			get
			{
				if ((Object)(object)_instance != (Object)null)
				{
					return _instance;
				}
				_instance = Resources.Load<AdmobConfig>("AdmobConfig");
				if ((Object)(object)_instance == (Object)null)
				{
					Debug.LogError((object)"[Ads] AdmobConfig file not found at Resources/AdmobConfig");
				}
				return _instance;
			}
		}

	//	public string AdUnityAppID => IsAndroid ? adUnityAppID_android : adUnityAppID_iOS;

		private bool IsAndroid => (int)Application.platform == 11 || (int)Application.platform == 7;

		public string AppID_Android => (!UseTestID) ? appID_android : "ca-app-pub-3940256099942544~3347511713";

		public string AppID_iOS => (!UseTestID) ? appID_iOS : "ca-app-pub-3940256099942544~1458002511";

		public string AppID
		{
			get
			{
				if (IsAndroid)
				{
					return AppID_Android;
				}
				return AppID_iOS;
			}
		}

		public bool UseRewardAd => useRewardVideoAd;

		public string RewardID
		{
			get
			{
				if (!useRewardVideoAd)
				{
					return string.Empty;
				}
				if (IsAndroid)
				{
					return (!UseTestID) ? reward_ID_android : "ca-app-pub-3940256099942544/5224354917";
				}
				return (!UseTestID) ? reward_ID_ios : "ca-app-pub-3940256099942544/1712485313";
			}
		}
		public bool UseInterstitialAd => useInterstitialAd;

		public string InterID
		{
			get
			{
				if (!useInterstitialAd)
				{
					return string.Empty;
				}
				if (IsAndroid)
				{
					return (!UseTestID) ? inter_ID_android : "ca-app-pub-3940256099942544/1033173712";
				}
				return (!UseTestID) ? inter_ID_iOS : "ca-app-pub-3940256099942544/4411468910";
			}
		}
	
		public bool UseBannerAd => useBannerAd;

		public string BannerID
		{
			get
			{
				if (!useBannerAd)
				{
					return string.Empty;
				}
				if (IsAndroid)
				{
					return (!UseTestID) ? bannerID_android : "ca-app-pub-3940256099942544/6300978111";
				}
				return (!UseTestID) ? bannerID_iOS : "ca-app-pub-3940256099942544/2934735716";
			}
		}

		public bool ShowBannerOnBottom => showBannerOnBottom;

		public bool AutoShowBanner => autoShowBanner;

        public bool IsBlockRequest { get => isBlockRequest; set => isBlockRequest = value; }
        public bool UseRewardInterAd { get => useRewardInterAd; set => useRewardInterAd = value; }
        public string RewardInterID
		{
			get
			{
				if (!useRewardInterAd)
				{
					return string.Empty;
				}
				if (IsAndroid)
				{
					return (!UseTestID) ? rewardInter_ID_android : "ca-app-pub-3940256099942544/5354046379";
				}
				return (!UseTestID) ? rewardInter_ID_ios : "ca-app-pub-3940256099942544/6978759866";
			}
		}
    }
}