using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhNV.GameBase
{
    [System.Serializable]
    public class SaveLoadDataTutorial : ILocalSaveLoad<SaveLoadDataTutorial>
    {
        private string KEY = "TUTORIAL_KEY";
        private bool HasKey => PlayerPrefs.HasKey(KEY);
        public bool HasTutorial1 { get => ht1; private set => ht1 = value; }

        private bool ht1;

        public void SetCompleteTutorial1()
        {
            HasTutorial1 = false;
        }

        public void Reset()
        {
            Init();
        }

        public SaveLoadDataTutorial Load()
        {
            if (HasKey)
            {
                var jsonData = PlayerPrefs.GetString(KEY);
                Debug.Log($"Tutorial Local Load: {jsonData} \n =====> Loading Completed <=====");
                var data = JsonUtility.FromJson<SaveLoadDataTutorial>(jsonData);

                HasTutorial1 = data.HasTutorial1;
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
            Debug.Log($"Tutorial Local Save: {jsonData} \n =====> Saving Completed <=====");
            PlayerPrefs.SetString(KEY, jsonData);
        }

        public void Init()
        {
            HasTutorial1 = true;
            Save();
        }

        public void Read()
        {
        }
    }
}
