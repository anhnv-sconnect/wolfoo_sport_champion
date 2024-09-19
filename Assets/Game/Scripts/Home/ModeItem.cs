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
        [SerializeField] private Image icon;
        [SerializeField] private Button btn;

        public int Id { get; private set; }

        private void Start()
        {
            btn.onClick.AddListener(OnClickMe);
        }
        internal void Setup(int id, Sprite sprite)
        {
            Id = id;
            icon.sprite = sprite;
            icon.SetNativeSize();
        }

        private void OnClickMe()
        {
            EventDispatcher.Instance.Dispatch(new EventKeyBase.ChangeScene { gameplay = true });
        }
    }
}
