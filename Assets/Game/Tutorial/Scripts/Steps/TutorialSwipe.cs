using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnhNV.GameBase.AnimatorHelper;

namespace AnhNV.GameBase
{
    public class TutorialSwipe : TutorialStep
    {
        [SerializeField] AnimatorHelper animatorHelper;
        [SerializeField] Animator animator;
        [SerializeField] string playDownName;
        [SerializeField] string playUpName;
        [SerializeField] string playRightName;
        [SerializeField] string playLeftName;
        [SerializeField] Transform pointer;
        [SerializeField] Transform arrow;
        [SerializeField] Canvas canvas;

        private Direction swipeMode;
        private string tutorialID;
        public System.Action OnSwipeCorrectDirection;

        public override string TutorialID { get => tutorialID; set => tutorialID = value; }

        private void Start()
        {
            animator.gameObject.SetActive(false);
        }

        public override void Play()
        {
            RegisterSwipeEvent();
            animator.gameObject.SetActive(true);
            IsPlaying = true;

            switch (swipeMode)
            {
                case Direction.Up:
                    animator.Play(playUpName, 0, 0);
                    break;
                case Direction.Down:
                    animator.Play(playDownName, 0, 0);
                    break;
                case Direction.Left:
                    animator.Play(playLeftName, 0, 0);
                    break;
                case Direction.Right:
                    animator.Play(playRightName, 0, 0);
                    break;
            }
        }
        public override void Stop()
        {
            RemoveSwipeEvent();
            animator.gameObject.SetActive(false);
            IsPlaying = false;
        }
        protected override void OnSwiping(Direction direction)
        {
            base.OnSwiping(direction);
            if(direction == swipeMode)
            {
                OnSwipeCorrectDirection?.Invoke();
            }
        }
        /// <summary>
        /// Attach to Object Position
        /// </summary>
        /// <param name="position"></param>
        public void Setup(Transform target, Direction swipeDirection)
        {
            canvas.worldCamera = Camera.main;
            swipeMode = swipeDirection;
            SetPointer(target.position);
        }

        private void SetPointer(Vector3 endPos)
        {
            pointer.position = endPos;
            arrow.position = pointer.position;

            pointer.localPosition = new Vector3(pointer.localPosition.x, pointer.localPosition.y, 0);
            arrow.localPosition = new Vector3(arrow.localPosition.x, arrow.localPosition.y, 0);

            if (swipeMode == Direction.Down || swipeMode == Direction.Up)
            {
                pointer.transform.localRotation = Quaternion.Euler(new Vector3(0, endPos.x < 0 ? 180 : 0, -40));
            }
        }

        public override void Release()
        {
            Destroy(gameObject);
        }
    }
}
