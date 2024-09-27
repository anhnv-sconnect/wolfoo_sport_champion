using DG.Tweening;
using SCN;
using SCN.Common;
using SCN.UIExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WFSport.Gameplay.FurnitureMode
{
    public abstract class ScrollItem : ScrollItemBase
    {
        protected abstract void OnEndDrag();
        protected abstract void OnClickAdsBtn();
        protected abstract void OnClickCoinBtn(LocalDataRecord localDataRecord);

        [SerializeField] Image icon;
        [SerializeField] Button coinLockBtn;
        [SerializeField] Button adLockBtn;

        private bool isLocking;

        public Topic.Kind TopicKind { get; private set; }
        public Sprite Icon { get => icon.sprite; }

        private bool isDragging;

        private Sequence animUnlock;

        internal Action<ScrollItem> OnDragInSide;
        internal Action<ScrollItem> OnBeginDrag;

        private void OnDestroy()
        {
            animUnlock?.Kill();
        }

        protected override void Setup(int order)
        {
            ReturnAfterUnselect = true;
            Master.AddEventTriggerListener(EventTrigger, EventTriggerType.PointerUp, OnPointerUp);
        }

        internal void Setup(Sprite sprite, LocalDataRecord localRecord, Topic.Kind topicKind)
        {
            coinLockBtn.onClick.AddListener(() =>
            {
                scrollInfinity.StopAutoMove();
                OnClickCoinBtn(localRecord);
            });
            adLockBtn.onClick.AddListener(() =>
            {
                scrollInfinity.StopAutoMove();
                OnClickAdsBtn();
            });

            icon.sprite = sprite;
            //  icon.SetNativeSize();
            gameObject.SetActive(true);
            TopicKind = topicKind;

            if (localRecord == null)
            {
                isLocking = false;
                UnLock(true);
            }
            else
            {
                isLocking = !localRecord.IsUnlock;
                if(isLocking)
                {
                    if (localRecord.Data.PurchaseType == WFSport.Base.PurchaseType.Coin)
                    {
                        var priceTxt = coinLockBtn.GetComponentInChildren<TMP_Text>();
                        priceTxt.text = localRecord.Data.Amount.ToString();
                        CoinLock();
                    }
                    else
                    {
                        AdsLock();
                    }
                }
                else
                {
                    UnLock(true);
                }
            }
        }
        private void CoinLock()
        {
            coinLockBtn.gameObject.SetActive(true);
            adLockBtn.gameObject.SetActive(false);
            Lock();
        }
        private void AdsLock()
        {
            adLockBtn.gameObject.SetActive(true);
            coinLockBtn.gameObject.SetActive(false);
            Lock();
        }
        private void Lock()
        {
            icon.color = Color.black;
            icon.DOFade(0.7f, 0);
        }
        public void UnLock(bool isImmediately = false)
        {
            if (isImmediately)
            {
                adLockBtn.gameObject.SetActive(false);
                coinLockBtn.gameObject.SetActive(false);
                icon.color = Color.white;
                icon.DOFade(1, 0);
            }
            else
            {
                var time = 0.25f;
                animUnlock = DOTween.Sequence()
                    .Append(adLockBtn.transform.DOScale(Vector3.zero, time).SetEase(Ease.InBack))
                    .Join(coinLockBtn.transform.DOScale(Vector3.zero, time).SetEase(Ease.InBack))
                    .Join(icon.DOColor(Color.white, time).SetEase(Ease.Linear))
                    .Join(icon.DOFade(1, time).SetEase(Ease.Linear));
            }
        }

        protected override void OnStartDragOut()
        {
            base.OnStartDragOut();
            isDragging = true;
        }

        private void OnPointerUp(BaseEventData arg0)
        {
            if (isDragging)
            {
                OnEndDrag();
            }
            isDragging = false;
        }
    }
}
