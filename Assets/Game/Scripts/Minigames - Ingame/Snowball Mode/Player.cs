using AnhNV.Helper;
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
        [SerializeField] private Transform ball;
        [SerializeField] private Transform stageSignal;
        [SerializeField] private Transform stageArea;
        [SerializeField] private Transform arrow;
        [SerializeField] private Transform rotationHolder;

        private CharacterWorldAnimation wolfoo;
        private IMinigame.GameState gamestate;
        private Vector3 lastTouch;
        private Camera cam;
        private Vector3 camPos;

        private float ballRadius;
        private Vector3 maxBallScale;
        private Vector3 initBallScale;
        private Vector3 initBallPos;
        private Vector3 initSignalScale;
        private Vector3 maxBallRange;
        private float arrowLength;
        private float ballValue;

        private bool isReadyToReleaseBall;
        private bool candrag;
        private bool canMoveCam;
        private bool arrowIsShowing;
        private bool arrowIsShown;
        private Sequence _tweenArrow;
        private Sequence _throwAnim;
        private bool isTrigglingWithSnow;

        protected override IMinigame.GameState GameplayState { get => gamestate; set => gamestate = value; }

        #region UNITY METHODS
        // Start is called before the first frame update
        void Start()
        {
            Init();
        }
        private void OnDestroy()
        {
            _tweenArrow?.Kill();
            _throwAnim?.Kill();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag(TAG.SNOW))
            {
                isTrigglingWithSnow = true;
            Holder.PlaySound?.Invoke();
            }

            if (collision.CompareTag(TAG.STAGE))
            {
                if(isReadyToReleaseBall)
                {
                    Pause(false);
                    isReadyToReleaseBall = false;

                    var stage = collision.GetComponent<SnowmanStage>();
                    if (stage == null) return;

                    Holder.PlaySound?.Invoke();
                    var emptyPosInStage = stage.GetNextSnowballEmpty;
                    var camBeginPos = cam.transform.position;
                    _throwAnim = DOTween.Sequence()
                        .Append(cam.transform.DOMove(
                            new Vector3(stage.transform.position.x, stage.transform.position.y, cam.transform.position.z),
                            0.5f));
                    _throwAnim.OnComplete(() =>
                    {
                        ReleaseBall(emptyPosInStage.position, () =>
                        {
                            stage.BuildNextSnowball(() =>
                            {
                                // PLay Game
                                _throwAnim = DOTween.Sequence()
                                    .Append(cam.transform.DOMove(camBeginPos, 0.5f).SetEase(Ease.Linear));
                                _throwAnim.OnComplete(() =>
                                {
                                    EventManager.OnFinishStage?.Invoke();
                                  //  Play();
                                });
                            });
                        });
                    });
                    HideArrow();
                }
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(TAG.SNOW))
            {
                isTrigglingWithSnow = false;
            Holder.PlaySound?.Invoke();
            }
        }
        #endregion

        #region METHODS
        internal void Setup(CharacterWorldAnimation wolfoo)
        {
            this.wolfoo = wolfoo;
            wolfoo.transform.SetParent(rotationHolder);
            wolfoo.transform.localPosition = Vector3.zero;
            wolfoo.ChangeSkin(CharacterWorldAnimation.SkinType.Christmas);
            wolfoo.PlayIdleAnim();
        }
        internal void ReleaseBall(Vector3 position, System.Action OnCompleted)
        {
            candrag = false;
            canMoveCam = false;

            _throwAnim = DOTween.Sequence()
                .AppendCallback(() => wolfoo.PlayThrowAnim(false))
                .AppendInterval(0.7f)
                .Append(ball.DOJump(position, 1, 1, 0.5f))
                .Join(ball.DOScale(Vector2.one * 0.8f, 0.5f));

            _throwAnim.OnComplete(() =>
            {
                wolfoo.PlayJumpWinAnim();
                OnCompleted?.Invoke();
                ResetBall();
            });

        }
        private void ResetBall()
        {
            ballValue = 0;
            ball.transform.localScale = initBallScale;
            ball.transform.localPosition = initBallPos;
            isReadyToReleaseBall = false;
        }
        private void OnScalingBall()
        {
            var value = ballValue / config.maxSize;
            ball.localScale = Vector2.Lerp(initBallScale, maxBallScale, value);
            ball.localPosition = Vector2.Lerp(initBallPos, maxBallRange, value);
        }
        private void ScaleBall()
        {
            if (ballValue >= config.maxSize)
            {
                isReadyToReleaseBall = true;
                return;
            }
            ballValue += Time.deltaTime * 10 * config.growthSpeed;
            OnScalingBall();
        }
        private void HideArrow(bool isImmediately = false)
        {
            if(isImmediately)
            {
                stageSignal.transform.localScale = Vector3.zero;
                arrowIsShowing = false;
                return;
            }

            _tweenArrow?.Kill();
            _tweenArrow = DOTween.Sequence()
               .Append(stageSignal.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack));
            _tweenArrow.OnComplete(() =>
            {
                arrowIsShown = false;
                arrowIsShowing = false;
            });
        }
        private void ShowArrow()
        {
            if (arrowIsShowing) return;
            arrowIsShowing = true;

            Holder.PlaySound?.Invoke();
            _tweenArrow?.Kill();
            _tweenArrow = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    stageSignal.localScale = Vector3.zero;
                    arrow.rotation = Quaternion.Euler(Vector3.zero);
                })
               .Append(stageSignal.DOScale(initSignalScale, 0.5f).SetEase(Ease.OutBack))
               .Join(DOVirtual.Float(0, 8, 1f, (angle) =>
               {
                   var x = Mathf.Cos(angle);
                   var y = Mathf.Sin(angle);
                   var direction = new Vector3(x, y, 0);
                   arrow.localPosition = direction * arrowLength;

                   angle = y * Mathf.Rad2Deg;
                   arrow.localRotation = Quaternion.Euler(Vector3.forward * -angle);
               }).SetEase(Ease.Linear));
            _tweenArrow.OnComplete(() =>
            {
                _tweenArrow = DOTween.Sequence()
                .Append(stageSignal.DOPunchScale(Vector3.one * 0.1f, 0.5f, 2).SetEase(Ease.InOutBack).SetLoops(-1, LoopType.Restart));
                arrowIsShown = true;
            });
        }
        private void ArrowRotatingTo(Vector3 endPos)
        {
            var direction = stageSignal.position - endPos;
            float angle = Mathf.Asin(direction.x / direction.magnitude) * Mathf.Rad2Deg;
            if (direction.y > 0) angle = 180 - angle;
            arrow.localPosition = direction.normalized * -1 * arrowLength;
            arrow.rotation = Quaternion.Euler(Vector3.forward * angle);
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

            if (!candrag) return;
            wolfoo.PlayIdleAnim();
        }

        public override void Init()
        {
            cam = Camera.main;
            camPos = cam.transform.position;

            ballRadius = GetComponentInChildren<CircleCollider2D>().radius;
            initBallScale = ball.localScale;
            initBallPos = ball.localPosition;
            initSignalScale = stageSignal.localScale;

            var maxScale = 2;
            maxBallScale = initBallScale * maxScale;
            maxBallRange = initBallPos + Vector3.one * ballRadius / 2 * maxScale;

            arrowLength = Vector2.Distance(arrow.position, stageSignal.position) / 2;
            HideArrow(true);
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
                rotationHolder.rotation = Quaternion.Euler(Vector3.up * (range > 0 ? 180 : 0));
            }

            if (transform.position != lastTouch)
            {
                lastTouch.x = lastTouch.x < config.limitPosition.x ? config.limitPosition.x : lastTouch.x;
                lastTouch.y = lastTouch.y < config.limitPosition.w ? config.limitPosition.w : lastTouch.y;
                lastTouch.x = lastTouch.x > config.limitPosition.z ? config.limitPosition.z : lastTouch.x;
                lastTouch.y = lastTouch.y > config.limitPosition.y ? config.limitPosition.y : lastTouch.y;
                transform.position = Vector2.Lerp(transform.position, lastTouch, config.velocity);
            }

            ball.Rotate( Vector3.back * config.rotateVelocity);
            if (isTrigglingWithSnow)
                ScaleBall();

            Holder.PlaySound?.Invoke();
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
            if (canMoveCam)
            {
                cam.transform.position = new Vector3(transform.position.x, transform.position.y, camPos.z);
            }

            if (isReadyToReleaseBall)
            {
                ShowArrow();

                if(arrowIsShown)
                {
                    ArrowRotatingTo(stageArea.position);
                }
            }
        }

        public override void Pause(bool isSystem = false)
        {
            if (!isSystem)
            {
                candrag = false;
                canMoveCam = false;
            }
        }

        public override void Play()
        {
            GameplayState = IMinigame.GameState.Playing;
            candrag = true;
            canMoveCam = true;
            wolfoo.PlayPushAnim();
        }

        public override void ResetDefault()
        {
        }

        #endregion

    }
}
