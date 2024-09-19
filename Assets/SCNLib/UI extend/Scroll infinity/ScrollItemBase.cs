using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SCN.Common;
using UnityEngine.UI;

namespace SCN.UIExtend
{
	public abstract class ScrollItemBase : MonoBehaviour
	{
		[SerializeField] EventTrigger eventTrigger;

		[Space(10)]
		[Header("Custom")]
		[SerializeField] bool canBePulledOut = true;
		[Tooltip("Neu bo tick, scroll se ko auto chay nua sau khi bo tay khoi Item")]
		[SerializeField] bool returnAfterUnselect = true;

		RectTransform rectTrans;
		protected int order;

		protected ScrollInfinityBase scrollInfinity;

		int lastSibling;
		Vector3 startDragPos;
		Vector3 firstTouchPos;
		Vector3 lastTouchPos;
		Vector3 deltaVt;

		bool isInit;
		bool isVertical;

		bool isAddListener;
		bool isDrag;

		protected const float deltaDetectDir = 0.5f;
		protected const float deltaBreak = 2f;

		public EventTrigger EventTrigger => eventTrigger;
		public RectTransform RectTrans => rectTrans;
		public int Order => order;
		public Vector3 StartDragPos => startDragPos;

		public bool CanBePulledOut
		{
			get => canBePulledOut;
			set => canBePulledOut = value;
		}

		public bool ReturnAfterUnselect
		{
			get => returnAfterUnselect;
			set => returnAfterUnselect = value;
		}

		public void Init(int order, ScrollInfinityBase scrollInfinity)
		{
			if (isInit)
			{
				return;
			}
			isInit = true;

			this.order = order;
			this.scrollInfinity = scrollInfinity;
			isVertical = scrollInfinity is VerticalScrollInfinity;

			rectTrans = GetComponent<RectTransform>();

			Setup(order);

			AddEventTriggerListener();
		}

		/// <summary>
		/// Goi de tiep tuc auto scroll, dung trong truong hop returnAfterUnselect = false
		/// </summary>
		public void Continue()
		{
			if (returnAfterUnselect)
			{
				return;
			}

			scrollInfinity.BlockInput(false);
			eventTrigger.enabled = true;

			SetItemBackToScroll();
		}

		public void AddEventTriggerListener()
		{
			if (isAddListener)
			{
				return;
			}
			isAddListener = true;

			Master.AddEventTriggerListener(eventTrigger, EventTriggerType.PointerDown, Callback_OnPointerDown);
			Master.AddEventTriggerListener(eventTrigger, EventTriggerType.Drag, Callback_OnDrag);
			Master.AddEventTriggerListener(eventTrigger, EventTriggerType.PointerUp, Callback_OnPointerUp);
		}

		public void RemoveEventTriggerListener()
		{
			isAddListener = false;

			Master.RemoveEventTriggerListener(eventTrigger, EventTriggerType.PointerDown, Callback_OnPointerDown);
			Master.RemoveEventTriggerListener(eventTrigger, EventTriggerType.Drag, Callback_OnDrag);
			Master.RemoveEventTriggerListener(eventTrigger, EventTriggerType.PointerUp, Callback_OnPointerUp);
		}

		#region Virtual methob
		protected abstract void Setup(int order);

		/// <summary>
		/// Xay ra khi item bat dau duoc keo ra ngoai
		/// </summary>
		protected virtual void OnStartDragOut()
		{

		}

		protected virtual void OnClickNoDrag()
		{

		}

		/// <summary>
		/// Xay ra khi item dang duoc drag o ben ngoai
		/// </summary>
		protected virtual void OnDragOut()
		{

		}
		#endregion

		#region Callback
		void Callback_OnPointerDown(BaseEventData data)
		{
			scrollInfinity.StopAutoMove();
			isDrag = false;
			
			var vt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			vt.z = transform.position.z;

			firstTouchPos = vt;
			lastTouchPos = vt;

			deltaVt = transform.position - vt;
		}

