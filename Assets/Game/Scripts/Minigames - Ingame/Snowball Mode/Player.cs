using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.SnowballMode
{
    public class Player : Base.Player
    {
       [SerializeField] private float velocity;
        private IMinigame.GameState gamestate;
        private Vector3 lastTouch;
        private Camera cam;
        private Vector3 camPos;

        protected override IMinigame.GameState GameplayState { get => gamestate; set => gamestate = value; }

        #region UNITY METHODS
        // Start is called before the first frame update
        void Start()
        {
            GameplayState = IMinigame.GameState.Playing;
            Init();
        }
        #endregion

        #region OVERRIDE METHODS

        public override void Init()
        {
            cam = Camera.main;
            camPos = cam.transform.position;
        }

        public override void Lose()
        {
        }

        public override void OnDragging(Vector3 force)
        {
            if (transform.position != lastTouch)
            {
                transform.position = Vector2.Lerp(transform.position, lastTouch, velocity);
                return;
            }
        }

        public override void OnSwipe()
        {
        }

        public override void OnTouching(Vector3 position)
        {
            lastTouch = position;
        }

        public override void OnUpdate()
        {
            if (GameplayState != IMinigame.GameState.Playing) return;
         //   cam.transform.position = new Vector3(transform.position.x, transform.position.y, camPos.z);
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
