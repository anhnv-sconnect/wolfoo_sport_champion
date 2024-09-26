using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AnhNV.Dialog
{
    public class LoadingLazy : LoadingPanel
    {
        [SerializeField] CanvasGroup bg;
        [SerializeField] Image fillImg;
        [SerializeField] private float fillWidth = 1250;

        private float playTime = 2;
        private Sequence anim;
        private Vector2 fillSize;

        private void Start()
        {
        }
        private void OnDestroy()
        {
            anim?.Kill();
        }
        public void Setup(float time)
        {
            playTime = time;
            fillSize = fillImg.rectTransform.sizeDelta;
            fillImg.rectTransform.sizeDelta = new Vector2(0, fillSize.y);
            bg.DOFade(0, 0);
        }
        public override void Hide()
        {
            anim?.Kill();
            anim = DOTween.Sequence()
                .Append(DOVirtual.Float(fillWidth, 0, 0.25f, (value) =>
                {
                    fillImg.rectTransform.sizeDelta = new Vector2(value, fillSize.y);
                }))
                .Join(bg.DOFade(1, 0.25f))
                .Append(bg.DOFade(0, 0.25f));
            anim.OnComplete(() =>
            {
                gameObject.SetActive(false);
                OnHide?.Invoke();
            });
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            anim?.Kill();
            fillImg.rectTransform.sizeDelta = new Vector2(0, fillSize.y);
            anim = DOTween.Sequence()
                .Append(bg.DOFade(1, 0.5f))
                .Join(DOVirtual.Float(0, fillWidth, playTime, (value) =>
                {
                    fillImg.rectTransform.sizeDelta = new Vector2(value, fillSize.y);
                }));
            anim.OnComplete(() =>
            {
                OnShow?.Invoke();
            });
        }
    }
}
