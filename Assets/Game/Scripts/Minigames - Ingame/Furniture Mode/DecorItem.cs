using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.FurnitureMode
{
    public abstract class DecorItem : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;
        private Sequence animReplace;

        public Topic.Kind TopicKind { get; private set; }

        public abstract void Replace(Sprite icon);
        protected virtual void OnDestroy()
        {
            animReplace?.Kill();
        }
        protected void PlayAnimReplace(Sprite sprite)
        {
            animReplace?.Complete();
            var curPos = transform.position;
            animReplace = DOTween.Sequence()
                .Append(transform.DOScale(0, 0))
                .Join(transform.DOMoveY(curPos.y + 0.1f, 0))
                .AppendCallback(() => { spriteRenderer.sprite = sprite; })
                .Append(transform.DOScale(1, 0.3f).SetEase(Ease.OutBack))
                .Join(transform.DOLocalMoveY(curPos.y - 0.1f, 0.5f).SetEase(Ease.OutBounce));
        }
        public void Setup(Sprite icon, Topic.Kind kind)
        {
            spriteRenderer.sprite = icon;
            TopicKind = kind;
        }
        public void Spawn(Vector2 pos)
        {
            spriteRenderer.sortingOrder = Base.Player.SortingLayer(pos);
            animReplace?.Kill();
            animReplace = DOTween.Sequence()
                .Append(transform.DOMoveY(pos.y + 0.2f, 0))
                .Join(transform.DORotate(Vector3.forward * 5, 0))
                .Append(transform.DOMoveY(pos.y, 0.5f).SetEase(Ease.OutBounce))
                .Join(transform.DORotate(Vector3.zero, 1).SetEase(Ease.OutBack));
        }
        public void Hide()
        {
            spriteRenderer.DOFade(0.5f, 0);
        }
        public void Show()
        {
            spriteRenderer.DOFade(1, 0);
        }
    }
}
