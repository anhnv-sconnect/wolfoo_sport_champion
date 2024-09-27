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
        [SerializeField] private float fillWeight;

        private float playTime = 2;
        private Sequence animShow;
        private Sequence animHide;

        private void Start()
        {

        }
        private void OnDestroy()
        {
            animHide?.Kill();
            animShow?.Kill();
        }
        public void Setup(float time)
        {
            playTime = time;
        }
        public override void Hide()
        {
            if (animShow.IsActive())
            {
                animShow.OnComplete(() =>
                {
                    OnHideExcuting();
                });
            }
            else
            {
                OnHideExcuting();
            }
        }
        private void OnHideExcuting()
        {
            OnHiding?.Invoke();
            animHide = DOTween.Sequence()
                .Append(bg.DOFade(0, 0.25f));
            animHide.OnComplete(() =>
            {
                gameObject.SetActive(false);
                fillImg.rectTransform.sizeDelta = new Vector2(0, fillWeight);
                OnHided?.Invoke();
            });
        }

        public override void Show()
        {
            animHide?.Kill();
            fillImg.rectTransform.sizeDelta = new Vector2(0, fillWeight);
            
            OnShowing?.Invoke();
            gameObject.SetActive(true);
            animShow = DOTween.Sequence()
                .Append(bg.DOFade(1, 0))
                .Join(DOVirtual.Float(10, fillWidth, playTime, (value) =>
                {
                    fillImg.rectTransform.sizeDelta = new Vector2(value, fillWeight);
                }));
            animShow.OnComplete(() =>
            {
                OnShown?.Invoke();
            });
        }
    }
}
