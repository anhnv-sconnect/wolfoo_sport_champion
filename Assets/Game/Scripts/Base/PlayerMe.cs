using AnhNV.GameBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Base
{
    [CreateAssetMenu(fileName = "PlayerMe", menuName = "Core Data/PlayerMe", order = 1)]
    public class PlayerMe : ScriptableObject, ILocalSaveLoad<PlayerMe>
    {
        public static readonly string KEY = "PLAYER_KEY";
        private bool HasKey { get => PlayerPrefs.HasKey(KEY); }
        public bool IsMuteMusic;
        public bool IsMuteSound;
        public int totalCoin;
        public int totalEnergy;

        private string myVersion;
        private int totalAdsSuccess;
        private string lastTimeString;

        private bool isInit;

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
            myVersion = Application.version;
            LastOpenTime = DateTime.Now;
        }

        public void Init()
        {
            isInit = true;
            myVersion = Application.version;
            LastOpenTime = DateTime.Now;
        }

        public PlayerMe Load()
        {
            if (HasKey)
            {
                var jsonData = PlayerPrefs.GetString(KEY);
                Debug.Log($"PlayerMe Local Load: {jsonData} \n =====> Loading Completed <=====");
                JsonUtility.FromJsonOverwrite(jsonData, this);
                if (isInit) Init();

                return this;
            }
            else
            {
                return null;
            }
        }

        [NaughtyAttributes.Button]
        public void Save()
        {
            var jsonData = JsonUtility.ToJson(this);
            Debug.Log($"PlayerMe Local Save: {jsonData} \n =====> Saving Completed <=====");
            PlayerPrefs.SetString(KEY, jsonData);
        }

        public void Read()
        {

        }
        [NaughtyAttributes.Button]
        public void Remove()
        {
            PlayerPrefs.DeleteKey(KEY);
            Debug.Log($"Deleting PlayerMe local.... \n =====> DELETE Completed <=====");
        }
    }
}
