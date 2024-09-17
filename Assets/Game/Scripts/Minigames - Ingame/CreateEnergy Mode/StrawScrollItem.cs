using SCN.Common;
using SCN.UIExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WFSport.Gameplay.CreateEnergyMode
{
    public class StrawScrollItem : ScrollItemBase
    {
        [SerializeField] Image icon;

        private bool isDragging;
        private Vector3 lastPos;

        internal Action<StrawScrollItem> OnDragInSide;
        private Vector3 comparePos;

        public Sprite Icon { get => icon.sprite;}

        protected override void Setup(int order)
        {
            ReturnAfterUnselect = true;
			Master.AddEventTriggerListener(EventTrigger, EventTriggerType.PointerUp, OnPointerUp);
			Master.AddEventTriggerListener(EventTrigger, EventTriggerType.Drag, OnDrag);
        }

        internal void Setup(Sprite sprite, Vector3 comparePos)
        {
            icon.sprite = sprite;
            icon.SetNativeSize();
            this.comparePos = comparePos;
            gameObject.SetActive(true);
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
