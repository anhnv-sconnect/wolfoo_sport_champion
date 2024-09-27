using AnhNV.GameBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Gameplay.FurnitureMode;

namespace WFSport.Base
{
    [CreateAssetMenu(fileName = "LocalSaveLoad Furniture", menuName = "Core Data/LocalSaveLoad Furniture", order = 1)]
    public class LocalDataFurniture : ScriptableObject, ILocalSaveLoad<LocalDataFurniture>
    {
        public static readonly string KEY = "LocalData_Furniture";
        private bool HasKey { get => PlayerPrefs.HasKey(KEY); }

        public LocalDataRecord[] toyUnlocked;
        public LocalDataRecord[] otherUnlocked;
        public LocalDataRecord[] chairUnlocked;
        public List<CreatedToyData> ToysCreated = new List<CreatedToyData>();
        public int createdChairId;

        [NaughtyAttributes.Button]
        public LocalDataFurniture Load()
        {
            if (HasKey)
            {
                var jsonData = PlayerPrefs.GetString(KEY);
                Debug.Log($"LocalDataFurniture Local Load: {jsonData} \n =====> Loading Completed <=====");
                JsonUtility.FromJsonOverwrite(jsonData, this);

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
            Debug.Log($"LocalDataFurniture Local Save: {jsonData} \n =====> Saving Completed <=====");
            PlayerPrefs.SetString(KEY, jsonData);
        }

        internal void UnlockToy(int idx)
        {
            toyUnlocked[idx].IsUnlock = true;
            Save();
        }
        internal void UnlockAllToy()
        {
            for (int i = 0; i < toyUnlocked.Length; i++)
            {
                toyUnlocked[i].IsUnlock = true;
            }
            Save();
        }
        internal void UnlockChair(int idx)
        {
            chairUnlocked[idx].IsUnlock = true;
            Save();
        }
        internal void UnlockAllChair()
        {
            for (int i = 0; i < chairUnlocked.Length; i++)
            {
                chairUnlocked[i].IsUnlock = true;
            }
            Save();
        }

        public void Init()
        {
        }

        public void Read()
        {
        }

        public void Reset()
        {
        }
    }
}
