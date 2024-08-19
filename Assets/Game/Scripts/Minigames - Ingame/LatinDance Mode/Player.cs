using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.LatinDanceMode
{
    public class Player : Base.Player
    {
        [SerializeField] CapsuleCollider2D limited;
        private IMinigame.GameState gameState;
        private Vector3 inititalPos;
        private Vector2 inittialLimited;
        private bool isCalculating = false;
        private Vector3 myLastPos;

        protected override IMinigame.GameState GameplayState { get => gameState; set => gameState = value; }

        #region UNITY METHODS

        void Start()
        {
            GameplayState = IMinigame.GameState.Playing;
            Init();
        }

        #endregion

        #region OVERRIDE METHODS

        public override void Init()
        {
            inititalPos = transform.position;
            inittialLimited = limited.size / 2;
        }

        public override void Lose()
        {
        }

        public override void OnDragging(Vector3 force)
        {
            if (isCalculating) return;

            isCalculating = true;
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = inititalPos.z;

            if (Mathf.Abs(pos.x) <= inittialLimited.x)
            {
                if (Mathf.Abs(pos.y) > myLastPos.y)
                {
                    transform.position = new Vector3(pos.x, myLastPos.y, pos.z);
                    myLastPos = transform.position;
                }
                else
                {
                    transform.position = new Vector3(pos.x, transform.position.y, pos.z);
                    myLastPos = transform.position;
                }
            }
            if (Mathf.Abs(pos.y) <= inittialLimited.y)
            {
                if (Mathf.Abs(pos.x) > myLastPos.x)
                {
                    transform.position = new Vector3(myLastPos.x, pos.y, pos.z);
                    myLastPos = transform.position;
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, pos.y, pos.z);
                    myLastPos = transform.position;
                }
            }

            isCalculating = false;
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
