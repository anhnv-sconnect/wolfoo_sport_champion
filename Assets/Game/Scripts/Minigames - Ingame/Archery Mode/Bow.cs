using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.ArcheryMode
{
    public class Bow : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Sprite idleSprite;
        [SerializeField] Sprite drawSprite;
        private Sequence _tweenShoot;
        private float drawingTime;

        private void OnDestroy()
        {
            _tweenShoot?.Kill();
        }

        internal void Setup(float drawingTime)
        {
            this.drawingTime = drawingTime;
        }
        private void Rotate(Vector3 target)
        {
            var direction = target - transform.position;
            var angle = Mathf.Atan(direction.x / direction.y) * -1 * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(Vector3.forward * angle);
        }
        internal void Shoot(Vector3 endPos)
        {
            Rotate(endPos);
            spriteRenderer.sprite = drawSprite;
            _tweenShoot?.Kill();
            _tweenShoot = DOTween.Sequence()
                .Append(DOVirtual.DelayedCall(drawingTime, () =>
                {
                    spriteRenderer.sprite = idleSprite;
                }));
        }
    }
}
