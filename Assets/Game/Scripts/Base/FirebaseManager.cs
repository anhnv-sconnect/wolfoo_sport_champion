using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;

namespace _Base
{
    public class FirebaseManager : MonoBehaviour
    {
        private string logString;
        private string curLocation;
        private string curPanel;

        public string CurLocation { get => curLocation; }
        public static FirebaseManager instance;

        private void Awake()
        {
            if (instance == null) instance = this;
        }

        void Start()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
#if !UNITY_EDITOR
                    FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart);
#endif

                    var app = FirebaseApp.DefaultInstance;

                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                }
            //    InitRemoteConfig();
            });

            /// T?t Analystic c?a Unity khi dùng IAP
            Analytics.initializeOnStartup = false;
            Analytics.enabled = false;
            PerformanceReporting.enabled = false;
            Analytics.limitUserTracking = true;
            Analytics.deviceStatsEnabled = false;
        }
        private void OnDestroy()
        {
        }
        public string GetLegitString(string input)
        {
            if (input == null) return "null";

            string word = string.Empty;
            string specialChar = @"|!#$%&/()=?»«@£§€{}.-;~`'<>_, ";
            foreach (var item in input)
            {
                if (specialChar.Contains(item))
                {
                    word += "_";
                }
                else
                {
                    word += item;
                }
            }
            return word;
        }
        public void LogClick(string eventName, string building)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;

            Debug.Log("Firebase Event: " + eventName);
            Parameter[] paramss = new Parameter[]
            {
                new Parameter("building", building),
                new Parameter("location", curPanel),
                new Parameter("name", eventName),
            };

            foreach (var param in paramss) { Debug.Log("Params: " + param); }

#if !UNITY_EDITOR
            FirebaseAnalytics.LogEvent("button_click", paramss);
#endif
        }
        public void LogEndLevel(string building, string location, float duration, float process, EndLevelState levelState)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;
            Debug.Log("Firebase Event: " + "level_end" + "\n" +
                building + " - " +
                location + " - " +
                duration + " - " +
                process + " - " +
                levelState + " - " );

            Parameter[] paramss = new Parameter[]
            {
                new Parameter("building", building),
                new Parameter("location", location),
                new Parameter("duration", duration),
                new Parameter("process", process),
                new Parameter("result", levelState.ToString()),
            };

#if !UNITY_EDITOR
            FirebaseAnalytics.LogEvent("level_end", paramss);
#endif
        }
        public void LogBeginLevel(string building, string location)
        {
            curLocation = location;
            if (Application.internetReachability == NetworkReachability.NotReachable) return;
            Debug.Log("Firebase Event: " + "level_start" + "\n" +
                building + " - " +
                location + " - "
                );

            Parameter[] paramss = new Parameter[2];
            paramss[0] = new Parameter("building", building);
            paramss[1] = new Parameter("location", location);

#if !UNITY_EDITOR
            FirebaseAnalytics.LogEvent("level_start", paramss);
#endif
        }
        public void LogBuySuccessIAP(string id, string price, string currency)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;
            logString = "IAP_success";

            Debug.Log("Firebase Event: " + logString + " With Params: ");
            Parameter[] paramss = new Parameter[]
            {
                new Parameter("package_id", id),
                new Parameter("value", price),
                new Parameter("currency", currency)
            };

            Debug.Log(id + " - " + price + " - " + currency);

#if !UNITY_EDITOR
            FirebaseAnalytics.LogEvent(logString, paramss);
#endif
        }
        public void LogBuyFailIAP(string id, string price, string currency, string reason)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;
            logString = "IAP_Fail";

            reason = GetLegitString(reason);

            Debug.Log("Firebase Event: " + logString + " With Params: ");
            Parameter[] paramss = new Parameter[4];
            paramss[0] = new Parameter("package_id", id);
            paramss[1] = new Parameter("value", price);
            paramss[2] = new Parameter("currency", currency);
            paramss[3] = new Parameter("error_name", reason);

            Debug.Log(id + " - " + price + " - " + currency + " - " + reason);

#if !UNITY_EDITOR
            FirebaseAnalytics.LogEvent(logString, paramss);
#endif
        }
        public void LogClickIAP(string id, string price, string currency)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;
            logString = "IAP_click";

            Debug.Log("Firebase Event: " + logString + " With Params: ");
            Parameter[] paramss = new Parameter[]
            {
                new Parameter("package_id", id),
                new Parameter("value", price),
                new Parameter("currency", currency),
            };

            Debug.Log(id + " - " + price + " - " + currency);

#if !UNITY_EDITOR
            FirebaseAnalytics.LogEvent(logString, paramss);
#endif
        }
        public void AssignPanel(string name_)
        {
            curPanel = name_;
        }
        public void LogWatchAds(string eventName, string building, string minigame, string rewardID)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;

            minigame = GetLegitString(minigame);
            rewardID = GetLegitString(rewardID);
            building = GetLegitString(building);

            Debug.Log("Firebase Event: " + eventName);
            Parameter[] paramss = new Parameter[4];
            paramss[0] = new Parameter("building", building);
            paramss[1] = new Parameter("location", curLocation);
            paramss[2] = new Parameter("minigame", minigame);
            paramss[3] = new Parameter("reward", rewardID);

            Debug.Log("Params: " + eventName);
            Debug.Log("Params: " + building);
            Debug.Log("Params: " + minigame);
            Debug.Log("Params: " + rewardID);
            Debug.Log("Params: " + curLocation);

#if !UNITY_EDITOR
            FirebaseAnalytics.LogEvent(eventName, paramss);
#endif
        }
        public void LogWatchAds(string eventName, string building)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;

            Debug.Log("Firebase Event: " + eventName);
            Parameter[] paramss = new Parameter[2];
            paramss[0] = new Parameter("building", building);
            paramss[1] = new Parameter("location", curLocation);

            foreach (var param in paramss) { Debug.Log("Params: " + param); }

#if !UNITY_EDITOR
            FirebaseAnalytics.LogEvent(eventName, paramss);
#endif
        }
        public void LogWatchAds(string eventName, string building, string failreason)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;

            Debug.Log("Firebase Event: " + eventName + 
                " - " + building + 
                " - " + failreason
                );
            Parameter[] paramss = new Parameter[]
            {
                new Parameter("building", building),
                new Parameter("location", curLocation),
            };

#if !UNITY_EDITOR
            FirebaseAnalytics.LogEvent(eventName, paramss);
#endif
        }
        public void LogWatchAds(AdsLogType logType)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;

            Debug.Log("Firebase Event: " + logType.ToString());
#if !UNITY_EDITOR
            FirebaseAnalytics.LogEvent(logType.ToString());
#endif
        }
    }
    public enum EndLevelState
    {
        Win,
        Back,
        Quit,
        Retry,
    }
    public enum AdsLogType
    {
        ad_rv_click,
        ad_rv_request,
        ad_rv_impress,
        ad_rv_skip,
        ad_rv_success,
        ad_rv_failed,
        ad_inter_request,
        ad_inter_impress,
        ad_inter_success,
        ad_inter_fail
    }
    public enum AdsType
    {
        Inters,
        Reward,
        Banner
    }
}