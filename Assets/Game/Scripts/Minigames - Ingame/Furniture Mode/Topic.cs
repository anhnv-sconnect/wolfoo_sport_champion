using DG.Tweening;
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
        [SerializeField] Color activeColor;
        [SerializeField] Color DeactiveColor;

        public Action<Topic> Click;
        private Sequence anim;

        public int Id { get; private set; }
        public Kind Type { get => type; }

        void Start()
        {
            btn.onClick.AddListener(OnClick);
        }
        private void OnDestroy()
        {
            anim?.Kill();
        }

        internal void Setup(int id, bool isClick)
        {
            Id = id;
            if (isClick) Active();
            else Deactive();
        }

        public void Active()
        {
            coverImg.color = activeColor;
            anim?.Complete();
            anim = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one * 1.5f, 0.15f))
                .Append(transform.DOScale(Vector3.one * 1.3f, 0.15f));
        }
        public void Deactive()
        {
            anim?.Kill();
            coverImg.color = DeactiveColor;
            transform.localScale = Vector3.one;
        }

        private void OnClick()
        {
            Click?.Invoke(this);
        }
    }
}
