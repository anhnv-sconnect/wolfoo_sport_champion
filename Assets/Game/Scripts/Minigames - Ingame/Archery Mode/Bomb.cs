using AnhNV.Helper;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.ArcheryMode
{
    public class Bomb : Marker
    {
        private float myLimit;
        private float delayHideTime;
        private Vector3 initLocalPos;
        private Quaternion initRotaton;
        private bool isInit;
        private bool canCompare;

        private Sequence _tweenHide;
        private Sequence _tweenShow;
        private Sequence _animCorrect;

        private void Start()
        {
            Init();
        }
        private void OnDestroy()
        {
            _animCorrect?.Kill();
            _tweenShow?.Kill();
            _tweenHide?.Kill();
        }

        internal override void OnHitCorrect(Vector3 position)
        {
            Holder.PlayParticle?.Invoke();

            canCompare = false;
            markedHole.transform.position = position;
            markedHole.SetActive(true);

            _tweenShow?.Kill();

            _animCorrect?.Kill();
            _animCorrect = DOTween.Sequence()
                .Append(transform.DOLocalMove(initLocalPos, 0))
                .Append(transform.DOLocalMoveX(initLocalPos.x - 0.1f, 0.1f).SetEase(Ease.Flash))
                .Append(transform.DOLocalMoveX(initLocalPos.x + 0.1f, 0.2f).SetEase(Ease.Flash))
                .Append(transform.DOLocalMoveX(initLocalPos.x, 0.1f).SetEase(Ease.Flash));
            _animCorrect.OnComplete(() =>
            {
                OnHiding();
            });
        }

        internal override void Init()
        {
            if (isInit) return;
            isInit = true;

            myLimit = myCollider.radius;
            initLocalPos = transform.localPosition;
            initRotaton = transform.rotation;

            markedHole.SetActive(false);
            gameObject.SetActive(false);

            if (IsSpecial) InitSpecial();

            IsShowing = false;
            canCompare = true;
        }
        private void OnHiding()
        {
            markedHole.SetActive(false);
            gameObject.SetActive(false);
            IsShowing = false;

            IsSpecial = false;
        }

        internal override void Hide()
        {

            _tweenHide?.Kill();
            _tweenHide = DOTween.Sequence()
                .AppendInterval(delayHideTime)
                .AppendCallback(() =>
                {
                    canCompare = false;
                    _tweenShow?.Kill();
                })
                .Append(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack));
            _tweenHide.OnComplete(() =>
            {
                OnHiding();
            });
        }

        internal override void Show()
        {
            _animCorrect?.Kill();
            _tweenHide?.Kill();
            _tweenShow?.Kill();

            gameObject.SetActive(true);

            _tweenShow = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.zero, 0))
                .Append(transform.DOScale(Vector3.one, 0.45f).SetEase(Ease.OutBack));
            _tweenShow.OnComplete(() =>
                {
                    IsShowing = true;
                    Hide();

                    _tweenShow = DOTween.Sequence()
                    .Append(transform.DOScale(Vector3.one * 0.8f, 1).SetEase(Ease.Linear)).SetLoops(-1, LoopType.Yoyo);
                });
        }
        internal void SetupSpecial()
        {
            IsSpecial = true;
            InitSpecial();
        }

        internal void Setup(float delayHideTime)
        {
            Init();

            this.delayHideTime = delayHideTime;
            markedHole.SetActive(false);

            canCompare = true;
        }

        internal override bool IsInside(Vector3 position)
        {
            if (!IsShowing) return false;
            if (!canCompare) return false;

            var distance = (myCollider.bounds.center - position).magnitude;
            var isInside = distance < myLimit;

            return isInside;
        }

        internal override void InitSpecial()
        {
        }
    }
}
