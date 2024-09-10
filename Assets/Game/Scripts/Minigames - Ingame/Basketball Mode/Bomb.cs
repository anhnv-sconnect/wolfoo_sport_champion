using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.BasketballMode
{
    public class Bomb : MonoBehaviour
    {
        public bool IsShowing { get; private set; }

        private Sequence animShow;
        private Sequence animHide;
        private Vector3 startPos;
        private float aliveTime;

        private void Start()
        {
            startPos = transform.localPosition;
        }
        private void OnDestroy()
        {
            animHide?.Kill();
            animShow?.Kill();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(TAG.PLAYER))
            {
                Hide();
            }
        }
        internal void Setup(GameplayConfig config)
        {
            aliveTime = config.aliveTime;
        }
        internal void Hide()
        {
            animShow?.Kill();
            animHide = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.zero, 0.5f));
        }
        internal void Show()
        {
            animHide?.Kill();
            animShow = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
            animShow.OnComplete(() =>
            {
                IsShowing = true;
                animShow = DOTween.Sequence()
                    .Append(transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 2).SetLoops(-1, LoopType.Restart))
                    .Append(transform.DOLocalMoveY(startPos.y + 0.5f, 1).SetLoops(-1, LoopType.Yoyo));
            });

        }
    }
}
