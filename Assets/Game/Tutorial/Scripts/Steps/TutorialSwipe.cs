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
        [SerializeField] Transform pointer;
        [SerializeField] Transform arrow;
        [SerializeField] Canvas canvas;

        private Direction swipeMode;
        private string tutorialID;

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
                    break;
                case Direction.Right:
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
            Debug.Log("OnSwipinggggg");
            if(direction == swipeMode)
            {
                OnTutorialComplete?.Invoke();
            }
        }
        /// <summary>
        /// Attach to Object Position
        /// </summary>
        /// <param name="position"></param>
        public void Setup(Transform target, Direction swipeDirection)
        {
            canvas.worldCamera = Camera.main;
            SetPointer(target.position);
            swipeMode = swipeDirection;
        }

        private void SetPointer(Vector3 endPos)
        {
            pointer.position = endPos;
            arrow.position = pointer.position;

            pointer.localPosition = new Vector3(pointer.localPosition.x, pointer.localPosition.y, 0);
            arrow.localPosition = new Vector3(arrow.localPosition.x, arrow.localPosition.y, 0);

            pointer.transform.localRotation = Quaternion.Euler(new Vector3(0, endPos.x < 0 ? 180 : 0, -40));
        }

        public override void Release()
        {
            Destroy(gameObject);
        }
    }
}
