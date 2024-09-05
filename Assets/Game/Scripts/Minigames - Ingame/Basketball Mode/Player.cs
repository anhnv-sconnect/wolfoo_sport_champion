using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    public class Player : Base.Player
    {
        [SerializeField] Ball ballPb;
        private IMinigame.GameState gameState;

        protected override IMinigame.GameState GameplayState { get => gameState; set => gameState = value; }

        #region UNITY METHODS
        private void Start()
        {
            gameState = IMinigame.GameState.Playing;
        }
        #endregion

        #region MY METHODS

        [NaughtyAttributes.Button]
        private void Throw(Vector3 endPos)
        {
           var ball = Instantiate(ballPb, ballPb.transform.parent);
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
