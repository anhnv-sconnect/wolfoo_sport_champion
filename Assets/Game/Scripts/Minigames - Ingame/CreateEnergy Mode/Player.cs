using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Helper;

namespace WFSport.Gameplay.CreateEnergyMode
{
    public class Player : MonoBehaviour
    {
        [SerializeField] Transform mouth;
        [SerializeField] CharacterWorldAnimation characterAnim;
        private Sequence moveAnim;
        private Sequence drinkAnim;
        private bool isInit;
        private Vector3 initPos;
        private Vector2 maxPos;

        public Vector3 MouthPos { get => mouth.position; }
        private Action OnDrinkComplete;

        private void OnDestroy()
        {
            moveAnim?.Kill();
            drinkAnim?.Kill();
        }
        private void Start()
        {
            MyInit();
            MoveOut(null, true);
        }
        private void MyInit()
        {
            if (isInit) return;
            isInit = true;

            characterAnim.PlayIdleAnim();
            initPos = transform.position;
            maxPos = ScreenHelper.GetMaxPosition() + Vector2.right * 5;
        }

        internal void Drink(System.Action OnComplete = null)
        {
            characterAnim.PlayIdleAnim();
            characterAnim.PlayEatAnim(false);

            drinkAnim?.Complete();
            OnDrinkComplete = OnComplete;
            drinkAnim = DOTween.Sequence()
                .AppendInterval(characterAnim.GetTimeAnimation(CharacterWorldAnimation.AnimState.Eat));
            drinkAnim.OnComplete(() =>
            {
                OnDrinkComplete?.Invoke();
            });
        }
        internal void PlayWining()
        {
            characterAnim.PlayJumpWinAnim(true);
        }

        internal void MoveOut(Action OnComplete = null, bool isImmediately = false)
        {
            moveAnim?.Kill();
            if (isImmediately)
            {
                transform.position = new Vector3(-(maxPos.x), initPos.y, initPos.z);
                OnComplete?.Invoke();
                return;
            }
            characterAnim.PlayRunAnim();
            moveAnim = DOTween.Sequence()
                .Append(transform.DOMoveX(-(maxPos.x), 0.5f).SetEase(Ease.InBack));
            moveAnim.OnComplete(() =>
            {
                characterAnim.PlayIdleAnim();
                OnComplete?.Invoke();
            });
        }
        internal void MoveIn(Action OnComplete = null)
        {
            moveAnim?.Kill();
            characterAnim.PlayRunAnim();
            moveAnim = DOTween.Sequence()
                .Append(transform.DOMoveX(initPos.x, 0.5f).SetEase(Ease.OutBack));
            moveAnim.OnComplete(() =>
            {
                characterAnim.PlayIdleAnim();
                OnComplete?.Invoke();
            });
        }
    }
}
