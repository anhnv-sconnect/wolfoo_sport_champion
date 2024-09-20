using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport.Gameplay.FurnitureMode
{
    public class Topic : MonoBehaviour
    {
        public enum Kind
        {
            None,
            Toy,
            Other,
            Chair
        }
        [SerializeField] Kind type;
        [SerializeField] Button btn;
        [SerializeField] Image icon;
        [SerializeField] Image coverImg;

        public Action<Topic> Click;

        public int Id { get; private set; }
        public Kind Type { get => type; }

        void Start()
        {
            btn.onClick.AddListener(OnClick);
            Click += GetCLick;
        }
        private void OnDestroy()
        {
            Click -= GetCLick;
        }

        internal void Setup(int id, bool isClick)
        {
            Id = id;
            if (isClick) Active();
            else Deactive();
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
