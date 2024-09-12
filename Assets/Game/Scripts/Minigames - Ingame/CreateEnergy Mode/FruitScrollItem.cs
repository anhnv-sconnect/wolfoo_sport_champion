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
    public class FruitScrollItem : ScrollItemBase
    {
        [SerializeField] Image icon;

        private (Fruit pb, Transform parent, Vector3 endPos, Vector3 comparePos) fruitData;
        private bool isDragging;
        private Vector3 lastPos;

        protected override void Setup(int order)
        {
            ReturnAfterUnselect = true;
			Master.AddEventTriggerListener(EventTrigger, EventTriggerType.PointerUp, OnPointerUp);
			Master.AddEventTriggerListener(EventTrigger, EventTriggerType.Drag, OnDrag);
        }

        internal void Setup(Sprite sprite, Transform fruitParent, Vector3 flyInPos, Vector3 comparePos, Fruit fruitPb)
        {
            icon.sprite = sprite;
            icon.SetNativeSize();
            fruitData.parent = fruitParent;
            flyInPos.z = 0;
            fruitData.endPos = flyInPos;
            fruitData.comparePos = comparePos;
            fruitData.pb = fruitPb;
            gameObject.SetActive(true);
        }
        internal void CreateFruit()
        {
            var fruit = Instantiate(fruitData.pb, fruitData.parent);
            fruit.transform.position = transform.position;
            fruit.Setup(icon.sprite);
            fruit.JumpTo(fruitData.endPos);
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
                var distance = Vector2.Distance(lastPos, fruitData.comparePos);
                if(distance < 2)
                {
                    CreateFruit();
                }
            }
            isDragging = false;
        }
    }
}
