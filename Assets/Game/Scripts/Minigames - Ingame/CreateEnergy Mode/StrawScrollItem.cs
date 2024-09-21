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

namespace WFSport.Gameplay.CreateEnergyMode
{
    public class StrawScrollItem : ScrollItemBase
    {
        [SerializeField] Image icon;
        [SerializeField] Button coinLockBtn;
        [SerializeField] Button adLockBtn;

        private bool isDragging;
        private Vector3 lastPos;

        internal Action<StrawScrollItem> OnDragInSide;
        private Vector3 comparePos;
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
        protected void OnClickAdsBtn()
        {
            EventDispatcher.Instance.Dispatch(
                new EventKey.UnlockLocalData
                {
                    id = order,
                    isStraw = true,
                    purchaseType = WFSport.Base.PurchaseType.Ads,
                    obj = gameObject
                });
        }

        protected void OnClickCoinBtn(LocalDataRecord localDataRecord)
        {
            EventDispatcher.Instance.Dispatch(
                new EventKey.UnlockLocalData
                {
                    id = order,
                    isStraw = true,
                    purchaseType = WFSport.Base.PurchaseType.Coin,
                    amount = localDataRecord.Data.Amount,
                    obj = gameObject,
                });
        }

        internal void Setup(Sprite sprite, Vector3 comparePos, LocalDataRecord localRecord)
        {
            coinLockBtn.onClick.AddListener(OnClickAdsBtn);
            adLockBtn.onClick.AddListener(() => OnClickCoinBtn(localRecord));

            icon.sprite = sprite;
            icon.SetNativeSize();
            this.comparePos = comparePos;
            gameObject.SetActive(true);

            if (localRecord == null)
            {
                UnLock(true);
            }
            else
            {
                if (localRecord.Data.PurchaseType == WFSport.Base.PurchaseType.Coin)
                {
                    CoinLock();
                }
                else
                {
                    var priceTxt = coinLockBtn.GetComponentInChildren<TMP_Text>();
                    priceTxt.text = localRecord.Data.Amount.ToString();
                    AdsLock();
                }
            }
        }
        private void CoinLock()
        {
            coinLockBtn.gameObject.SetActive(true);
            Lock();
        }
        private void AdsLock()
        {
            adLockBtn.gameObject.SetActive(true);
            Lock();
        }
        private void Lock()
        {
            icon.color = Color.black;
            icon.DOFade(0.7f, 0);
        }
        public void UnLock(bool isImmediately = false)
        {
            if(isImmediately)
            {
                adLockBtn.gameObject.SetActive(false);
                coinLockBtn.gameObject.SetActive(false);
                icon.color = Color.white;
                icon.DOFade(1, 0);
            }
            else
            {
                animUnlock = DOTween.Sequence()
                    .Append(adLockBtn.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack))
                    .Join(coinLockBtn.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack))
                    .Join(icon.DOColor(Color.white, 0.5f).SetEase(Ease.Linear))
                    .Join(icon.DOFade(1, 0.5f).SetEase(Ease.Linear));
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
