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

        private Sprite[] scoreSprites;


        private Sequence animShow;
        private Sequence animHide;
        private Vector3 startPos;
        private float aliveTime;

        public bool IsShowing { get; private set; }
        public int Score { get; private set; }

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
        internal void Setup(int score, Sprite[] sprites)
        {
            scoreSprites = sprites;
            Score = score;

            if (score == 1) spriteRenderer.sprite = scoreSprites[0];
            else if (score == 2) spriteRenderer.sprite = scoreSprites[1];
            else if (score == 3) spriteRenderer.sprite = scoreSprites[2];
        }
        internal void Hide(bool isImmediately = false)
        {
            animShow?.Kill();

            if (isImmediately)
            {
                transform.localScale = Vector3.zero;
                IsShowing = false;
                return;
            }

            animHide = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.zero, 0.5f));
            animHide.OnComplete(() =>
            {
                IsShowing = false;
            });
        }
        internal void Show()
        {
            animHide?.Kill();
            IsShowing = true;

            animShow = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
            animShow.OnComplete(() =>
            {
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
