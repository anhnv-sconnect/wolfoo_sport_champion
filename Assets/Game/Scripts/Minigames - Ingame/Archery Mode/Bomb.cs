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

        private Tween _tweenHide;
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
            canCompare = false;
            _tweenHide = DOVirtual.DelayedCall(delayHideTime, () =>
            {
                OnHiding();
            });
        }

        internal override void Show()
        {
            _animCorrect?.Kill();
            _tweenHide?.Kill();

            gameObject.SetActive(true);
            _tweenShow = DOTween.Sequence()
                .Append(transform.DORotate(initRotaton.eulerAngles, 0))
                .Append(transform.DOLocalMoveY(initLocalPos.y + 1, 0))
                .Append(transform.DOLocalMoveY(initLocalPos.y, 0.2f).SetEase(Ease.Flash))
                .Append(transform.DORotate(Vector3.forward * -2, 0.1f))
                .Append(transform.DORotate(Vector3.forward * 2, 0.2f))
                .Append(transform.DORotate(initRotaton.eulerAngles, 0.1f));
            IsShowing = true;
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
