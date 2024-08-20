using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.LatinDanceMode
{
    public class Player : Base.Player
    {
        [SerializeField] BoxCollider2D limited;
        [SerializeField] float time;
        private IMinigame.GameState gameState;
        private Vector3 inititalPos;
        private Vector2 inittialLimited;
        private Camera cam;
        private Vector3 initialCamPos;
        private bool isCalculating = false;

        protected override IMinigame.GameState GameplayState { get => gameState; set => gameState = value; }

        #region UNITY METHODS

        void Start()
        {
            GameplayState = IMinigame.GameState.Playing;
            Init();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(TAG.BONUSITEM))
            {

            }
        }
        #endregion

        #region OVERRIDE METHODS

        public override void Init()
        {
            inititalPos = transform.position;
            inittialLimited = limited.size / 2;
            cam = Camera.main;
            initialCamPos = cam.transform.position;
        }

        public override void Lose()
        {
        }
        public bool IsPointInCapsule(Vector2 point, Vector2 capsuleStart, Vector2 capsuleEnd, float radius)
        {
            // Vector from start to end of the capsule (capsule's axis)
            Vector2 capsuleAxis = capsuleEnd - capsuleStart;

            // Length of the capsule axis
            float capsuleLength = capsuleAxis.magnitude;

            // Normalize the capsule axis to unit length
            Vector2 capsuleAxisNormalized = capsuleAxis / capsuleLength;

            // Project the point onto the capsule axis
            Vector2 pointProjection = Vector2.Dot(point - capsuleStart, capsuleAxisNormalized) * capsuleAxisNormalized + capsuleStart;

            // Distance from the start to the projected point
            float projectionLength = Vector2.Dot(pointProjection - capsuleStart, capsuleAxisNormalized);

            // Check if the point projection lies within the central rectangle part of the capsule
            if (projectionLength >= 0 && projectionLength <= capsuleLength)
            {
                // Check if the perpendicular distance to the capsule axis is within the capsule's radius
                float distanceToAxis = Vector2.Distance(point, pointProjection);
                if (distanceToAxis <= radius)
                {
                    return true; // Point is within the central rectangular part
                }
            }

            // If not in the rectangle, check if it is in either semicircle
            if (Vector2.Distance(point, capsuleStart) <= radius || Vector2.Distance(point, capsuleEnd) <= radius)
            {
                return true; // Point is within one of the semicircles
            }

            return false; // Point is outside the capsule
        }

        public override void OnDragging(Vector3 force)
        {
            if (isCalculating) return;

            isCalculating = true;
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (pos.x > inittialLimited.x) pos.x = inittialLimited.x;
            if (pos.y > inittialLimited.y) pos.y = inittialLimited.y;
            if (pos.x < -inittialLimited.x) pos.x = -inittialLimited.x;
            if (pos.y < -inittialLimited.y) pos.y = -inittialLimited.y;
            transform.position = pos;

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
            cam.transform.position = Vector3.Lerp(cam.transform.position, 
                new Vector3(transform.position.x, transform.position.y + 2, initialCamPos.z), 
                time);
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
