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
using WFSport.Base;

namespace WFSport.Gameplay.CreateEnergyMode
{
    public class FruitScrollItem : ScrollItemBase
    {
        [SerializeField] Image icon;
        [SerializeField] Button coinLockBtn;
        [SerializeField] Button adLockBtn;

        private bool isDragging;
        private Vector3 lastPos;

        internal Action<FruitScrollItem> OnDragInSide;
        private Sequence animUnlock;
        private bool isLocking;
        private Blender blender;

        public Sprite Icon { get => icon.sprite; }

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
        protected void OnClickAdsBtn()
        {
            scrollInfinity.StopAutoMove();
            EventDispatcher.Instance.Dispatch(
                new EventKey.UnlockLocalData
                {
                    id = order,
                    isFruit = true,
                    purchaseType = WFSport.Base.PurchaseType.Ads,
                    obj = gameObject
                });
        }

        protected void OnClickCoinBtn(LocalDataRecord localDataRecord)
        {
            scrollInfinity.StopAutoMove();
            EventDispatcher.Instance.Dispatch(
                new EventKey.UnlockLocalData
                {
                    id = order,
                    isFruit = true,
                    purchaseType = PurchaseType.Coin,
                    amount = localDataRecord.Data.Amount,
                    obj = gameObject,
                });
        }

        internal void Setup(Sprite sprite, Blender blender, LocalDataRecord localRecord)
        {
            coinLockBtn.onClick.AddListener(() => OnClickCoinBtn(localRecord));
            adLockBtn.onClick.AddListener(OnClickAdsBtn);

            icon.sprite = sprite;
            icon.SetNativeSize();
            this.blender = blender;
            gameObject.SetActive(true);

            if (localRecord == null)
            {
                isLocking = false;
                UnLock(true);
            }
            else
            {
                isLocking = !localRecord.IsUnlock;
                if (isLocking)
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
            icon.color = Color.white;
            icon.DOFade(0.8f, 0);
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

        private void OnDrag(BaseEventData arg0)
        {
            lastPos = transform.position;
        }

        private void OnPointerUp(BaseEventData arg0)
        {
            if (isDragging)
            {
                if (blender.CheckInside(transform))
                {
                    OnDragInSide?.Invoke(this);
                }
            }
            isDragging = false;
        }
    }
}
