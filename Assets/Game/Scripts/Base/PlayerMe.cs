using AnhNV.GameBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Base
{
    [System.Serializable]
    public class PlayerMe : ILocalSaveLoad<PlayerMe>
    {
        public static readonly string KEY = "PLAYER_NEW_KEY";
        private bool HasKey { get => PlayerPrefs.HasKey(KEY); }
        public bool IsMuteMusic;
        public bool IsMuteSound;
        public string MyVersion;
        public int TotalAdsSuccess;
        public string lastTimeString;

        public DateTime LastOpenTime
        {
            get
            {
                DateTime timeResult;
                var isValid = DateTime.TryParse(lastTimeString, out timeResult);

                if (!isValid) return DateTime.Now;
                else return timeResult;
            }
            set
            {
                lastTimeString = value.ToString();
            }
        }

        public PlayerMe(bool isMuteMusic, bool isMuteSound)
        {
            IsMuteMusic = isMuteMusic;
            IsMuteSound = isMuteSound;
            MyVersion = Application.version;
            LastOpenTime = DateTime.Now;
        }
        public PlayerMe()
        {
        }

        public void Init()
        {
            IsMuteMusic = false;
            IsMuteSound = false;
            MyVersion = Application.version;
            LastOpenTime = DateTime.Now;
        }

        public PlayerMe Load()
        {
            if (HasKey)
            {
                var jsonData = PlayerPrefs.GetString(KEY);
                Debug.Log($"PlayerMe Local Load: {jsonData} \n =====> Loading Completed <=====");
                var data = JsonUtility.FromJson<PlayerMe>(jsonData);

                IsMuteMusic = data.IsMuteMusic;
                IsMuteSound = data.IsMuteSound;
                TotalAdsSuccess = data.TotalAdsSuccess;
                return data;
            }
            else
            {
                return null;
            }
        }

        public void Save()
        {
            var jsonData = JsonUtility.ToJson(this);
            Debug.Log($"PlayerMe Local Save: {jsonData} \n =====> Saving Completed <=====");
            PlayerPrefs.SetString(KEY, jsonData);
        }

        public void Reset()
        {
        }

        public void Read()
        {

        }
    }
}
