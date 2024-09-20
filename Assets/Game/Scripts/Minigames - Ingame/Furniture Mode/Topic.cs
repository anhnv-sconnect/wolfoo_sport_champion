using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport.Gameplay.FurnitureMode
{
    public class Topic : MonoBehaviour
    {
        [SerializeField] Button btn;
        [SerializeField] Image icon;
        [SerializeField] Image coverImg;

        public Action<Topic> Click;

        void Start()
        {
            btn.onClick.AddListener(OnClick);
            Click += GetCLick;
        }
        private void OnDestroy()
        {
            Click -= GetCLick;
        }

        private void GetCLick(Topic obj)
        {
            if(obj == this)
            {
                Active();
            }
            else
            {
                Deactive();
            }
        }

        public void Active()
        {

        }
        public void Deactive()
        {

        }

        private void OnClick()
        {
            Click?.Invoke(this);
        }
    }
}
