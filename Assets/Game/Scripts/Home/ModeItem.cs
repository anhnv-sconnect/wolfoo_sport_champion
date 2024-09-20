using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WFSport.Base;

namespace WFSport.Home
{
    public class ModeItem : MonoBehaviour
    {
        [SerializeField] private Button btn;
        private ConfigDataManager.GameplayConfigData gameplayConfigData;

        public int Id { get; private set; }

        private void Start()
        {
            btn.onClick.AddListener(OnClickMe);
        }
        internal void Setup(int id, GameObject obj, ConfigDataManager.GameplayConfigData data)
        {
            Id = id;
            var item = Instantiate(obj, transform);
            item.transform.localPosition = Vector3.zero;
            gameplayConfigData = data;
        }

        private void OnClickMe()
        {
            Debug.Log("click Mode Item");
            EventDispatcher.Instance.Dispatch(new EventKeyBase.ChangeScene { gameplay = true, gameplayConfig = gameplayConfigData });
        }
    }
}