		void Callback_OnDrag(BaseEventData data)
		{
			isDrag = true;
			if (scrollInfinity.CurrentState == ScrollInfinityBase.State.Break)
			{
				return;
			}

			var vt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			vt.z = transform.position.z;

			var vtDragTemp = vt - lastTouchPos;
			var vtDrag = vt - firstTouchPos;

			lastTouchPos = vt;

			if (scrollInfinity.CurrentState == ScrollInfinityBase.State.None)
			{
				scrollInfinity.CurrentState = DetectFirstAction(vtDrag);

				if (scrollInfinity.CurrentState == ScrollInfinityBase.State.Drag)
				{
					OnStartDragOut();

					lastSibling = transform.GetSiblingIndex();
					startDragPos = transform.position;

					transform.SetParent(scrollInfinity.transform);
				}
			}
			else if (scrollInfinity.CurrentState == ScrollInfinityBase.State.MoveDefault 
				|| scrollInfinity.CurrentState == ScrollInfinityBase.State.MoveReverse)
			{
				if (CheckBreak(vtDrag))
				{
					scrollInfinity.CurrentState = ScrollInfinityBase.State.Break;
				}
				else
				{
					scrollInfinity.UpdateMoveDir(vtDragTemp);
					scrollInfinity.MoveDelta(isVertical ? vtDragTemp.y : vtDragTemp.x);
				}
			}
			else if (scrollInfinity.CurrentState == ScrollInfinityBase.State.Drag)
			{
				OnDragOut();

				vt.z = transform.position.z;
				transform.position = vt + deltaVt;
			}
		}

		void Callback_OnPointerUp(BaseEventData data)
		{
			if (!isDrag)
			{
				OnClickNoDrag();
			}

			if (scrollInfinity.CurrentState == ScrollInfinityBase.State.Drag)
			{
				if (returnAfterUnselect)
				{
					SetItemBackToScroll();
				}
				else
				{
					scrollInfinity.BlockInput(true);
					eventTrigger.enabled = false;
				}
			}
			else
			{
				scrollInfinity.PlayAutoMove();
			}
		}
		#endregion

		void SetItemBackToScroll()
		{
			transform.SetParent(scrollInfinity.MaskTrans);
			transform.SetSiblingIndex(lastSibling);
			transform.position = startDragPos;

			scrollInfinity.PlayAutoMove();
		}

		ScrollInfinityBase.State DetectFirstAction(Vector2 vt)
		{
			if (canBePulledOut)
			{
				if (isVertical)
				{
					if (Mathf.Abs(vt.x) > deltaDetectDir)
					{
						return ScrollInfinityBase.State.Drag;
					}
					else if (Mathf.Abs(vt.y) > deltaDetectDir)
					{
						return vt.y >= 0 ? ScrollInfinityBase.State.MoveDefault
							: ScrollInfinityBase.State.MoveReverse;
					}
				}
				else
				{
					if (Mathf.Abs(vt.y) > deltaDetectDir)
					{
						return ScrollInfinityBase.State.Drag;
					}
					else if (Mathf.Abs(vt.x) > deltaDetectDir)
					{
						return vt.x < 0 ? ScrollInfinityBase.State.MoveDefault
							: ScrollInfinityBase.State.MoveReverse;
					}
				}
			}
			else
			{
				return (isVertical ? (vt.y >= 0) : (vt.x < 0)) 
					? ScrollInfinityBase.State.MoveDefault
					: ScrollInfinityBase.State.MoveReverse;
			}

			return ScrollInfinityBase.State.None;
		}

		bool CheckBreak(Vector2 vt)
		{
			if (canBePulledOut)
			{
				if (Mathf.Abs(isVertical ? vt.x : vt.y) > deltaBreak)
				{
					return true;
				}
			}

			return false;
		}
	}
}