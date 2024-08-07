using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Gameplay.IMinigame;
using static WFSport.Gameplay.IPlayer;
using TAG = WFSport.Base.Constant.TAG;

namespace WFSport.Gameplay.RelayMode
{
    public class Player : Base.Player
    {
        public enum Mode
        {
            Hurdling,
            Passthrough,
            Pathway1,
            Pathway2,
        }

        [NaughtyAttributes.OnValueChanged("OnPlayModeChanged")]
        [SerializeField] private Mode playerMode;
        [SerializeField] private float dragMovingSpeed;
        [SerializeField] private float speed;
        [SerializeField] private float slowSpeed;
        [SerializeField] private float boostSpeed;
        [SerializeField] private float jumpingForce;
        [SerializeField] private float conePassingForce;
        [SerializeField] private float switchLaneForce;
        [SerializeField] private float speedBoostingTime;
        [SerializeField] private float protectedTimer;
        [SerializeField] private int totalProtectTime;
        [SerializeField] private CharacterWorldAnimation characterAnimation;
        [SerializeField] GameObject[] colliderStages;
        [SerializeField] GameObject shield;

        private int curStageIdx;
        private GameState gameState;

        private int myLane = 1;
        private int maxLane = 3;
        private bool isStop;
        private Camera camera_;
        private Vector3 camRange;
        private Rigidbody2D rb2D;
        private bool canJumping;
        private bool isInited;

        private float mySpeed;
        private bool isBoosting;
        private Tweener _tweenDelay;
        private bool isProtecting;
        private int countShield;
        private (TrafficCone cone, float distance, bool isLine1) neighborCrossCone;

        private void OnEnable()
        {
            EventManager.OnInitGame += Init;
            EventManager.OnTracking += FindWaytoPassObstacle;
        }
        private void OnDisable()
        {
            EventManager.OnInitGame -= Init;
            EventManager.OnTracking -= FindWaytoPassObstacle;
        }
        private void Start()
        {
            Setup(playerMode);
        }
        private void OnDestroy()
        {
            _tweenDelay?.Kill();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isStop && collision.collider.CompareTag(TAG.GROUND))
            {
                Holder.PlaySound?.Invoke();
                Holder.PlayParticle?.Invoke();
                canJumping = true;
                PlayRun();
            }

