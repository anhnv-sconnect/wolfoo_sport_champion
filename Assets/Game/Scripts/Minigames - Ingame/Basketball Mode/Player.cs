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
        private IMinigame.GameState gameState;
        private bool isReloading;
        private Vector3 myTouchPos;
        private Basket verifiedBasket;
        private GameplayConfig config;

        protected override IMinigame.GameState GameplayState { get => gameState; set => gameState = value; }

        #region UNITY METHODS
        private void Start()
        {
            EventManager.OnBallTracking += OnBallTracking;
            Init();
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

        private void OnThrowing(Vector3 endPos, Transform parent)
        {
            var ball = Instantiate(ballPb, ballPb.transform.parent);
            ball.Setup(config.height, config);
            ball.FlyTo(endPos, parent);
        }
        private void OnThrowing()
        {
           var ball = Instantiate(ballPb, ballPb.transform.parent);
            var rdX = UnityEngine.Random.Range(-7, 7);
            var rdY = UnityEngine.Random.Range(-1, 4);
            var rd = UnityEngine.Random.Range(0, 2);
            ball.FlyTo(rd == 1 ? basket.position : new Vector3(rdX, rdY, 0), transform);
        }

        private IEnumerator BeginThrow()
        {
            isReloading = true;
            verifiedBasket = null;
            EventManager.OnThrow?.Invoke(this, myTouchPos);
            yield return new WaitForSeconds(0.1f);

            if (verifiedBasket != null)
            {
                OnThrowing(verifiedBasket.HolePos, verifiedBasket.transform);
            }
            else
            {
                OnThrowing(myTouchPos, null);
            }

            yield return new WaitForSeconds(config.reloadTime);
            isReloading = false;
        }

        #endregion

        #region OVERRIDE METHODS
        public override void Init()
        {
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
