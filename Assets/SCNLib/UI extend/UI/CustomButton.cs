using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using SCN.Common;
using DG.Tweening;

namespace SCN.UIExtend
{
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(EventTrigger))]
	public class CustomButton : MonoBehaviour
	{
		public static System.Action<CustomButton> OnClickBtn;

		[SerializeField] EventTrigger et;
		[SerializeField] Image image;

		[Space(5)]
		[Header("Adjust")]
		[Tooltip("Hieu ung scale to ra khi bam vao")]
		[SerializeField] bool scaleWhenClick = true;
		[Tooltip("Hieu ung auto scale to ra, nho lai")]
		[SerializeField] bool scaleLoop = false;
		[Tooltip("Khi bam button, Tutorial se auto bi Kill")]
		[SerializeField] bool stopTutorial = true;
		[Tooltip("Khi bam button, se co am thanh bam nut")]
		[SerializeField] bool playSound = true;

		[Space(3)]
		[Tooltip("Mau khi normal button")]
		[SerializeField] Color normalColor = Color.white;
		[Tooltip("Mau khi disable button")]
		[SerializeField] Color disableColor = new Color(0.7843137f, 0.7843137f, 0.7843137f, 0.5f);

		[Space(3)]
		[Header("Anim")]
		[Tooltip("He so scale")]
		[SerializeField] float animScale = 1.2f;
		[Tooltip("Thoi gian anim")]
		[SerializeField] float animTime = 0.25f;

		[Space(10)]

		public UnityEvent OnClick;
		public UnityEvent OnPointerDown;
		public UnityEvent OnPointerUp;

		Vector3 scale;
		Tweener currentTweener;

		public bool ScaleWhenClick 
		{
			get => scaleWhenClick;
			set => scaleWhenClick = value;
		}

		public bool ScaleLoop
		{
			get => scaleLoop;
			set 
			{
				scaleLoop = value;
				if (value)
				{
					ScaleBtnLoop();
				}
				else
				{
					StopScaleBtnLoop();
				}
			}
		}

		public bool StopTutorial
		{
			get => stopTutorial;
			set => stopTutorial = value;
		}

		public bool PlaySound
		{
			get => playSound;
			set => playSound = value;
		}

		public bool Interactable
		{
			get => image.raycastTarget;
			set
			{
				image.raycastTarget = value;

				if (value)
				{
					image.color = normalColor;
				}
				else
				{
					image.color = disableColor;
				}
			}
		}

		public Vector3 Scale
		{
			get => scale;
			set => scale = value;
		}

		private void Awake()
		{
			if (image == null)
			{
				image = GetComponent<Image>();
			}
			if (et == null)
			{
				et = GetComponent<EventTrigger>();
			}

			Master.AddEventTriggerListener(et, EventTriggerType.PointerClick
				, Callback_OnPointerClick);

			Master.AddEventTriggerListener(et, EventTriggerType.PointerDown
				, Callback_OnPointerDown);

			Master.AddEventTriggerListener(et, EventTriggerType.PointerUp
				, Callback_OnPointerUp);
		}

		private void Start()
		{
			scale = transform.localScale;

			if (scaleLoop)
			{
				transform.localScale = scale * animScale;
				ScaleBtnLoop();
			}
		}

		public void InvokeClickManual()
		{
			Callback_OnPointerClick(null);
		}

		public void InvokeDownManual()
		{
			Callback_OnPointerDown(null);
		}

		public void InvokeUpManual()
		{
			Callback_OnPointerUp(null);
		}

		void Callback_OnPointerClick(BaseEventData data)
		{
			OnClickBtn?.Invoke(this);

			if (stopTutorial)
			{
			//	Tutorial.TutorialManager.Instance.Stop();
			}
			if (playSound)
			{
			//	AudioGlobal.Instance.PlayActionSound(AudioGlobal.ActionOp.Btn);
			}

			OnClick?.Invoke();
		}

		void Callback_OnPointerDown(BaseEventData data)
		{
			if (ScaleWhenClick)
			{
				DOTweenManager.Instance.KillTween(currentTweener);
				currentTweener = DOTweenManager.Instance.TweenScaleTime(
					transform, scale * 1.2f, 0.25f).SetEase(Ease.OutBack);
			}

			OnPointerDown?.Invoke();
		}

		void Callback_OnPointerUp(BaseEventData data)
		{
			if (scaleLoop)
			{
				ScaleBtnLoop();
			}
			else if (ScaleWhenClick)
			{
				DOTweenManager.Instance.KillTween(currentTweener);
				currentTweener = DOTweenManager.Instance.TweenScaleTime(transform, scale, 0.1f);
			}

			OnPointerUp?.Invoke();
		}

		void ScaleBtnLoop()
		{
			DOTweenManager.Instance.KillTween(currentTweener);
			currentTweener = DOTweenManager.Instance.TweenScaleTime(transform
				, scale * animScale, scale, animTime).SetLoops(-1, LoopType.Yoyo);
		}

		void StopScaleBtnLoop()
		{
			DOTweenManager.Instance.KillTween(currentTweener);
			transform.localScale = scale;
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (image == null)
			{
				image = GetComponent<Image>();
			}
			if (et == null)
			{
				et = GetComponent<EventTrigger>();
			}
		}
#endif
	}
}