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
    public class OtherScrollItem : ScrollItem
    {
        private Vector4 myLimit;

        public void Setup(Vector4 limitPos)
        {
            myLimit = limitPos;
        }
        protected override void OnEndDrag()
        {
            if (transform.position.x > myLimit.x && transform.position.x < myLimit.z
                && transform.position.y < myLimit.y && transform.position.y > myLimit.w)
            {
                OnDragInSide?.Invoke(this);
            }
        }
    }
}
