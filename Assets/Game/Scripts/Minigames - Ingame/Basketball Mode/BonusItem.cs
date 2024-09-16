using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.BasketballMode
{
    public class BonusItem : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Sprite[] scoreSprites;

        public bool IsShowing { get; private set; }

        private Sequence animShow;
        private Sequence animHide;
        private Vector3 startPos;
        private float aliveTime;

        private Action OnShow;
        private Action OnComplete;

        private void Start()
        {
            startPos = transform.localPosition;
        }
        private void OnDestroy()
        {
            animHide?.Kill();
            animShow?.Kill();
        }
        internal void Setup(GameplayConfig config)
        {
            aliveTime = config.aliveTime;
            transform.localScale = Vector3.zero;
        }
        internal void Setup(int score)
        {
            if (score == 1) spriteRenderer.sprite = scoreSprites[0];
            else if (score == 2) spriteRenderer.sprite = scoreSprites[1];
            else if (score == 3) spriteRenderer.sprite = scoreSprites[2];
        }
        internal void Hide()
        {
            OnComplete?.Invoke();
            animShow?.Kill();
            animHide = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.zero, 0.5f));
        }
        internal void Show(System.Action OnShowing, System.Action OnComplete)
        {
            OnShow = OnShowing;
            this.OnComplete = OnComplete;

            animHide?.Kill();
            animShow = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
            animShow.OnComplete(() =>
            {
                IsShowing = true;
                OnShow?.Invoke();
                animShow = DOTween.Sequence()
                    .Append(transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 2).SetLoops(-1, LoopType.Restart))
                    .Append(transform.DOLocalMoveY(startPos.y + 0.5f, 1).SetLoops(-1, LoopType.Yoyo))
                    .AppendInterval(aliveTime)
                    .AppendCallback(() =>
                    {
                        Hide();
                    });
            });
        }

        internal void Pause()
        {
            animHide?.Pause();
            animShow?.Pause();
        }
        internal void Play()
        {
            animHide?.Play();
            animShow?.Play();
        }
    }
}
