using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.SnowballMode
{
    public class Player : Base.Player
    {
        [SerializeField] private GameplayConfig config;
        [SerializeField] private CharacterWorldAnimation wolfoo;
        [SerializeField] private Transform ball;

        private IMinigame.GameState gamestate;
        private Vector3 lastTouch;
        private Camera cam;
        private Vector3 camPos;

        private float ballRadius;
        private Vector3 maxBallScale;
        private Vector3 initBallScale;
        private Vector3 initBallPos;
        private Vector3 maxBallRange;
        private float ballValue;

        private bool isReadyToReleaseBall;
        private bool candrag;
        private Sequence throwAnim;

        protected override IMinigame.GameState GameplayState { get => gamestate; set => gamestate = value; }

        #region UNITY METHODS
        // Start is called before the first frame update
        void Start()
        {
            GameplayState = IMinigame.GameState.Playing;
            Init();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag(TAG.SNOW))
            {
                if (ballValue >= config.maxSize)
                {
                    isReadyToReleaseBall = true;
                    return;
                }
                ballValue += Time.deltaTime * 10 * config.growthSpeed;
                ScaleBall();
            }

            if(collision.CompareTag(TAG.STAGE))
            {
                if(isReadyToReleaseBall)
                {
                    var stage = collision.GetComponent<SnowmanStage>();
                    if (stage == null) return;
                    ReleaseBall(stage.GetNextSnowballPos);
                }
            }
        }
        #endregion

        #region METHODS
        internal void ReleaseBall(Vector3 position)
        {
            candrag = false;

            throwAnim = DOTween.Sequence()
                .AppendCallback(() => wolfoo.PlayThrowAnim(false))
                .AppendInterval(0.5f)
                .Append(ball.DOJump(position, 1, 1, 0.5f))
                .Join(ball.DOScale(Vector2.one * 0.8f, 0.5f));

            throwAnim.OnComplete(() =>
            {
                ResetBall();
            });

        }
        private void ResetBall()
        {
            ballValue = 0;
            transform.localScale = initBallScale;
            transform.localPosition = initBallPos;
        }
        private void ScaleBall()
        {
            var value = ballValue / config.maxSize;
            ball.localScale = Vector2.Lerp(initBallScale, maxBallScale, value);
            ball.localPosition = Vector2.Lerp(initBallPos, maxBallRange, value);
        }
        #endregion

        #region OVERRIDE METHODS
        protected override void OnBeginDrag()
        {
            base.OnBeginDrag();

            if (!candrag) return;
            wolfoo.PlayPushAnim();
        }
        protected override void OnEndDrag()
        {
            base.OnEndDrag();
            wolfoo.PlayIdleAnim();
        }

        public override void Init()
        {
            cam = Camera.main;
            camPos = cam.transform.position;

            ballRadius = GetComponentInChildren<CircleCollider2D>().radius;
            initBallScale = ball.localScale;
            initBallPos = ball.localPosition;

            var maxScale = 2;
            maxBallScale = initBallScale * maxScale;
            maxBallRange = initBallPos + Vector3.one * ballRadius / 2 * maxScale;
        }

        public override void Lose()
        {
        }

        public override void OnDragging(Vector3 force)
        {
            if (!candrag) return;

            var range = transform.position.x - lastTouch.x;
            if (range * range > 3)
            {
                transform.rotation = Quaternion.Euler(Vector3.up * (range > 0 ? 180 : 0));
            }

            if (transform.position != lastTouch)
            {
                transform.position = Vector2.Lerp(transform.position, lastTouch, config.velocity);
            }

            ball.Rotate( Vector3.back * config.rotateVelocity);
        }

        public override void OnSwipe()
        {
        }

        public override void OnTouching(Vector3 position)
        {
            lastTouch = position;
        }

        public override void OnUpdate()
        {
            if (GameplayState != IMinigame.GameState.Playing) return;
            cam.transform.position = new Vector3(transform.position.x, transform.position.y, camPos.z);
        }

        public override void Pause(bool isSystem)
        {
        }

        public override void Play()
        {
        }

        public override void ResetDefault()
        {
        }

        #endregion

    }
}
