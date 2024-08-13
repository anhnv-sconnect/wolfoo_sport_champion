using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport
{
    public class LoadingBlur : LoadingPanel
    {
        [SerializeField] Image bgImg;
        private TweenerCore<Color, Color, ColorOptions> _tween;
        private Sequence _sequence;

        private void OnDestroy()
        {
            _tween?.Kill();
            _sequence?.Kill();
        }

        public override void Hide()
        {
            Debug.Log("Loading Blur Begin Hiding");
            _tween?.Kill();
            _tween = bgImg.DOFade(0, 1).OnComplete(() =>
            {
            Debug.Log("Loading Blur Hided");
                OnHide?.Invoke();
                gameObject.SetActive(false);
            });
        }

        public override void Show()
        {
            Debug.Log("Loading Blur Begin Showing");
            gameObject.SetActive(true);
            _tween?.Kill();
            _tween = bgImg.DOFade(1, 1).OnComplete(() =>
            {
                Debug.Log("Loading Blur Showed");
                OnShow?.Invoke();
            });
        }

        public void ShowToHide(float time)
        {
            Debug.Log("Loading Blur Begining");
            gameObject.SetActive(true);
            _sequence = DOTween.Sequence()
                .Append(bgImg.DOFade(1, 1))
                .AppendCallback(() => OnShow?.Invoke())
                .AppendInterval(time)
                .Append(bgImg.DOFade(0, 1))
                .AppendCallback(() => OnHide?.Invoke())
                .AppendCallback(() =>
                {
                    gameObject.SetActive(false);
                    Debug.Log("Loading Blur Ended");
                });
        }
    }
}
