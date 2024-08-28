using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.ArcheryMode
{
    public class Marker : MonoBehaviour
    {
        [SerializeField] GameObject markedHole;
        [SerializeField] CircleCollider2D myCollider;
        [SerializeField] SpriteRenderer myRenderer;

        private float myLimit;
        private float delayHideTime;
        private Vector3 initLocalPos;
        private Quaternion initRotaton;
        private bool isInit;
        private bool canCompare;

        private Tween _tweenHide;
        private Sequence _tweenShow;
        private Sequence _animCorrect;

        public bool IsShowing { get; private set; }

        private void Start()
        {
            myLimit = myCollider.radius;
            Init();
        }
        private void OnDestroy()
        {
            _animCorrect?.Kill();
            _tweenShow?.Kill();
            _tweenHide?.Kill();
        }
        private void Init()
        {
            if (isInit) return;
            isInit = true;

            initLocalPos = transform.localPosition;
            initRotaton = transform.rotation;

            markedHole.SetActive(false);
            gameObject.SetActive(false);

            IsShowing = false;
            canCompare = true;
        }
        internal void Setup(float delayHideTime)
        {
            Init();

            this.delayHideTime = delayHideTime;
            markedHole.SetActive(false);

            canCompare = true;
        }
        internal void Show()
        {
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
        internal void Hide()
        {
            canCompare = false;
            _tweenHide = DOVirtual.DelayedCall(delayHideTime, () =>
            {
                markedHole.SetActive(false);
                gameObject.SetActive(false);
                IsShowing = false;
            });
        }
        private void OnHitCorrect(Vector3 position)
        {
            Holder.PlayParticle?.Invoke();

            markedHole.transform.position = position;
            markedHole.SetActive(true);

            _animCorrect?.Kill();
            _animCorrect = DOTween.Sequence()
                .Append(transform.DOLocalMove(initLocalPos, 0))
                .Append(transform.DOLocalMoveX(initLocalPos.x - 0.1f, 0.1f).SetEase(Ease.Flash))
                .Append(transform.DOLocalMoveX(initLocalPos.x + 0.1f, 0.2f).SetEase(Ease.Flash))
                .Append(transform.DOLocalMoveX(initLocalPos.x, 0.1f).SetEase(Ease.Flash));

        }
        internal bool IsInside(Vector3 position)
        {
            if (!IsShowing) return false;
            if (!canCompare) return false;

            var distance = (myCollider.bounds.center - position).magnitude;
            var isInside = distance < myLimit;
            if (isInside)
            {
                canCompare = false;
                OnHitCorrect(position);
            }

            return isInside;
        }
    }
}
