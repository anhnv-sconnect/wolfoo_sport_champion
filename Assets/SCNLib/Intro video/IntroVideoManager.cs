using DG.Tweening;
using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

namespace SCN.MediaPlayer
{
	public class IntroVideoManager : MonoBehaviour
	{
		public static IntroVideoManager Instance { get; private set; }

		[SerializeField] VideoPlayer videoPlayer;

		[SerializeField] Image bgVideoImage;
		[SerializeField] RawImage videoImage;
		[SerializeField] Button skipBtn;

		[Space(5)]
		[Header("Custom")]
		[SerializeField] bool isPlayAwake = true;
		[SerializeField] bool isSkipable = true;
		[SerializeField] Vector2 textureSize;
		public UnityEvent OnVideoComplete;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}

			bgVideoImage.gameObject.SetActive(false);
			videoImage.gameObject.SetActive(false);
			skipBtn.gameObject.SetActive(false);

			skipBtn.onClick.RemoveAllListeners();
			skipBtn.onClick.AddListener(Callback_OnClickSkipBtn);

			if (isPlayAwake)
			{
				Play(isSkipable);
			}
		}

		private void Start()
		{
			var coeff = (float)videoImage.texture.width / videoImage.texture.height;

			var screenSize = GetComponent<RectTransform>().rect.size;

			var rect = videoImage.GetComponent<RectTransform>();
			rect.sizeDelta = new Vector2(screenSize.x, screenSize.x / coeff);
		}

		public void SetVideoClip(VideoClip video)
		{
			videoPlayer.clip = video;
		}

		public void Play(bool skipable = true)
		{
			bgVideoImage.gameObject.SetActive(true);
			videoImage.gameObject.SetActive(true);
			skipBtn.gameObject.SetActive(skipable);

			if (videoPlayer == null)
			{
				Debug.LogWarning("Null video");
				return;
			}
			else
			{
				videoPlayer.Play();
				StartCoroutine(DelayCallMaster.WaitAndDoIE((float)videoPlayer.clip.length + 0.5f, () =>
				{
					Stop(true);
				}));
			}
		}

		void Stop(bool anim)
		{
			StopAllCoroutines();
			videoPlayer.Stop();

			skipBtn.gameObject.SetActive(false);
			videoImage.gameObject.SetActive(false);

			if (anim)
			{
				DOTweenManager.Instance.TweenChangeAlphaImage(bgVideoImage, 1, 0, 0.25f)
				.OnComplete(() =>
				{
					bgVideoImage.gameObject.SetActive(false);
					OnVideoComplete?.Invoke();
				});
			}
			else
			{
				bgVideoImage.gameObject.SetActive(false);
				OnVideoComplete?.Invoke();
			}
		}

		void Callback_OnClickSkipBtn()
		{
			Stop(false);
		}
	}
}