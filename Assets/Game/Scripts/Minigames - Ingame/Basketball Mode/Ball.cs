using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.BasketballMode
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] float flyTime;
        [SerializeField] float rotateSpeed;
        [SerializeField] float flyingPower = 2;
        private Vector3 targetPos;
        private bool isFlying;
        private Sequence tweenFlying;
        private bool isTriggerWithObstacle;
        private Vector2 screenPosRange;
        private float radius;
        private float basketHeight;

        private void Start()
        {
            radius = GetComponent<CircleCollider2D>().radius;

            var maxXPos = Camera.main.ScreenToWorldPoint(Vector3.right * Camera.main.rect.xMax);
            var maxYPos = Camera.main.ScreenToWorldPoint(Vector3.right * Camera.main.rect.yMax);
            screenPosRange = new Vector2(maxXPos.x, maxYPos.y);
        }
        private void Update()
        {
            if(isFlying)
            {
                transform.Rotate(Vector3.forward * rotateSpeed);
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag(TAG.OBSTACLE))
            {
                isTriggerWithObstacle = true;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(TAG.OBSTACLE))
            {
                isTriggerWithObstacle = false;
            }
        }

        internal void Setup(float basketHeight)
        {
            this.basketHeight = basketHeight;
        }

        private void Hiding()
        {
            gameObject.SetActive(false);
        }

        internal void FlyTo(Vector3 endPos, Transform basket)
        {
            isFlying = true;
            targetPos = endPos;
            var isBasket = basket != null;
            var throwDirection = targetPos - transform.position;

            tweenFlying = DOTween.Sequence()
                .Append(transform.DOJump(targetPos, flyingPower, 1, flyTime).SetEase(Ease.Flash));

            if (isBasket)
            {
                tweenFlying
                    .AppendCallback(() => transform.SetParent(basket))
                    .Append(transform.DOLocalMoveY(targetPos.y - basketHeight, 0.5f));
            }
            else
            {
                if (isTriggerWithObstacle) // => Boucing to Outside
                {
                    var xPos = throwDirection.x < 0 ? -screenPosRange.x - radius * 2 : screenPosRange.x + radius * 2;
                    var yPos = -throwDirection.y / 2;
                    var outSidePos = new Vector3(xPos, yPos, 0);
                    tweenFlying.Append(transform.DOJump(outSidePos, flyingPower, 1, 1));
                }
            }
            tweenFlying.OnComplete(() =>
            {
                Hiding();
                isFlying = false;
            });
        }
    }
}
