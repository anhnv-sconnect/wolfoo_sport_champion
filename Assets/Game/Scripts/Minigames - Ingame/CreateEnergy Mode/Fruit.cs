using DG.Tweening;
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

        internal void JumpTo(Vector3 endPos)
        {
            jumpAnim = DOTween.Sequence()
                .Append(transform.DOJump(endPos, 3, 1, 0.5f));
            jumpAnim.OnComplete(() =>
            {
                EventManager.OnFruitJumpIn?.Invoke(this);
            });
        }
    }
}
