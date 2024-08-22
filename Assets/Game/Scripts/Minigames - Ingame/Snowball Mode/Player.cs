using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.SnowballMode
{
    public class Player : Base.Player
    {
        private IMinigame.GameState gamestate;

        protected override IMinigame.GameState GameplayState { get => gamestate; set => gamestate = value; }

        #region UNITY METHODS
        // Start is called before the first frame update
        void Start()
        {

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
