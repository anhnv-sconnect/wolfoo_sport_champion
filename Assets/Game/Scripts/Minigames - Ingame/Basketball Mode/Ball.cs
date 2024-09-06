using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.BasketballMode
{
    public class Ball : MonoBehaviour
    {
        private Vector3 targetPos;
        private bool isFlying;
        private Sequence tweenFlying;
       [SerializeField] private bool isTriggerWithObstacle;
        private Vector2 screenPosRange;
        private float radius;
        private float basketHeight;
        private GameplayConfig config;
        private bool isInsideBasket;

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
                if (!isInsideBasket)
                {
                    transform.Rotate(Vector3.forward * config.rotateSpeed);
                }
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

        internal void Setup(float basketHeight, GameplayConfig config)
        {
            this.basketHeight = basketHeight;
            this.config = config;
        }

        private void Hiding()
        {
            gameObject.SetActive(false);
        }

        internal void FlyTo(Vector3 endPos, Transform basket)
        {
            isFlying = true;
            isInsideBasket = false;
            targetPos = endPos;
            var isBasket = basket != null;
            var throwDirection = targetPos - transform.position;

            tweenFlying = DOTween.Sequence()
                .Append(transform.DOJump(targetPos, config.flyingPower, 1, config.flyTime).SetEase(Ease.Flash));

            if (isBasket)
            {
                tweenFlying
                    .AppendCallback(() =>
                    {
                        isInsideBasket = true;
                        transform.SetParent(basket);
                    })
                    .Append(transform.DOLocalMoveY(transform.position.y - basketHeight, 1));
            }
            tweenFlying.OnComplete(() =>
            {
                Debug.Log("1");
                if (isTriggerWithObstacle) // => Boucing to Outside
                {
                    Debug.Log("2");
                    var xPos = throwDirection.x < 0 ? -screenPosRange.x - radius * 2 : screenPosRange.x + radius * 2;
                    var yPos = -throwDirection.y / 2;
                    var outSidePos = new Vector3(xPos, yPos, 0);
                    tweenFlying = DOTween.Sequence()
                        .Append(transform.DOJump(outSidePos, config.flyingPower, 1, 1));
                    tweenFlying.OnComplete(() =>
                    {
                        Hiding();
                        isFlying = false;
                    });
                }
                else
                {
                    Hiding();
                    isFlying = false;
                }
            });
        }
    }
}
