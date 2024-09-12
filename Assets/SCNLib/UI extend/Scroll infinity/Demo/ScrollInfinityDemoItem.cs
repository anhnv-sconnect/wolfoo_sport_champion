using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SCN.Common;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace SCN.UIExtend
{
	public class ScrollInfinityDemoItem : ScrollItemBase
	{
		/// <summary>
		/// Nhac tay len sau khi da Drag item
		/// </summary>
		public static System.Action<ScrollInfinityDemoItem> OnPointerUpAfterDrag;

		/// <summary>
		/// Khi item dang duoc Drag
		/// </summary>
		public static System.Action<ScrollInfinityDemoItem> OnItemDragging;

		[SerializeField] Image image;
		[SerializeField] Text text;

		bool isDragging;

		protected override void Setup(int order)
		{
			var colorOrder = order % 3;
			if (colorOrder == 0)
			{
				image.color = Color.red;
			}
			else if(colorOrder == 1)
			{
				image.color = Color.green;
			}
			else
			{
				image.color = Color.blue;
			}

			text.text = (order + 1).ToString();

			Master.AddEventTriggerListener(EventTrigger, EventTriggerType.PointerDown, OnPointerDown);
			Master.AddEventTriggerListener(EventTrigger, EventTriggerType.PointerUp, OnPointerUp);
			Master.AddEventTriggerListener(EventTrigger, EventTriggerType.PointerClick, OnPointerClick);
		}

		protected override void OnStartDragOut()
		{
			image.transform.DOKill();
			DOTweenManager.Instance.TweenScaleTime(image.transform, Vector3.one * 1.5f, 0.3f)
				.SetEase(Ease.OutBack);

			isDragging = true;
		}

		protected override void OnDragOut()
		{
			OnItemDragging?.Invoke(this);
		}

		void OnPointerDown(BaseEventData data)
		{
			image.transform.DOKill();
			DOTweenManager.Instance.TweenScaleTime(image.transform, Vector3.one * 1.2f, 0.3f)
				.SetEase(Ease.OutBack);
		}

		void OnPointerUp(BaseEventData data)
		{
			if (isDragging)
			{
				isDragging = false;
				OnPointerUpAfterDrag?.Invoke(this);
			}

			image.transform.DOKill();
			image.transform.localScale = Vector3.one;
		}

		void OnPointerClick(BaseEventData data)
		{
			image.DOKill();
			DOTweenManager.Instance.TweenChangeAlphaImage(image, 1, 0.5f, 0.15f)
				.OnComplete(() =>
				{
					DOTweenManager.Instance.TweenChangeAlphaImage(image, 1, 0.15f);
				});
		}
	}
}