            if (collision.collider.CompareTag(TAG.OBSTACLE))
            {
                if (collision.collider.GetComponent<Barrier>())
                {
                    Holder.PlaySound?.Invoke();
                    Holder.PlayParticle?.Invoke();
                    isStop = true;
                    StartCoroutine("OnCollisonWithBarrier");
                }
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(TAG.BONUSITEM))
            {
                if (collision.GetComponent<StarItem>())
                {
                    Debug.Log("Claim Star");
                    Holder.PlaySound?.Invoke();
                    EventManager.OnPlayerClaimNewStar?.Invoke(this);
                }
                else if (collision.GetComponent<ShieldItem>())
                {
                    Debug.Log("Claim Shield");
                    Holder.PlaySound?.Invoke();
                    StopCoroutine("OnShieldProtecting");
                    StartCoroutine("OnShieldProtecting");
                }
                else if (collision.GetComponent<BoostItem>())
                {
                    Debug.Log("Claim Boost");
                    Holder.PlaySound?.Invoke();
                    StopCoroutine("OnSpeedBoosting");
                    StartCoroutine("OnSpeedBoosting");
                }
            }
            if (collision.CompareTag(TAG.FINISH))
            {
                Debug.Log($"Finish Stage {curStageIdx + 1} !!!");
                Holder.PlaySound?.Invoke();
                EventManager.OnFinishStage?.Invoke();
                gameState = GameState.Stopping;
                OnFinishStage();
            }
            if (collision.CompareTag(TAG.OBSTACLE))
            {
                if (collision.GetComponent<Mud>())
                {
                    if(isProtecting)
                    {
                        countShield--;
                        if(countShield == 0)
                        {
                            DisableShield();
                        }
                    }
                    else
                    {
                        Holder.PlaySound?.Invoke();
                        PlayRunSlower();
                    }
                }
                else if (collision.GetComponent<TrafficCone>())
                {
                    if(neighborCrossCone.cone == null)
                    {
                        var cone = collision.GetComponent<TrafficCone>();
                        Holder.PlaySound?.Invoke();
                        Holder.PlayParticle?.Invoke();

                        neighborCrossCone = (cone, 1000, cone.IsLine1);
                        EventManager.OnTriggleWithCone?.Invoke(cone);
                        StartCoroutine("OnCollisonWithCone");
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(TAG.OBSTACLE))
            {
                if (collision.GetComponent<Mud>() && !isProtecting)
                {
                    Holder.PlaySound?.Invoke();
                    if(isBoosting)
                    {
                        PlayRunFaster();
                    }
                    else
                    {
                        PlayRun();
                    }
                }
            }
        }

        #region Editor

        private void OnPlayModeChanged()
        {
            Setup(playerMode);
            isInited = false;
        }

        #endregion

        #region METHODS

        private void PlayRunFaster()
        {
            mySpeed = boostSpeed;
            characterAnimation.PlayRunFastAnim();
        }
        private void PlayRunSlower()
        {
            mySpeed = slowSpeed;
            characterAnimation.PlaySlowAnim();
        }
        private void PlayRun()
        {
            mySpeed = speed;
            characterAnimation.PlayRunAnim();
        }
        private void DisableShield()
        {
            isProtecting = false;
            StopCoroutine(OnShieldProtecting());
            shield.SetActive(false);
        }
        private IEnumerator OnSpeedBoosting()
        {
            isBoosting = true;
            PlayRunFaster();

            yield return new WaitForSeconds(speedBoostingTime);

            isBoosting = false;
            PlayRun();
        }
        private IEnumerator OnShieldProtecting()
        {
            isProtecting = true;
            countShield = totalProtectTime;
            shield.SetActive(true);

            yield return new WaitForSeconds(protectedTimer);

            shield.SetActive(false);
            isProtecting = false;
        }

        private void OnFinishStage()
        {
            _tweenDelay = DOVirtual.Float(speed, 0, 1, (value) =>
            {
                mySpeed = value;
                transform.Translate(Vector3.right * mySpeed * Time.deltaTime);

            }).OnComplete(() =>
            {
                characterAnimation.PlayIdleAnim();

                curStageIdx++;
                if(curStageIdx < colliderStages.Length)
                {
                    for (int i = 0; i < colliderStages.Length; i++)
                    {
                        colliderStages[i].SetActive(i == curStageIdx);
                    }
                }
            });
        }
        private void OnSwitchLane()
        {
            transform.position = new Vector3(transform.position.x, (-switchLaneForce + myLane * 2f) * -1, 0);
        }
        private void CamFollowing()
        {
            camera_.transform.position = new Vector3(transform.position.x + camRange.x, camera_.transform.position.y, camera_.transform.position.z);
        }

        #endregion


        #region Inititial
        public override void OnBeginDrag(DragEventType dragEvent)
        {
        }

        public override void OnDrag(DragEventType dragEvent)
        {
        }

        public override void OnEndDrag(DragEventType dragEvent)
        {
        }

        public override void OnDragging(Vector3 force)
        {
            if (!isStop)
            {
                transform.Translate((Vector3.up * force.y) * dragMovingSpeed);
            }
        }

        public override void OnUpdate()
        {
            if (gameState == GameState.Pausing || gameState == GameState.Stopping) return;

            if (!isStop)
            {
                AutoMoving(Direction.Right);
            }

            CamFollowing();
        }

        public override void Init()
        {
            if (isInited) return;
            isInited = true;
            canJumping = true;

            camera_ = Camera.main;
            rb2D = GetComponent<Rigidbody2D>();

            camRange = camera_.transform.position - transform.position;
            mySpeed = speed;
        }
        public void Setup(Mode mode)
        {
            Init();

            shield.SetActive(false);

            curStageIdx = (int)playerMode;
            for (int i = 0; i < colliderStages.Length; i++)
            {
                colliderStages[i].SetActive(i == curStageIdx);
            }

            switch (mode)
            {
                case Mode.Hurdling:
                    rb2D.bodyType = RigidbodyType2D.Dynamic;
                    DetectType = DetectType.Swiping;
                    EnableTopDown = false;
                    break;
                case Mode.Passthrough:
                    rb2D.bodyType = RigidbodyType2D.Kinematic;
                    DetectType = DetectType.Swiping;
                    EnableTopDown = false;
                    break;
                case Mode.Pathway1:
                case Mode.Pathway2:
                    rb2D.bodyType = RigidbodyType2D.Kinematic;
                    DetectType = DetectType.Dragging;
                    EnableTopDown = true;
                    break;
            }
        }
        #endregion

        #region Pathway Mode

        private IEnumerator OnCollisonWithCone()
        {
            isStop = true;
            characterAnimation.PlayDizzyAnim();

            Holder.PlayAnim?.Invoke();

            yield return new WaitForSeconds(1);

            if (neighborCrossCone.cone != null)
            {
                var force = 0.0f;
                var distance = neighborCrossCone.cone.transform.position.y - transform.position.y;
                var moveDirection = distance < 0 ? Direction.Down : Direction.Up;
                while (force < Mathf.Abs(distance) / 2)
                {
                    AutoMoving(moveDirection);
                    yield return new WaitForEndOfFrame();
                    force += conePassingForce * Time.deltaTime;
                }
            }

            isStop = false;
            neighborCrossCone = (null, 0, true);
            characterAnimation.PlayRunAnim();
        }

        public void FindWaytoPassObstacle(TrafficCone cone)
        {
            if (cone.IsLine1 != neighborCrossCone.isLine1)
            {
                var distance = Vector2.Distance(cone.transform.position, transform.position);
                if (distance < neighborCrossCone.distance)
                {
                    neighborCrossCone = (cone, distance, neighborCrossCone.isLine1);
                }
            }
        }

        #endregion

        #region Hurdling Mode
        private IEnumerator OnCollisonWithBarrier()
        {
            characterAnimation.PlayDizzyAnim();

            yield return new WaitForSeconds(1);

            canJumping = true;
        }

        private void OnJumping()
        {
            isStop = false;
            characterAnimation.PlayJumpAnim();
            rb2D.AddForce(Vector2.up * jumpingForce);
        }

        private void AutoMoving(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    transform.Translate(Vector3.up * mySpeed * Time.deltaTime);
                    break;
                case Direction.Down:
                    transform.Translate(Vector3.down * mySpeed * Time.deltaTime);
                    break;
                case Direction.Left:
                    transform.Translate(Vector3.left * mySpeed * Time.deltaTime);
                    break;
                case Direction.Right:
                    transform.Translate(Vector3.right * mySpeed * Time.deltaTime);
                    break;
            }
        }

        private void OnHurdlingMode()
        {
            if (playerMode != Mode.Hurdling) return;

            if (canJumping)
            {
                canJumping = false;
                OnJumping();
            }
        }
        #endregion

        #region Passthrough Mode
        private void OnPassthroughMode()
        {
            if (playerMode != Mode.Passthrough) return;

            if (currentSwipingDirection == Direction.Up)
            {
                if (myLane <= 0) return;
                myLane--;
                OnSwitchLane();
            }
            else if (currentSwipingDirection == Direction.Down)
            {
                if (myLane >= maxLane - 1) return;
                myLane++;
                OnSwitchLane();
            }
        }

        public override void OnSwipe()
        {
            switch (currentSwipingDirection)
            {
                case Direction.Up:
                    OnPassthroughMode();
                    OnHurdlingMode();
                    break;
                case Direction.Down:
                    currentSwipingDirection = Direction.Down;
                    OnPassthroughMode();
                    break;
                case Direction.Left:
                    break;
                case Direction.Right:
                    break;
            }
        }
        #endregion

    }
}
