using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    public class Player : Base.Player
    {
        [SerializeField] Ball ballPb;
        [SerializeField] Transform basket;
        [SerializeField] float reloadTime;
        private IMinigame.GameState gameState;
        private bool isReloading;

        protected override IMinigame.GameState GameplayState { get => gameState; set => gameState = value; }

        #region UNITY METHODS
        private void Start()
        {
            gameState = IMinigame.GameState.Playing;
        }
        #endregion

        #region MY METHODS

        [NaughtyAttributes.Button]
        private void Throw()
        {
           var ball = Instantiate(ballPb, ballPb.transform.parent);
            var rdX = UnityEngine.Random.Range(-7, 7);
            var rdY = UnityEngine.Random.Range(-1, 4);
            var rd = UnityEngine.Random.Range(0, 2);
            ball.FlyTo(rd == 1 ? basket.position : new Vector3(rdX, rdY, 0), transform);
        }

        private IEnumerator CountdownReloadTime()
        {
            isReloading = true;
            yield return new WaitForSeconds(reloadTime);
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
            StartCoroutine("CountdownReloadTime");
            Throw();
        }

        public override void OnUpdate()
        {
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
