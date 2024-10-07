using AnhNV.Helper;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace WFSport.Gameplay.ArcheryMode
{
    public class MovingMarker : Marker
    {
        [SerializeField]private bool isAutoMoving;
        [SerializeField] Canvas scoreHolder;
        [SerializeField] TMPro.TMP_Text scoreTxt;

        private int myScore;
        private float myLimit;
        private float delayHideTime;
        private float movingTime;
        private bool isInit;
        private bool canCompare;
        private Vector3 beginPos;
        private Vector3 endPos;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tweenShow;
        private Sequence _animCorrect;
        private Tween _tweenHide;
        private float mySpeed;
        private Sequence _animScore;
        private SortingGroup sortingGroup;
        private Bomb myBomb;

        public float MovingTime { get => movingTime; }
        public float Width { get => myCollider.radius * 2; }

        public bool HasBomb { get => myBomb != null && myBomb.IsShowing; }

        private void OnDestroy()
        {
            _animCorrect?.Kill();
            _tweenShow?.Kill();
            _tweenHide?.Kill();
            _animScore?.Kill();
        }
        private void Start()
        {
            Init();
        }
        internal void Setup(Bomb bomb)
        {
            myBomb = bomb;
        }

        internal override void OnHitCorrect(Vector3 position)
        {
            Holder.PlayParticle?.Invoke();

            canCompare = false;
            markedHole.transform.position = position;
            markedHole.SetActive(true);

            var curPos = transform.localPosition;

            scoreHolder.sortingOrder = sortingGroup.sortingOrder + 1;
            scoreHolder.transform.localScale = Vector3.one * 0.01f;
            scoreHolder.gameObject.SetActive(true);
            scoreTxt.text = "+ " + myScore;

            _animScore?.Kill();
            _animScore = DOTween.Sequence()
                .Append(scoreHolder.transform.DOMove(myCollider.bounds.center, 0))
                .Append(scoreHolder.transform.DOMoveY(myCollider.bounds.center.y + 1.5f, 0.5f))
                .Append(scoreHolder.transform.DOShakeScale(0.5f, 1, 2, 90))
                .AppendCallback(() => scoreHolder.gameObject.SetActive(false));

            _tweenShow?.Kill();
            _animCorrect?.Kill();
            _animCorrect = DOTween.Sequence()
                .Append(transform.DOLocalMove(curPos, 0))
                .Append(transform.DOLocalMoveX(curPos.x - 0.1f, 0.1f).SetEase(Ease.Flash))
                .Append(transform.DOLocalMoveX(curPos.x + 0.1f, 0.2f).SetEase(Ease.Flash))
                .Append(transform.DOLocalMoveX(curPos.x, 0.1f).SetEase(Ease.Flash));
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
            markedHole.SetActive(false);
            scoreHolder.gameObject.SetActive(false);
            scoreHolder.worldCamera = Camera.main;

            IsShowing = false;
            canCompare = true;
        }
        private void OnHiding()
        {
            markedHole.SetActive(false);
            gameObject.SetActive(false);
            IsShowing = false;
            OnHidingEvent?.Invoke(this);
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

            IsShowing = true;
            transform.position = beginPos;
            _tweenShow = transform.DOMoveX(endPos.x, mySpeed).SetSpeedBased(true).SetEase(Ease.Linear);
            _tweenShow.OnComplete(() =>
            {
                Hide();
            });
        }

        internal void SetupNext(Vector3 target, float spacing)
        {
            var range = target - transform.position;
            beginPos += Vector3.left * (myRenderer.sprite.rect.width / 100 + spacing) + Vector3.right * range.x;
            transform.position = beginPos;

            movingTime = (endPos - beginPos).x / mySpeed;
        }

        internal void SetupScore(int score)
        {
            myScore = score;
        }
        internal void Setup(float delayHideTime, float yPos, float speed)
        {
            Init();

            this.delayHideTime = delayHideTime;
            markedHole.SetActive(false);

            mySpeed = speed;

            // Calculate X Range Screen
            var point1 = Camera.main.ScreenToWorldPoint(new Vector3(-myRenderer.sprite.rect.width, 0, Camera.main.transform.position.z));
            var point2 = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth + myRenderer.sprite.rect.width, 0, Camera.main.transform.position.z));
            beginPos = new Vector3(point1.x, yPos, transform.position.z);
            endPos = new Vector3(point2.x, yPos, transform.position.z);
            transform.position = beginPos;

            movingTime = (endPos - beginPos).x / speed;

            // Sorting Order Layer
            sortingGroup = GetComponent<SortingGroup>();
            sortingGroup.sortingOrder = Base.Player.SortingLayer(transform.position);

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
