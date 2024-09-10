using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.BasketballMode
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] BoneFollower playerHandBone;
        private Vector3 targetPos;
        private bool isFlying;
        private Sequence tweenFlying;
        private bool isTriggerWithObstacle;
        private Vector2 screenPosRange;
        private Vector2 screenPixelSize;
        private Transform initParent;
        private Vector3 initPos;
        private float radius;
        private float basketHeight;
        private float rotateSpeed;
        private bool isInsideBasket;
        private float flyTime;
        private float scaleRange;
        private float flyingPower;

        private void Start()
        {
            radius = GetComponent<CircleCollider2D>().radius;
            screenPosRange = ScreenHelper.GetMaxPosition();
            screenPixelSize = ScreenHelper.GetMaxPizelSize();

        }
        private void OnDestroy()
        {
            tweenFlying?.Kill();
        }
        private void Update()
        {
            if(isFlying)
            {
                if (!isInsideBasket)
                {
                    transform.Rotate(Vector3.forward * rotateSpeed);
                    var yRange = Camera.main.WorldToScreenPoint(transform.position);
                    transform.localScale = Vector3.one -  Vector3.one * scaleRange * (yRange.y / screenPixelSize.y);
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

        internal void Setup(GameplayConfig config)
        {
            basketHeight = config.height;
            rotateSpeed = config.rotateSpeed;
            flyingPower = config.flyingPower;
            flyTime = config.flyTime;
            scaleRange = config.scaleRange;

            isFlying = false;

            if (initParent == null)
            {
                initParent = transform.parent;
                initPos = transform.position;
            }
            else
            {
                transform.parent = initParent;
                transform.position = initPos;
            }
        }

        internal void Show()
        {
            gameObject.SetActive(true);
            playerHandBone.enabled = true;
        }
        internal void Hide()
        {
            Hiding();
        }

        private void Hiding()
        {
            gameObject.SetActive(false);
            playerHandBone.enabled = true;
        }

        internal void FlyTo(Vector3 endPos, Transform basket)
        {
            isFlying = true;
            isInsideBasket = false;
            targetPos = endPos;
            var isBasket = basket != null;
            var throwDirection = targetPos - transform.position;

            tweenFlying = DOTween.Sequence()
                .Append(transform.DOJump(targetPos, flyingPower, 1, flyTime).SetEase(Ease.Flash)
                .OnStart(() => { playerHandBone.enabled = false; }));

            if (isBasket)
            {
                tweenFlying
                    .AppendCallback(() =>
                    {
                        isInsideBasket = true;
                        transform.SetParent(basket);
                    })
                    .Append(transform.DOLocalMoveY(transform.position.y - basketHeight, 0.5f));
            }
            tweenFlying.OnComplete(() =>
            {
                if (isTriggerWithObstacle && !isInsideBasket) // => Boucing to Outside
                {
                    // Calculate Angle -> 60 degrees
                    var xPos = throwDirection.x * 3;
                    var yPos = -screenPosRange.y - radius * 2;
                    var outSidePos = new Vector3(xPos, yPos, 0);
                    tweenFlying = DOTween.Sequence()
                        .Append(transform.DOJump(outSidePos, flyingPower, 1, 1));
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
