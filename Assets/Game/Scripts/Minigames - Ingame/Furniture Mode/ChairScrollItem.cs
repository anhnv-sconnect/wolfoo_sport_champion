using DG.Tweening;
using SCN;
using SCN.Common;
using SCN.UIExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WFSport.Base;

namespace WFSport.Gameplay.FurnitureMode
{
    public class ChairScrollItem : ScrollItemBase
    {
        [SerializeField] Image icon;
        [SerializeField] Button lockBtn;

        private bool isDragging;
        private Vector3 lastPos;

        internal Action<ChairScrollItem> OnDragInSide;
        private Vector3 comparePos;
        private bool isLocking;
        private Sequence animUnlock;

        public Sprite Icon { get => icon.sprite;}

        protected override void Setup(int order)
        {
            ReturnAfterUnselect = true;
			Master.AddEventTriggerListener(EventTrigger, EventTriggerType.PointerUp, OnPointerUp);
			Master.AddEventTriggerListener(EventTrigger, EventTriggerType.Drag, OnDrag);
        }

        private void OnDestroy()
        {
            animUnlock?.Kill();
        }

        internal void Setup(Sprite sprite, Vector3 comparePos, bool isLock)
        {
            lockBtn.onClick.AddListener(OnClickLockBtn);

            icon.sprite = sprite;
            icon.SetNativeSize();
            this.comparePos = comparePos;
            gameObject.SetActive(true);
            isLocking = isLock;

            if (!isLocking) UnLock(true);
            else Lock();

        }
        private void Lock()
        {
            lockBtn.gameObject.SetActive(true);
            icon.color = Color.black;
            icon.DOFade(0.7f, 0);
        }
        private void UnLock(bool isImmediately = false)
        {
            if (isImmediately)
            {
                lockBtn.gameObject.SetActive(false);
                icon.color = Color.white;
                icon.DOFade(1, 0);
            }
            else
            {
                animUnlock = DOTween.Sequence()
                    .Append(lockBtn.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack))
                    .Join(icon.DOColor(Color.white, 0.5f).SetEase(Ease.Linear))
                    .Join(icon.DOFade(1, 0.5f).SetEase(Ease.Linear));
            }
        }

        private void OnClickLockBtn()
        {
            EventDispatcher.Instance.Dispatch(new EventKey.UnlockLocalData { id = order, isFruit = true });
            UnLock();
        }
        protected override void OnStartDragOut()
        {
            base.OnStartDragOut();
            isDragging = true;
        }

        private void OnDrag(BaseEventData arg0)
        {
            lastPos = transform.position;
        }

        private void OnPointerUp(BaseEventData arg0)
        {
            if (isDragging)
            {
                var distance = Vector2.Distance(lastPos, comparePos);
                if(distance < 2)
                {
                    OnDragInSide?.Invoke(this);
                }
            }
            isDragging = false;
        }
    }
}
