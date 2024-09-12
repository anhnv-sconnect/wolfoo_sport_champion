using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SCN.UIExtend
{
	public class HorizontalScrollInfinity : ScrollInfinityBase
	{
		public override void MoveDelta(float delta)
		{
			for (int i = 0; i < maskTrans.childCount; i++)
			{
				var pos = maskTrans.GetChild(i).position;
				maskTrans.GetChild(i).position = new Vector3(pos.x + delta, pos.y, pos.z);
			}
		}

		public override void UpdateMoveDir(Vector2 currentVtDrag)
		{
			if (currentState != State.MoveReverse && currentVtDrag.x >= 0)
			{
				currentState = State.MoveReverse;
			}
			else if (currentState != State.MoveDefault && currentVtDrag.x < 0)
			{
				currentState = State.MoveDefault;
			}

			UpdateChild();
		}

		protected override void SetFirstPosition(ScrollItemBase item)
		{
			item.RectTrans.anchoredPosition = new Vector2(
				- maskTrans.rect.size.x / 2 + item.RectTrans.sizeDelta.x / 2 + spacing, 0);
		}

		protected override IEnumerator AutoMoveIE()
		{
			while (true)
			{
				var vtDrag = new Vector2(-Time.deltaTime * velocity, 0);
				MoveDelta(vtDrag.x);
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
					if (lastItem.RectTrans.anchoredPosition.x > -litmitPoint.anchoredPosition.x)
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
					if (firstItem.RectTrans.anchoredPosition.x < litmitPoint.anchoredPosition.x)
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
			item.RectTrans.anchoredPosition = new Vector2(lastItem.anchoredPosition.x
				+ lastItem.sizeDelta.x / 2 + item.RectTrans.sizeDelta.x / 2 + spacing, 0);
		}

		protected override void SetItemAsFirst(ScrollItemBase item)
		{
			item.RectTrans.SetAsFirstSibling();
			var fistItem = maskTrans.GetChild(1).GetComponent<ScrollItemBase>().RectTrans;
			item.RectTrans.anchoredPosition = new Vector2(fistItem.anchoredPosition.x
				- spacing - item.RectTrans.sizeDelta.x / 2 - item.RectTrans.sizeDelta.x / 2, 0);
		}

		#region Callback
		protected override void Callback_OnDrag(BaseEventData data)
		{
			var vt = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			var vtDragTemp = (Vector2)vt - lastPos;

			UpdateMoveDir(vtDragTemp);

			MoveDelta(vtDragTemp.x);
			lastPos = vt;
		}
		#endregion
	}
}