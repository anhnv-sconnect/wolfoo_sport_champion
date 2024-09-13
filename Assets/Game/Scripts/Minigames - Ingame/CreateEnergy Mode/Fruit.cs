using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CreateEnergyMode
{
    public class Fruit : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        private Sequence jumpAnim;

        internal void Setup(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
        private void OnDestroy()
        {
            jumpAnim?.Kill();
        }

        internal void JumpTo(Vector3 endPos, Transform endParent, Action OnComplete)
        {
            jumpAnim = DOTween.Sequence()
                .Append(transform.DOJump(endPos, 3, 1, 0.5f));
            jumpAnim.OnComplete(() =>
            {
                EventManager.OnFruitJumpIn?.Invoke(this);
                OnComplete?.Invoke();
                transform.SetParent(endParent);
            });
        }

        internal void Dancing()
        {
            var rd = UnityEngine.Random.Range(0.5f, 2f);
            jumpAnim?.Kill();
            jumpAnim = DOTween.Sequence()
                .AppendInterval(rd)
                .Append(transform.DOLocalJump(transform.localPosition + Vector3.up * 1, 3, 1, 0.5f).SetLoops(-1, LoopType.Yoyo));
        }

        internal void Release()
        {
            jumpAnim?.Kill();
            gameObject.SetActive(false);
        }
    }
}
