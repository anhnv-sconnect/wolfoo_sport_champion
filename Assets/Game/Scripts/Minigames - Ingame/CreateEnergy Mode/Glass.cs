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
        private float pouringSpeed;
        private Vector3 beginJumpPos;
        private Sequence anim;
        private bool canDrag;

        public Action<Glass> OnEndDrag;

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
            var size = water.size;
            size.y = 0;
            water.size = size;

            pouringSpeed = waterHeight / pouringTime;
        }
        internal void SetupDrag(bool value)
        {
            canDrag = value;
        }

        internal void OnPouringWater()
        {
            if (water.size.y >= waterHeight) return;
            water.size += Vector2.up * pouringSpeed * Time.deltaTime;
        }

        internal void JumpBacktoTray( Action OnComplete)
        {
            anim?.Kill();
            anim = DOTween.Sequence().Append(transform.DOJump(beginJumpPos, 1, 1, 0.5f));
            anim.OnComplete(() =>
            {
                OnComplete?.Invoke();
            });

        }

        internal void JumpOutOfTray(Vector3 endPos, Action OnComplete)
        {
            anim?.Kill();

            beginJumpPos = transform.position;
            anim = DOTween.Sequence().Append(transform.DOJump(endPos, 1, 1, 0.5f));
            anim.OnComplete(() =>
            {
                OnComplete?.Invoke();
            });

        }
    }
}
