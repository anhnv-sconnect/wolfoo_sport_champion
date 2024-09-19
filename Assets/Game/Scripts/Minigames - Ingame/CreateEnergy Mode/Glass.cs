using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Helper;

namespace WFSport.Gameplay.CreateEnergyMode
{
    public class Glass : MonoBehaviour
    {
        [SerializeField] float waterHeight;
        [SerializeField] SpriteRenderer water;

        public int Idx { get; private set; }

        private float pouringSpeed;
        private Vector3 initLocalPos;
        private Vector3 beginJumpPos;
        private Sequence anim;
        private bool canDrag;

        public Action<Glass> OnEndDrag;
        private Transform initParent;

        public bool IsJumping { get; private set; }
        public bool HasStraw { get; private set; }
        public bool HasWater { get; private set; }

        private void Start()
        {
        }
        private void OnDestroy()
        {
            anim?.Kill();
        }
        private void OnMouseDown()
        {
            if (!canDrag) return;
            transform.position = ScreenHelper.GetMousePos();
        }
        private void OnMouseDrag()
        {
            if (!canDrag) return;
            transform.position = ScreenHelper.GetMousePos();
        }
        private void OnMouseUp()
        {
            if (!canDrag) return;
            OnEndDrag?.Invoke(this);
        }

        internal void Setup(float pouringTime)
        {
            initLocalPos = transform.localPosition;
            initParent = transform.parent;

            var size = water.size;
            size.y = 0;
            water.size = size;
            pouringSpeed = waterHeight / pouringTime;
        }
        internal void SetupDrag(bool value)
        {
            canDrag = value;
        }
        internal void ReleaseWater(Vector3 endPos)
        {
            anim?.Kill();
            anim = DOTween.Sequence()
                .Append(transform.DOMove(endPos, 0.25f))
                .Append(DOVirtual.Float(1, 0, 0.5f, (value) =>
                {
                    water.size =new Vector2(water.size.x, value * waterHeight);
                }).SetEase(Ease.Linear))
                .Join(transform.DOScale(Vector3.one * 0.5f, 0.5f));
            anim.OnComplete(() =>
            {
                JumpBacktoTray(null);
            });
        }

        internal void OnGettingWater()
        {
            if (water.size.y >= waterHeight)
            {
                HasWater = true;
                return;
            }
            water.size += Vector2.up * pouringSpeed * Time.deltaTime;
        }
        internal void StrawJumpIn()
        {
            HasStraw = true;
        }

        internal void JumpBacktoTray(Action OnComplete)
        {
            anim?.Kill();
            IsJumping = true;
            var endPos = initParent.TransformPoint(initLocalPos);
            anim = DOTween.Sequence()
                .Append(transform.DOJump(endPos, 1, 1, 0.5f))
                .Join(transform.DOScale(Vector3.one, 0.5f));
            anim.OnComplete(() =>
            {
                OnComplete?.Invoke();
                IsJumping = false;
            transform.SetParent(initParent);
            });
        }

        internal void JumpOutOfTray(Vector3 endPos, Action OnComplete)
        {
            anim?.Kill();

            IsJumping = true;
            beginJumpPos = transform.position;
            anim = DOTween.Sequence().Append(transform.DOJump(endPos, 1, 1, 0.5f));
            anim.OnComplete(() =>
            {
                IsJumping = false;
                OnComplete?.Invoke();
            });

        }
    }
}
