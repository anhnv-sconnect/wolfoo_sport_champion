using AnhNV.GameBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Gameplay.BasketballMode;

namespace WFSport.Base
{
    [CreateAssetMenu(fileName = "LocalSaveLoad CreateEnergy", menuName = "Core Data/LocalSaveLoad CreateEnergy", order = 1)]
    public class LocalDataCreateEnergy : ScriptableObject, ILocalSaveLoad<LocalDataCreateEnergy>
    {
        public static readonly string KEY = "LocalData_CreateEnergy";
        private bool HasKey { get => PlayerPrefs.HasKey(KEY); }

        public LocalDataRecord[] fruitUnlocked;
        public LocalDataRecord[] strawUnlocked;

        [NaughtyAttributes.Button]
        public LocalDataCreateEnergy Load()
        {
            if (HasKey)
            {
                var jsonData = PlayerPrefs.GetString(KEY);
                Debug.Log($"LocalDataCreateEnergy Local Load: {jsonData} \n =====> Loading Completed <=====");
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
            Debug.Log($"LocalDataCreateEnergy Local Save: {jsonData} \n =====> Saving Completed <=====");
            PlayerPrefs.SetString(KEY, jsonData);
        }

        internal void UnlockFruit(int idx)
        {
            fruitUnlocked[idx].IsUnlock = true;
            Save();
        }
        internal void UnlockAllFruit()
        {
            for (int i = 0; i < fruitUnlocked.Length; i++)
            {
                fruitUnlocked[i].IsUnlock = true;
            }
            Save();
        }
        internal void UnlockStraw(int idx)
        {
            strawUnlocked[idx].IsUnlock = true;
            Save();
        }
        internal void UnlockAllStraw()
        {
            for (int i = 0; i < strawUnlocked.Length; i++)
            {
                strawUnlocked[i].IsUnlock = true;
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
