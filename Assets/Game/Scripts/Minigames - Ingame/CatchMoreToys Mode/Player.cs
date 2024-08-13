using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    public class Player : Base.Player
    {
        private IMinigame.GameState gameState;

        protected override IMinigame.GameState GameplayState { get => gameState; set => gameState = value; }

        #region Unity Methods
        void Start()
        {

        }
        #endregion

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

        public override void Play()
        {
        }

        public override void ResetDefault()
        {
        }
    }
}
