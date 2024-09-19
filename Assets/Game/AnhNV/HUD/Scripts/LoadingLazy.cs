using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AnhNV.Dialog
{
    public class LoadingLazy : LoadingPanel
    {
        [SerializeField] Image bg;
        [SerializeField] Image fillImg;

        private Sequence anim;

        private void Start()
        {
            fillImg.fillAmount = 0;
            bg.DOFade(0, 0);
        }
        private void OnDestroy()
        {
            anim?.Kill();
        }
        public override void Hide()
        {
            var fillValue = fillImg.fillAmount;
            anim?.Kill();
            anim = DOTween.Sequence()
                .Append(fillImg.DOFillAmount(1, fillValue == 1 ? 0 : 0.5f))
                .Join(bg.DOFade(1, 0))
                .Append(bg.DOFade(0, 0.5f));
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
            fillImg.fillAmount = 0;
            anim = DOTween.Sequence()
                .Append(bg.DOFade(1, 0.5f))
                .Join(fillImg.DOFillAmount(1, 2));
            anim.OnComplete(() =>
            {
                OnShow?.Invoke();
            });
        }
    }
}
