using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.FurnitureMode
{
    public class ChairScrollItem : ScrollItem
    {
        private Vector3 endPos;
        public void Setup(Vector3 endPos)
        {
            this.endPos = endPos;
        }

        protected override void OnClickAdsBtn()
        {
            EventDispatcher.Instance.Dispatch(
                new EventKey.UnlockLocalData
                {
                    id = order,
                    isChair = true,
                    purchaseType = WFSport.Base.PurchaseType.Ads,
                    obj = gameObject
                });
        }

        protected override void OnClickCoinBtn(LocalDataRecord localDataRecord)
        {
            EventDispatcher.Instance.Dispatch(
                new EventKey.UnlockLocalData
                {
                    id = order,
                    isChair = true,
                    purchaseType = WFSport.Base.PurchaseType.Coin,
                    amount = localDataRecord.Data.Amount,
                    obj = gameObject
                });
        }

        protected override void OnEndDrag()
        {
            var distance = Vector2.Distance(endPos, transform.position);
            if(distance < 2)
            {
                OnDragInSide?.Invoke(this);
            }
        }
    }
}
