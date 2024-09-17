using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Helper;

namespace WFSport.Gameplay.BasketballMode
{
    public class Player : Base.Player
    {
        [SerializeField] Ball ballPb;
        [SerializeField] Transform basket;
        [SerializeField] protected CharacterWorldAnimation characterAnim;

        private bool isReloading;
        private Vector3 myTouchPos;
        private int countBall;
        private int myScore;
        private bool isAutoThrowing;
        private Vector2 screenSize;

        private Basket verifiedBasket;
        private Ball currentBall;
        private Ball[] poolingBalls;
        protected GameplayConfig config;
        protected LevelConfig.Mode level;
        protected GameplayManager gameManager;
        private IMinigame.GameState gameState;
        protected float reloadTime;

        public int Score { get => myScore; }
        public CharacterWorldAnimation CharacterAnim { get => characterAnim; }
        protected override IMinigame.GameState GameplayState { get => gameState; set => gameState = value; }

        #region UNITY METHODS
        private void Start()
        {
            EventManager.OnBallTracking += OnBallTracking;
            EventManager.OnBallShootingTarget += OnBallShootingTarget;
            // Init();
        }
        private void OnDestroy()
        {
            EventManager.OnBallTracking -= OnBallTracking;
            EventManager.OnBallShootingTarget -= OnBallShootingTarget;
        }

        #endregion

        #region MY METHODS
        internal virtual void Setup(GameplayConfig config, LevelConfig.Mode mode, GameplayManager gameManager)
        {
            this.config = config;
            level = mode;
            this.gameManager = gameManager;

            reloadTime = config.reloadTime;
            screenSize = ScreenHelper.GetMaxPosition();
        }
        internal virtual void Create(CharacterWorldAnimation pb)
        {
            characterAnim = Instantiate(pb, transform);

            characterAnim.transform.localPosition = Vector3.zero;
            characterAnim.PlayBackIdleAnim();
            characterAnim.SetTimeScale(2);
            characterAnim.SetLayer(5);
            characterAnim.transform.rotation = Quaternion.Euler(Vector3.up * 0);
        }
        private void OnBallTracking(Player player, Basket basket)
        {
            if (verifiedBasket == null && this == player)
            {
                verifiedBasket = basket;
            }
        }
        private Ball GetNextBall()
        {
            var ball = poolingBalls[countBall];
            if (ball == null)
            {
                ball = Instantiate(ballPb, ballPb.transform.position, ballPb.transform.rotation, ballPb.transform.parent);
                poolingBalls[countBall] = ball;
            }
            ball.Setup(config, this);
            ball.Show();

            countBall++;
            if (countBall >= poolingBalls.Length) countBall = 0;
            return ball;
        }
        protected void PlayAutoThrowing()
        {
            isAutoThrowing = true;

            var isShootingTarget = UnityEngine.Random.Range(0, 100) <= level.botShootingTargetPercent;
            if(isShootingTarget)
            {
                var rdBasket = gameManager.GetRandomBasketInScreen;
                myTouchPos = rdBasket.HolePos;
            }
            else
            {
                var xPos = UnityEngine.Random.Range(-screenSize.x, screenSize.x);
                var yPos = UnityEngine.Random.Range(-screenSize.y, screenSize.y);
                myTouchPos = new Vector3(xPos, yPos, 0);
            }

            StartCoroutine("BeginThrow");
        }
        protected void StopAutoThrowing()
        {
            StopCoroutine("BeginThrow");
        }

        private IEnumerator BeginThrow()
        {
            isReloading = true;
            verifiedBasket = null;
            EventManager.OnThrow?.Invoke(this, myTouchPos);

            characterAnim.PlayBackIdleAnim();
            characterAnim.PlayThrowbackAnim(false);
            yield return new WaitForSeconds(0.3f); // time character Anim Throwing

            if (verifiedBasket != null)
            {
                currentBall.FlyTo(verifiedBasket.HolePos, verifiedBasket);
            }
            else
            {
                currentBall.FlyTo(myTouchPos, null);
            }

            yield return new WaitForSeconds(reloadTime);
            currentBall = GetNextBall();

            isReloading = false;

            if(isAutoThrowing)
            {
                PlayAutoThrowing();
            }
        }

        private void OnBallShootingTarget(Ball ball)
        {
            if (ball.PLayer != this) return;

            var basket = ball.TargetBasket;
            myScore += basket.Score;

            EventManager.OnGetScore?.Invoke(this);
        }
        #endregion

        #region OVERRIDE METHODS
        public override void Init()
        {
            int totalBallTime = Mathf.Max(Mathf.CeilToInt(config.flyTime * 3), Mathf.FloorToInt(reloadTime)) + 1; // => Time FlyIn and FlyOut
            poolingBalls = new Ball[totalBallTime];
            ballPb.Hide();
            currentBall = GetNextBall();
        }

        public override void Lose()
        {
        }

        public override void OnDragging(Vector3 force)
        {
        }

        public override void OnSwipe()
        {
        }

        public override void OnTouching(Vector3 position)
        {
            if (isReloading) return;
            myTouchPos = position;
            myTouchPos.z = 0;
            StartCoroutine("BeginThrow");
        }

        public override void OnUpdate()
        {
        }

        public override void Pause(bool isSystem)
        {
            if(isSystem)
            {
                gameState = IMinigame.GameState.Pausing;
            }
        }

        public override void Play()
        {
            gameState = IMinigame.GameState.Playing;
        }

        public override void ResetDefault()
        {
        }
        #endregion
    }
}
