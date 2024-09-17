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
        [SerializeField] CharacterWorldAnimation characterAnim;
        private Sequence moveAnim;
        private bool isInit;
        private Vector3 initPos;
        private Vector2 maxPos;

        private void OnDestroy()
        {
            moveAnim?.Kill();
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
            maxPos = ScreenHelper.GetMaxPosition();
        }

        internal void Drink()
        {
            Debug.Log("Player Drinking");
            // Character play Eating
        }

        internal void MoveOut(Action OnComplete = null, bool isImmediately = false)
        {
            moveAnim?.Kill();
            if (isImmediately)
            {
                transform.position = new Vector3(-(maxPos.x + 2), initPos.x, initPos.z);
                OnComplete?.Invoke();
                return;
            }
            characterAnim.PlayRunAnim();
            moveAnim = DOTween.Sequence()
                .Append(transform.DOMoveX(-(maxPos.x + 2), 0.5f).SetEase(Ease.InBack));
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
