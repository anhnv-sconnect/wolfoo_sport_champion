using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    public class Player : Base.Player
    {
        [SerializeField] private GameplayConfig config;

        private IMinigame.GameState gameState;

        protected override IMinigame.GameState GameplayState { get => gameState; set => gameState = value; }

        #region Unity Methods
        void Start()
        {

        }
        #endregion

        #region Override Methods
        public override void Init()
        {
        }

        public override void Lose()
        {
        }

        public override void OnDragging(Vector3 force)
        {
            transform.Translate(new Vector3(force.x * config.velocity * Time.deltaTime, 0, 0));
            if (transform.position.x < config.limitLeft) transform.position = new Vector3(config.limitLeft, transform.position.y, transform.position.z);
            if (transform.position.x > config.limitRight) transform.position = new Vector3(config.limitRight, transform.position.y, transform.position.z);
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
        #endregion
    }
}
