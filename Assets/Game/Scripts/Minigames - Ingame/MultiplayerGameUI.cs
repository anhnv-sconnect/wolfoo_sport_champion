using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport.Gameplay
{
    public class MultiplayerGameUI : MinigameUI
    {
        [SerializeField] private Transform mainCharacterSignal;
        [SerializeField] private Transform otherCharacterSignal;
        [SerializeField] private Image mainCharcterIcon;
        [SerializeField] private Image otherCharcterIcon;
        [SerializeField] private Image fillBar2;
        private float barLength;
        private float wolfooNormalizePos;
        private float alienNormalizePos;
        private Sequence _tweenLoadingBar;
        private Sequence _tweenLoadingBar2;

        protected override void Start()
        {
            base.Start();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _tweenLoadingBar2?.Kill();
            _tweenLoadingBar?.Kill();
        }

        internal override void Setup(int time, float[] timeline)
        {
            barLength = fillBar2.rectTransform.rect.height;
            alienNormalizePos = 0 * barLength - barLength / 2;
            wolfooNormalizePos = 0 * barLength - barLength / 2;

            base.Setup(time, timeline);

            fillBar2.fillAmount = wolfooNormalizePos;
            mainCharacterSignal.localPosition = new Vector3(mainCharacterSignal.localPosition.x, wolfooNormalizePos, 0);
            otherCharacterSignal.localPosition = new Vector3(mainCharacterSignal.localPosition.x, alienNormalizePos, 0);
        }

        internal override void UpdateLoadingBar(float value)
        {
            if (value > 1) return;

            wolfooNormalizePos = value * barLength - barLength / 2;

            mainCharacterSignal.SetAsLastSibling();
            fillBar.transform.SetAsLastSibling();

            _tweenLoadingBar?.Kill();
            _tweenLoadingBar = DOTween.Sequence()
                .Append(fillBar.DOFillAmount(value, 0.5f))
                .Join(mainCharacterSignal.DOLocalMoveY(wolfooNormalizePos, 0.5f));
            _tweenLoadingBar.OnComplete(() =>
            {
                CheckingStar(fillBar.fillAmount);
            });
        }

        internal void UpdateLoadingBar2(float value)
        {
            if (value > 1) return;

            alienNormalizePos = value * barLength - barLength / 2;

            otherCharacterSignal.SetAsLastSibling();
            fillBar2.transform.SetAsLastSibling();

            _tweenLoadingBar2?.Kill();
            _tweenLoadingBar2 = DOTween.Sequence()
                .Append(fillBar2.DOFillAmount(value, 0.5f))
                .Join(otherCharacterSignal.DOLocalMoveY(alienNormalizePos, 0.5f));
            _tweenLoadingBar2.OnComplete(() =>
            {
                CheckingStar(value);
            });
        }
    }
}
