using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Gameplay.IMinigame;
using static WFSport.Gameplay.IPlayer;

namespace WFSport.Gameplay.RelayMode
{
    public class Player : Base.Player
    {
        [NaughtyAttributes.OnValueChanged("OnPlayModeChanged")]
        [SerializeField] private Mode playerMode;
        [SerializeField] private float speed;
        [SerializeField] private float slowSpeed;
        [SerializeField] private float boostSpeed;
        [SerializeField] private float jumpingForce;
        [SerializeField] private float switchLaneForce;
        [SerializeField] private float speedBoostingTime;
        [SerializeField] private float protectedTimer;
        [SerializeField] private int totalProtectTime;
        [SerializeField] private CharacterWorldAnimation characterAnimation;
        [SerializeField] GameObject[] colliderStages;
        [SerializeField] GameObject shield;

    //    [NaughtyAttributes.ShowNonSerializedField] private int curStageIdx;
        [SerializeField] private int curStageIdx;

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

        private void OnEnable()
        {
            EventManager.OnInitGame += Init;
        }
        private void OnDisable()
        {
            EventManager.OnInitGame -= Init;
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
            if (!isStop && collision.collider.CompareTag(Constant.GROUND_TAG))
            {
                Holder.PlaySound?.Invoke();
                Holder.PlayParticle?.Invoke();
                canJumping = true;
                PlayRun();
            }

            if (collision.collider.CompareTag(Constant.OBSTACLE_TAG))
            {
                if (collision.collider.GetComponent<Barrier>())
                {
                    Holder.PlaySound?.Invoke();
                    Holder.PlayParticle?.Invoke();
                    isStop = true;
                    StartCoroutine(OnCollisonWithBarrier());
                }
                else if (collision.collider.GetComponent<TrafficCone>())
                {
                    Holder.PlaySound?.Invoke();
                    Holder.PlayParticle?.Invoke();
                    StartCoroutine(OnCollisonWithCone());
                }
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Constant.BONUSITEM_TAG))
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
            if (collision.CompareTag(Constant.FINISH_TAG))
            {
                Debug.Log($"Finish Stage {curStageIdx + 1} !!!");
                Holder.PlaySound?.Invoke();
                EventManager.OnFinishStage?.Invoke();
                gameState = GameState.Stopping;
                OnFinishStage();
            }
            if (collision.CompareTag(Constant.OBSTACLE_TAG))
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
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(Constant.OBSTACLE_TAG))
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

        public override void OnBeginDragCharacter()
        {
        }

        public override void OnDragCharacter()
        {
        }

        public override void OnEndDragCharacter()
        {
        }

        public override void OnUpdate()
        {
            if (gameState == GameState.Pausing || gameState == GameState.Stopping) return;

            if (!isStop)
            {
                AutoMoving();
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

            for (int i = 0; i < colliderStages.Length; i++)
            {
                colliderStages[i].SetActive(i == curStageIdx);
            }

            playerMode = mode;
            switch (mode)
            {
                case Mode.Swiping:
                    rb2D.bodyType = RigidbodyType2D.Static;
                    break;
                case Mode.Running:
                    rb2D.bodyType = RigidbodyType2D.Dynamic;
                    break;
            }
        }

        #endregion

        #region Running Mode

        private IEnumerator OnCollisonWithBarrier()
        {
            characterAnimation.PlayDizzyAnim();

            yield return new WaitForSeconds(1);

            canJumping = true;
        }
        private IEnumerator OnCollisonWithCone()
        {
            isStop = true;
            // Continue Heree

            yield return new WaitForSeconds(1);

            isStop = false;
        }

        private void Jumping()
        {
            isStop = false;
            characterAnimation.PlayJumpAnim();
            rb2D.AddForce(Vector2.up * jumpingForce);
        }

        private void AutoMoving()
        {
            transform.Translate(Vector3.right * mySpeed);
        }

        private void OnCheckingRunningMode()
        {
            if (playerMode != Mode.Running) return;

            if (canJumping)
            {
                canJumping = false;
                Jumping();
            }
        }

        #endregion
        #region Swiping Mode

        private void OnCheckingSwipingMode()
        {
            if (playerMode != Mode.Swiping) return;

            if (currentSwipingDirection == SwipingDirection.Up)
            {
                if (myLane <= 0) return;
                myLane--;
                OnSwitchLane();
            }
            else if (currentSwipingDirection == SwipingDirection.Down)
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
                case SwipingDirection.Up:
                    OnCheckingSwipingMode();
                    OnCheckingRunningMode();
                    break;
                case SwipingDirection.Down:
                    currentSwipingDirection = SwipingDirection.Down;
                    OnCheckingSwipingMode();
                    break;
                case SwipingDirection.Left:
                    break;
                case SwipingDirection.Right:
                    break;
            }
        }

        #endregion

    }
}
