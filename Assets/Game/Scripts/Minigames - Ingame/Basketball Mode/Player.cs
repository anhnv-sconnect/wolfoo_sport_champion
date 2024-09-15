using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    public class Player : Base.Player
    {
        [SerializeField] Ball ballPb;
        [SerializeField] Transform basket;
        [SerializeField] CharacterWorldAnimation characterAnim;

        private bool isReloading;
        private Vector3 myTouchPos;
        private Basket verifiedBasket;
        private GameplayConfig config;

        private Ball[] poolingBalls;
        private int countBall;
        private IMinigame.GameState gameState;
        private Ball currentBall;
        private int myScore;

        protected override IMinigame.GameState GameplayState { get => gameState; set => gameState = value; }
        public int Score { get => myScore; }

        #region UNITY METHODS
        private void Start()
        {
            EventManager.OnBallTracking += OnBallTracking;
           // Init();
        }
        private void OnDestroy()
        {
            EventManager.OnBallTracking -= OnBallTracking;
        }

        #endregion

        #region MY METHODS
        internal void Setup(GameplayConfig config)
        {
            this.config = config;
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

            countBall++;
            if (countBall >= poolingBalls.Length) countBall = 0;
            return ball;
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
                myScore++;
                currentBall.FlyTo(verifiedBasket.HolePos, verifiedBasket.transform);
                if(verifiedBasket.HasBomb)
                {
                    myScore += config.effectBombScore;
                    EventManager.OnShootingBomb?.Invoke(this);
                }

                if(verifiedBasket.BonusItem.isPlaying)
                {
                    myScore += verifiedBasket.BonusItem.score;
                }
            }
            else
            {
                currentBall.FlyTo(myTouchPos, null);
            }

            yield return new WaitForSeconds(config.reloadTime);
            currentBall = GetNextBall();
            currentBall.Show();

            isReloading = false;
        }

        #endregion

        #region OVERRIDE METHODS
        public override void Init()
        {
            int totalBallTime = Mathf.Max(Mathf.CeilToInt(config.flyTime * 3), Mathf.FloorToInt(config.reloadTime)) + 1; // => Time FlyIn and FlyOut
            poolingBalls = new Ball[totalBallTime];
            ballPb.Hide();
            currentBall = GetNextBall();
            currentBall.Show();
            countBall = 0;
            characterAnim.PlayBackIdleAnim();
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
