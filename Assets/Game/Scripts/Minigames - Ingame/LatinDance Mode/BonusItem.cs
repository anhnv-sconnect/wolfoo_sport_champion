using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.LatinDanceMode
{
    public class BonusItem : MonoBehaviour
    {
        [SerializeField] Sprite exploreSprite;

        private Vector3 initialPos;
        private Sprite idleSprite;
        private SpriteRenderer spriteRender;
        private Sequence _sequence;
        private TweenerCore<Vector3, Vector3, VectorOptions> tweenAnim;

        private void OnDestroy()
        {
            tweenAnim?.Kill();
            _sequence?.Kill();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(TAG.PLAYER))
            {
                OnKill();
            }
        }
        private void Init()
        {
            if(spriteRender == null) spriteRender = GetComponent<SpriteRenderer>();
            initialPos = transform.localPosition;
            idleSprite = spriteRender.sprite;
            // Turn off Particle
        }
        private void PlayIdleAnim()
        {
            tweenAnim = transform.DOLocalMoveY(initialPos.y + 0.5f, 1).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
        private void OnKill()
        {
            _sequence?.Kill();
            tweenAnim?.Kill();

            Holder.PlaySound?.Invoke();
            _sequence = DOTween.Sequence()
                .AppendCallback(() => spriteRender.sprite = exploreSprite)
                .Append(transform.DOScale(2, 0.25f).OnStart(() =>
                {
                    // Play Particle
                }))
                .AppendCallback(() =>
                {
                    gameObject.SetActive(false);
                    EventManager.OnHide?.Invoke(this);
                });
        }
        internal void Setup(Sprite sprite, Vector2 position)
        {
            transform.position = position;
            if(spriteRender == null) spriteRender = GetComponent<SpriteRenderer>();
            spriteRender.sprite = sprite;

            Init();
        }
        internal void Hide()
        {
            gameObject.SetActive(false);
        }
        internal void Spawn()
        {
            Holder.PlaySound?.Invoke();
            gameObject.SetActive(true);
            _sequence = DOTween.Sequence()
                .Append(transform.DOScale(1, 0.5f))
                .AppendCallback(() =>
                {
                    PlayIdleAnim();
                });
        }
    }
}
