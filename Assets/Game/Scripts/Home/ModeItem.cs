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
        private Minigame holderMinigame;

        private void Start()
        {
            btn.onClick.AddListener(OnClickMe);
        }
        internal void Setup(GameObject obj, Minigame minigame)
        {
            var item = Instantiate(obj, transform);
            item.transform.localPosition = Vector3.zero;
            this.holderMinigame = minigame;
        }

        private void OnClickMe()
        {
            Debug.Log("click Mode Item");
            EventDispatcher.Instance.Dispatch(new EventKeyBase.ChangeScene { gameplay = true, minigame = holderMinigame });
        }
    }
}
