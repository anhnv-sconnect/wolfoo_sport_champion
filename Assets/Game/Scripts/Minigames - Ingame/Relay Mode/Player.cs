using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
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
        [SerializeField] private GameObject[] colliderStages;
        [SerializeField] private GameObject shield;
        [SerializeField] private CharacterWorldAnimation[] characterData;

        private Mode playerMode;
        private CharacterWorldAnimation characterAnimation;
        private int curStageIdx;
        private GameState gameState;
        protected GameState GameState
        {
            set
            {
                gameState = value;
                if(GameState == GameState.Stopping || GameState == GameState.Pausing)
                {
                    isStop = true;
                }
                _tweenMove?.Kill();
            }
            get
            {
                return gameState;
            }
        }

        private int myLane = 1;
        private int maxLane = 3;
        private bool isStop = true;
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
        private (TrafficCone cone, float distance, bool isLine1, float lane) neighborCrossCone;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tweenMove;

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
                if (!HasProtection())
                {
                    if (collision.collider.transform.parent.GetComponent<Barrier>())
                    {
                        Holder.PlaySound?.Invoke();
                        Holder.PlayParticle?.Invoke();
                        isStop = true;
                        StartCoroutine("OnCollisonWithBarrier");
                    }
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
                Debug.Log($"Finish Stage {playerMode} !!!");
                Holder.PlaySound?.Invoke();
                EventManager.OnFinishStage?.Invoke();
                GameState = GameState.Stopping;
                OnFinishStage();
            }
            if (collision.CompareTag(TAG.OBSTACLE))
            {
                if (collision.GetComponent<Mud>())
                {
                    if (!HasProtection())
                    {
                        Holder.PlaySound?.Invoke();
                        PlayRunSlower();
                    }
                }
                else if (collision.GetComponent<TrafficCone>())
                {
                    if (!HasProtection())
                    {
                        if (neighborCrossCone.cone == null)
                        {
                            var cone = collision.GetComponent<TrafficCone>();
                            Holder.PlaySound?.Invoke();
                            Holder.PlayParticle?.Invoke();

                            OnCollisonWithCone(cone);
                        }
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

        private bool HasProtection()
        {
            if (isProtecting)
            {
                countShield--;
                if (countShield == 0)
                {
                    DisableShield();
                }
                return true;
            }
            return false;
        }
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
            _tweenDelay = DOVirtual.Float(speed, 0, 2, (value) =>
            {
                mySpeed = value;
                AutoMoving(Direction.Right);

            }).OnComplete(() =>
            {
                characterAnimation.PlayIdleAnim();
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
                if (transform.position.y > Constant.ABOVE_LIMIT)
                {
                    transform.position = new Vector3(transform.position.x, Constant.ABOVE_LIMIT, transform.position.z);
                }
                if (transform.position.y < Constant.BELOW_LIMIT)
                {
                    transform.position = new Vector3(transform.position.x, Constant.BELOW_LIMIT, transform.position.z);
                }
            }
        }

        public override void OnUpdate()
        {
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
        }

        public void CreateNew()
        {
            var character = Instantiate(characterData[UnityEngine.Random.Range(0, characterData.Length)], transform);
            characterAnimation = character;
            characterAnimation.gameObject.layer = 6;
            characterAnimation.transform.localPosition = new Vector3(0, -1, 0);
            characterAnimation.transform.rotation = Quaternion.Euler(Vector3.up * 180);
        }
        public override void Play()
        {
            isStop = false;
        }
        public void Setup(Mode mode)
        {
            Init();

            if (characterAnimation == null) CreateNew();
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
                    speed = 7;
                    break;
                case Mode.Passthrough:
                    rb2D.bodyType = RigidbodyType2D.Kinematic;
                    DetectType = DetectType.Swiping;
                    EnableTopDown = false;
                    speed = 7;
                    break;
                case Mode.Pathway1:
                case Mode.Pathway2:
                    rb2D.bodyType = RigidbodyType2D.Kinematic;
                    DetectType = DetectType.Dragging;
                    EnableTopDown = true;
                    speed = 10;
                    break;
            }
            mySpeed = speed;
        }
        #endregion

        #region Pathway Mode

        private void OnCollisonWithCone(TrafficCone cone)
        {
            if(playerMode == Mode.Pathway1)
            {
                neighborCrossCone = (cone, 1000, cone.IsLine1, cone.Lane);
                EventManager.OnTriggleWithCone?.Invoke(cone);
                StartCoroutine("PlayWithModePathway1");
            }
            else if(playerMode == Mode.Pathway2)
            {
                neighborCrossCone = (cone, 0, cone.IsLine1, cone.Lane);
                EventManager.OnTriggleWithCone?.Invoke(cone);
                StartCoroutine("PlayWithModePathway2");
            }
        }
        private IEnumerator PlayWithModePathway2()
        {
            isStop = true;
            characterAnimation.PlayDizzyAnim();

            Holder.PlayAnim?.Invoke();

            yield return new WaitForSeconds(1);

            var destination = Vector2.zero;
            switch (neighborCrossCone.lane)
            {
                case Constant.CONE_LINE1:
                case Constant.CONE_LINE3:
                    destination = new Vector2(transform.position.x, Constant.MID);
                    break;
                case Constant.CONE_LINE2:
                    destination = new Vector2(transform.position.x, UnityEngine.Random.Range(0, 2) == 1 ? Constant.ABOVE_LIMIT : Constant.BELOW_LIMIT);
                    break;
            }
            MovingTo(destination, () =>
            {
                isStop = false;
                neighborCrossCone = (null, 0, true, Constant.CONE_LINE3);
                characterAnimation.PlayRunAnim();
            });

        }
        private IEnumerator PlayWithModePathway1()
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
                while (force < Mathf.Abs(distance) / 3)
                {
                    AutoMoving(moveDirection);
                    yield return new WaitForEndOfFrame();
                    force += conePassingForce * Time.deltaTime;
                }
            }

            isStop = false;
            neighborCrossCone = (null, 0, true, Constant.CONE_LINE3);
            characterAnimation.PlayRunAnim();
        }

        public void FindWaytoPassObstacle(TrafficCone cone)
        {
            if(playerMode == Mode.Pathway1)
            {
                if (cone.IsLine1 != neighborCrossCone.isLine1)
                {
                    var distance = Vector2.Distance(cone.transform.position, transform.position);
                    if (distance < neighborCrossCone.distance)
                    {
                        neighborCrossCone = (cone, distance, neighborCrossCone.isLine1, cone.Lane);
                    }
                }
            }
            else if(playerMode == Mode.Pathway2)
            {
                if(neighborCrossCone.cone.Lane != cone.Lane)
                {
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

        private void MovingTo(Vector2 position, System.Action OnCompleted)
        {
            characterAnimation.PlayRunAnim();
            _tweenMove?.Kill();
            _tweenMove = transform.DOMove(position, 0.5f)
                .OnUpdate(() =>
                {
                    EventManager.OnPlayerIsMoving?.Invoke(this);
                })
                .OnComplete(() =>
                {
                    OnCompleted?.Invoke();
                });
        }
        private void AutoMoving(Direction direction)
        {
            _tweenMove?.Kill();
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

            EventManager.OnPlayerIsMoving?.Invoke(this);
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
