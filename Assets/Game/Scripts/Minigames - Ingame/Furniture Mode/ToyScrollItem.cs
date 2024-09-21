using SCN;
using UnityEngine;

namespace WFSport.Gameplay.FurnitureMode
{
    public class ToyScrollItem : ScrollItem
    {
        private Vector4 myLimit;

        public void Setup(Vector4 limitPos)
        {
            myLimit = limitPos;
        }

        protected override void OnClickAdsBtn()
        {
            EventDispatcher.Instance.Dispatch(new EventKey.UnlockLocalData
            {
                id = order,
                isToy = true,
                obj = gameObject,
                purchaseType = WFSport.Base.PurchaseType.Ads
            });
        }

        protected override void OnClickCoinBtn(LocalDataRecord localDataRecord)
        {
            EventDispatcher.Instance.Dispatch(new EventKey.UnlockLocalData
            {
                id = order,
                isToy = true,
                purchaseType = WFSport.Base.PurchaseType.Coin,
                amount = localDataRecord.Data.Amount,
                obj = gameObject
            });
        }

        protected override void OnEndDrag()
        {
            if(transform.position.x > myLimit.x && transform.position.x < myLimit.z
                && transform.position.y < myLimit.y && transform.position.y > myLimit.w)
            {
                OnDragInSide?.Invoke(this);
            }
        }
    }
}
