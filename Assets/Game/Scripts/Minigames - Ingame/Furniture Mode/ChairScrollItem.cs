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
    public class ChairScrollItem : ScrollItem
    {
        private Vector3 endPos;
        public void Setup(Vector3 endPos)
        {
            this.endPos = endPos;
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
