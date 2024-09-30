using AnhNV.GameBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Base
{
    [CreateAssetMenu(fileName = "LocalSaveLoad Tutorial", menuName = "Core Data/LocalSaveLoad Tutorial", order = 1)]
    public class TutorialLocalData : ScriptableObject, ILocalSaveLoad<TutorialLocalData>
    {
        public static readonly string KEY = "LocalData_Tutorial";

        [SerializeField] private bool isArcheryShown;
        [SerializeField] private bool isBasketShown;
        [SerializeField] private bool isCatchMoreToyShown;
        [SerializeField] private bool isCreateEnergyShown;
        [SerializeField] private bool isFurnitureShown;
        [SerializeField] private bool isLatinDanceShown;
        [SerializeField] private bool isRelayShown;
        [SerializeField] private bool isSnowballShown;

        public bool HasKey { get => PlayerPrefs.HasKey(KEY); }
        public bool IsArcheryShown { get => isArcheryShown; set => isArcheryShown = value; }
        public bool IsBasketShown { get => isBasketShown; set => isBasketShown = value; }
        public bool IsCatchMoreToyShown { get => isCatchMoreToyShown; set => isCatchMoreToyShown = value; }
        public bool IsCreateEnergyShown { get => isCreateEnergyShown; set => isCreateEnergyShown = value; }
        public bool IsFurnitureShown { get => isFurnitureShown; set => isFurnitureShown = value; }
        public bool IsLatinDanceShown { get => isLatinDanceShown; set => isLatinDanceShown = value; }
        public bool IsRelayShown { get => isRelayShown; set => isRelayShown = value; }
        public bool IsSnowballShown { get => isSnowballShown; set => isSnowballShown = value; }

        public void Init()
        {
        }

        [NaughtyAttributes.Button]
        public TutorialLocalData Load()
        {
            if(HasKey)
            {
                var record = PlayerPrefs.GetString(KEY);
                Debug.Log($"Tutorial Load: {record} \n =====> Loading Completed <=====");
                JsonUtility.FromJsonOverwrite(record, this);

                return this;
            }
            else
            {
                return null;
            }
        }

        public void Read()
        {

        }

        [NaughtyAttributes.Button]
        public void Save()
        {
            var jsonData = JsonUtility.ToJson(this);
            Debug.Log($"Tutorial Save: {jsonData} \n =====> Saving Completed <=====");
            PlayerPrefs.SetString(KEY, jsonData);
        }
    }
}
