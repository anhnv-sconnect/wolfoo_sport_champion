using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WFSport.Helper;

namespace WFSport.Gameplay.CreateEnergyMode
{
    public class Blender : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string pouringAnimName;
        [SerializeField] private string grindAnimName;
        [SerializeField] private Transform fruitHolder;
        [SerializeField] private Transform lid;
        [SerializeField] private Button powerBtn;
        [SerializeField] private Transform cup;
        [SerializeField] private Transform pourArea;

        private Vector3 startPos;
        private float outSideX;
        private Vector3 initLidPos;
        private int countPos = -2;
        private int maxCount = 2;
        private float grindingAnimTime = 2;
        private float pouringAnimTime = 2;
        private bool canGrinding;

        private Canvas canvas;
        private List<Fruit> myFruits;

        private Sequence powerBtnAnim;
        private Sequence animShaking;
        private Sequence animMoveOut;
        private Sequence lidAnim;

        public Action OnGrindingComplete { get; private set; }

        public (Vector3 position, Transform holder) FruitArea
        { 
            get
            {
                countPos++;
                if (countPos >= maxCount) countPos = -1;
                return (fruitHolder.position + Vector3.right * countPos * 0.5f, fruitHolder);
            }
        }

        private void Start()
        {
            EventManager.OnFruitJumpIn += OnFruitJumpIn;
            powerBtn.onClick.AddListener(OnClickPower);
        }

        private void OnDestroy()
        {
            EventManager.OnFruitJumpIn -= OnFruitJumpIn;
            animShaking?.Kill();
            animMoveOut?.Kill();
            lidAnim?.Kill();
            powerBtnAnim?.Kill();
        }

        internal void Setup()
        {
            startPos = transform.position;
            outSideX = ScreenHelper.GetMaxPosition().x + 5;
            initLidPos = lid.localPosition;
            canvas = GetComponentInChildren<Canvas>();
            canvas.worldCamera = Camera.main;
        }

        private void OnClickPower()
        {
            if (!canGrinding) return;
            canGrinding = false;
            StopGrindTutorial();
            OnGrinding();
        }

        internal void MoveOut()
        {
            lid.localPosition = initLidPos;
            animMoveOut = DOTween.Sequence()
                .Append(transform.DOMoveX(outSideX, 0.5f));
        }

        private void OnFruitJumpIn(Fruit fruit)
        {
            if (myFruits == null) myFruits = new List<Fruit>();
            animShaking?.Kill();
            transform.position = startPos;
            animShaking = DOTween.Sequence()
                .Append(transform.DOPunchPosition(Vector3.one * 0.1f, 0.5f, 1));
            myFruits.Add(fruit);
        }

        internal void OpenLid(Action OnComplete)
        {
            lidAnim?.Kill();
            lidAnim = DOTween.Sequence()
                .Append(lid.DOJump(new Vector3(-outSideX - 2.5f, lid.position.y, 0), 0.5f, 1, 0.5f));
            lidAnim.OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        }
        internal void CloseLid(Action OnComplete)
        {
            lidAnim?.Kill();
            lidAnim = DOTween.Sequence()
                .Append(lid.DOLocalJump(initLidPos, 0.5f, 1, 0.5f));
            lidAnim.OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        }
        private void OnGrinding()
        {
            animator.Play(grindAnimName, 0, 0);
            var loop = grindingAnimTime / 0.5f;
            animShaking?.Kill();
            foreach (var fruit in myFruits) { fruit.Dancing(); }
            animShaking = DOTween.Sequence()
                .Append(transform.DOPunchPosition(Vector3.one * 0.1f, 0.5f, 1).SetLoops((int)loop, LoopType.Restart))
                .AppendCallback(() => {
                    foreach (var fruit in myFruits)
                    {
                        fruit.Release();
                    }
                })
                .AppendInterval(grindingAnimTime);
            animShaking.OnComplete(() =>
            {
                OpenLid(() =>
                {
                    OnGrindingComplete?.Invoke();
                });
            });
        }
        internal void StopGrindTutorial()
        {
            powerBtnAnim?.Kill();
            powerBtn.transform.localScale = Vector3.one;
        }
        internal void PlayGrindTutorial()
        {
            powerBtnAnim?.Kill();
            powerBtnAnim = DOTween.Sequence()
                .Append(powerBtn.transform.DOScale(Vector3.one, 0))
                .Append(powerBtn.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f))
                .AppendInterval(1)
                .SetLoops(-1, LoopType.Restart);
        }
        internal void Grind(System.Action OnComplete)
        {
            CloseLid(() =>
            {
                PlayGrindTutorial();
                canGrinding = true;
                OnGrindingComplete = OnComplete;
            });
        }
        internal void Pouring(Vector3 endPos, System.Action OnPouring, System.Action OnComplete)
        {
            endPos = new Vector3(0.4f, 1.384f);
            animShaking?.Kill();
            var cupBeginPos = cup.position;
            animShaking = DOTween.Sequence()
                .Append(cup.DOMove(endPos, 0.5f))
                .AppendCallback(() =>
                {
                    animator.Play(pouringAnimName, 0, 0);
                })
                .AppendInterval(1.25f)
                .Append(DOVirtual.Float(0, 1, pouringAnimTime, (value) => { OnPouring?.Invoke(); }))
                .Append(cup.DOMove(cupBeginPos, 0.5f));
            animShaking.OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        }
    }
}
