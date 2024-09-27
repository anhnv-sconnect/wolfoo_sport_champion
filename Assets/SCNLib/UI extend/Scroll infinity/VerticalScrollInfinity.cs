using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WFSport.Helper;

namespace SCN.UIExtend
{
    public class VerticalScrollInfinity : ScrollInfinityBase
	{
        private Sequence moveAnim;
        private bool isMyInit;
        private Vector2 outSide;
        private Vector3 initPos;

		public bool IsAlready { get => isInit; }

        private void OnDestroy()
        {
			moveAnim?.Kill();
        }
        private void Start()
        {
			MyInit();
        }
        private void MyInit()
		{
			if (isMyInit) return;
			isMyInit = true;

			outSide = ScreenHelper.GetMaxPosition();
			initPos = transform.position;
		}
		
        internal void MoveOut(bool isImmediately = false)
		{
			MyInit();
			moveAnim?.Kill();
			if(isImmediately)
			{
				transform.position = new Vector3(outSide.x + 3.2f, initPos.y, initPos.z);
				return;
			}
            moveAnim = DOTween.Sequence()
				.Append(transform.DOMoveX(outSide.x + 3.2f, 0.5f).SetEase(Ease.InBack));
		}
        internal void MoveIn()
		{
			MyInit();
			moveAnim?.Kill();
			moveAnim = DOTween.Sequence()
				.Append(transform.DOMoveX(initPos.x, 0.5f).SetEase(Ease.OutBack));
		}
		public override void MoveDelta(float delta)
		{
			for (int i = 0; i < maskTrans.childCount; i++)
			{
				var pos = maskTrans.GetChild(i).position;
				maskTrans.GetChild(i).position = new Vector3(pos.x, pos.y + delta, pos.z);
			}
		}

		public override void UpdateMoveDir(Vector2 currentVtDrag)
		{
			if (currentState != State.MoveReverse && currentVtDrag.y < 0)
			{
				currentState = State.MoveReverse;
			}
			else if (currentState != State.MoveDefault && currentVtDrag.y >= 0)
			{
				currentState = State.MoveDefault;
			}

			UpdateChild();
		}

		protected override void SetFirstPosition(ScrollItemBase item)
		{
			item.RectTrans.anchoredPosition = new Vector2(0
				, maskTrans.rect.size.y / 2 - item.RectTrans.sizeDelta.y / 2 - spacing);
		}

		protected override IEnumerator AutoMoveIE()
		{
			while (true)
			{
				var vtDrag = new Vector2(0, Time.deltaTime * velocity);
				MoveDelta(vtDrag.y);
				UpdateMoveDir(vtDrag);

				yield return null;
			}
		}

		protected override void UpdateChild()
		{
			if (currentState == State.MoveReverse)
			{
				while (true)
				{
					var lastItem = maskTrans.GetChild(itemCount - 1).GetComponent<ScrollItemBase>();
					if (lastItem.RectTrans.anchoredPosition.y < -litmitPoint.anchoredPosition.y)
					{
						SetItemAsFirst(lastItem);
					}
					else
					{
						break;
					}
				}
			}
			else if (currentState == State.MoveDefault)
			{
				while (true)
				{
					var firstItem = maskTrans.GetChild(0).GetComponent<ScrollItemBase>();
					if (firstItem.RectTrans.anchoredPosition.y > litmitPoint.anchoredPosition.y)
					{
						SetItemAsLast(firstItem);
					}
					else
					{
						break;
					}
				}
			}
		}

		protected override void SetItemAsLast(ScrollItemBase item)
		{
			item.RectTrans.SetAsLastSibling();
			var lastItem = maskTrans.GetChild(item.RectTrans.GetSiblingIndex() - 1)
				.GetComponent<ScrollItemBase>().RectTrans;
			item.RectTrans.anchoredPosition = new Vector2(0, lastItem.anchoredPosition.y
				- lastItem.sizeDelta.y / 2 - item.RectTrans.sizeDelta.y / 2 - spacing);
		}

		protected override void SetItemAsFirst(ScrollItemBase item)
		{
			item.RectTrans.SetAsFirstSibling();
			var fistItem = maskTrans.GetChild(1).GetComponent<ScrollItemBase>().RectTrans;
			item.RectTrans.anchoredPosition = new Vector2(0, fistItem.anchoredPosition.y
				+ spacing + item.RectTrans.sizeDelta.y / 2 + item.RectTrans.sizeDelta.y / 2);
		}

		#region Callback
		protected override void Callback_OnDrag(BaseEventData data)
		{
			var vt = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			var vtDragTemp = (Vector2)vt - lastPos;
			lastPos = vt;

			UpdateMoveDir(vtDragTemp);

			MoveDelta(vtDragTemp.y);
		}
		#endregion
	}
}