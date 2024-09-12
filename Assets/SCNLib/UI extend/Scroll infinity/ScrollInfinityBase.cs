using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SCN.Common;

namespace SCN.UIExtend
{
	public abstract class ScrollInfinityBase : MonoBehaviour
	{
		public enum State
		{
			/// <summary>
			/// Verticle => up, Horizontal => left
			/// </summary>
			MoveDefault = 0,
			/// <summary>
			/// Verticle => down, Horizontal => right
			/// </summary>
			MoveReverse = 1,
			Drag = 2,
			None = 3,
			Break = 4
		}

		protected RectTransform maskTrans;
		protected EventTrigger eventTrigger;
		protected RectTransform litmitPoint;
		protected GameObject blockObj;

		[Space]
		[Header("Custom")]
		[SerializeField] protected ScrollItemBase prefab;

		[SerializeField] protected int itemCount;
		[SerializeField] protected int spacing;
		[SerializeField] protected float velocity = 1;
		[SerializeField] protected bool isAutoMove = true;

		[SerializeField] protected bool initAwake;

		[SerializeField] protected State currentState = State.None;

		protected bool isInit;

		protected Vector2 lastPos;

		protected MonoBehaviour mono;
		protected Coroutine autoMoveCorou;

		protected List<ScrollItemBase> listItem = new List<ScrollItemBase>();

		public RectTransform MaskTrans => maskTrans;

		public List<ScrollItemBase> ListItem => listItem;

		public float Velocity
        {
			get => velocity;
			set => velocity = value;
        } 

		public State CurrentState
		{
			get => currentState;
			set => currentState = value;
		}

		private void Awake()
		{
			if (initAwake)
			{
				Setup(itemCount, this);
			}
		}

		public void Setup(int itemCount, MonoBehaviour mono = null)
		{
			if (isInit)
			{
				return;
			}
			isInit = true;

			maskTrans = Master.GetChildByName(gameObject, "Mask").GetComponent<RectTransform>();
			eventTrigger = maskTrans.GetComponent<EventTrigger>();
			litmitPoint = Master.GetChildByName(gameObject, "Litmit point").GetComponent<RectTransform>();
			blockObj = Master.GetChildByName(gameObject, "Block");

			blockObj.SetActive(false);

			currentState = State.None;
			this.itemCount = itemCount;
			this.mono = mono;

			listItem.Clear();
			for (int i = 0; i < itemCount; i++)
			{
				var item = Instantiate(prefab.gameObject, maskTrans).GetComponent<ScrollItemBase>();
				listItem.Add(item);

				item.name = i.ToString();
				item.Init(i, this);

				// item dau
				if (i == 0)
				{
					SetFirstPosition(item);
				}
				else
				{
					SetItemAsLast(item);
				}
			}

			Master.AddEventTriggerListener(eventTrigger, EventTriggerType.PointerDown, Callback_OnPointerDown);
			Master.AddEventTriggerListener(eventTrigger, EventTriggerType.Drag, Callback_OnDrag);
			Master.AddEventTriggerListener(eventTrigger, EventTriggerType.PointerUp, Callback_OnPointerUp);

			PlayAutoMove();
		}

		#region Public methob
		public void BlockInput(bool block)
		{
			blockObj.SetActive(block);
		}

		public void StopAutoMove()
		{
			if (!isAutoMove)
			{
				return;
			}

			CurrentState = State.None;
			if (autoMoveCorou != null)
			{
				mono.StopCoroutine(autoMoveCorou);
			}
		}

		public void PlayAutoMove()
		{
			if (!isAutoMove)
			{
				return;
			}

			if (autoMoveCorou != null)
			{
				mono.StopCoroutine(autoMoveCorou);
			}
			autoMoveCorou = mono.StartCoroutine(AutoMoveIE());
		}

		public abstract void MoveDelta(float delta);
		public abstract void UpdateMoveDir(Vector2 currentVtDrag);
		#endregion

		#region Callback
		protected virtual void Callback_OnPointerDown(BaseEventData data)
		{
			StopAutoMove();

			var vt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			lastPos = vt;
		}

		protected virtual void Callback_OnDrag(BaseEventData data)
		{

		}

		protected virtual void Callback_OnPointerUp(BaseEventData data)
		{
			PlayAutoMove();
		}
		#endregion

		protected abstract void SetFirstPosition(ScrollItemBase item);
		protected abstract IEnumerator AutoMoveIE();
		protected abstract void UpdateChild();

		protected abstract void SetItemAsLast(ScrollItemBase item);

		protected abstract void SetItemAsFirst(ScrollItemBase item);
	}
}