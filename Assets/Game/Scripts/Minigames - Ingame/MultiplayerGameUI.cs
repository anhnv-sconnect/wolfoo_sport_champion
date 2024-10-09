using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WFSport.Base;

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
        private (float[] timelines, int lastScore, int timelineIdx) loadingBar1;
        private (float[] timelines, int lastScore, int timelineIdx) loadingBar2;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _tweenLoadingBar2?.Kill();
            _tweenLoadingBar?.Kill();
        }
        public void Setup(Sprite playerIcon1,Sprite playerIcon2)
        {
            if (playerIcon1 != null)
                mainCharcterIcon.sprite = playerIcon1;
            if (playerIcon2 != null)
                otherCharcterIcon.sprite = playerIcon2;
        }

        internal override void Setup(int time, float[] timelines)
        {
            barLength = fillBar2.rectTransform.rect.height;
            alienNormalizePos = 0 * barLength - barLength / 2;
            wolfooNormalizePos = 0 * barLength - barLength / 2;

            base.Setup(time, timelines);

            fillBar2.fillAmount = wolfooNormalizePos;
            mainCharacterSignal.localPosition = new Vector3(mainCharacterSignal.localPosition.x, wolfooNormalizePos, 0);
            otherCharacterSignal.localPosition = new Vector3(mainCharacterSignal.localPosition.x, alienNormalizePos, 0);

            loadingBar1 = (GetLoadingSteps(timelines), 0, 0);
            loadingBar2 = (GetLoadingSteps(timelines), 0, 0);
        }
        internal void SetupSinglePlayer()
        {
            otherCharacterSignal.gameObject.SetActive(false);
        }

        internal override void UpdateLoadingBar(int score)
        {
            if (loadingBar1.timelineIdx >= loadingBar1.timelines.Length) return;

            var step = (1f / loadingBar1.timelines.Length) * (loadingBar1.timelineIdx + 1);
            var totalStep = score - loadingBar1.lastScore;
            loadingBar1.lastScore = score;
            var value = fillBar.fillAmount + loadingBar1.timelines[loadingBar1.timelineIdx] * totalStep;
            if (value >= step) { loadingBar1.timelineIdx++; }
            if (value > 1) value = 1;

            wolfooNormalizePos = value * barLength - barLength / 2;

            mainCharacterSignal.SetAsLastSibling();
            fillBar.transform.SetAsLastSibling();

            _tweenLoadingBar?.Complete();
            _tweenLoadingBar = DOTween.Sequence()
                .Append(fillBar.DOFillAmount(value, 0.5f))
                .Join(mainCharacterSignal.DOLocalMoveY(wolfooNormalizePos, 0.5f));
            _tweenLoadingBar.OnComplete(() =>
            {
                CheckingStar();
            });
        }

        internal void UpdateLoadingBar2(int score)
        {
            if (loadingBar2.timelineIdx >= loadingBar2.timelines.Length) return;

            var step = (1f / loadingBar2.timelines.Length) * (loadingBar2.timelineIdx + 1);
            var totalStep = score - loadingBar2.lastScore;
            loadingBar2.lastScore = score;
            var value = fillBar2.fillAmount + loadingBar2.timelines[loadingBar2.timelineIdx] * totalStep;
            if (value >= step) { loadingBar2.timelineIdx++; }
            if (value > 1) value = 1;

            alienNormalizePos = value * barLength - barLength / 2;

            otherCharacterSignal.SetAsLastSibling();
            fillBar2.transform.SetAsLastSibling();

            _tweenLoadingBar2?.Kill();
            _tweenLoadingBar2 = DOTween.Sequence()
                .Append(fillBar2.DOFillAmount(value, 0.5f))
                .Join(otherCharacterSignal.DOLocalMoveY(alienNormalizePos, 0.5f));
            _tweenLoadingBar2.OnComplete(() =>
            {
                CheckingStar();
            });
        }
    }
}
