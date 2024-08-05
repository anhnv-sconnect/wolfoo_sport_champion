using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Gameplay.IPlayer;

namespace WFSport.Gameplay.Base
{
    public abstract class Player: MonoBehaviour
    {
        public enum DragEventType
        {
            OnScreen,
            OnCharacter
        }
        public abstract void Init();
        public abstract void OnUpdate();
        public abstract void OnSwipe();
        public abstract void OnBeginDragCharacter();
        public abstract void OnDragCharacter();
        public abstract void OnEndDragCharacter();

        [SerializeField] private DragEventType dragEvent;
        [SerializeField] private bool canSwiping;

        protected SwipingDirection currentSwipingDirection;
        protected DragEventType DragEvent { get => dragEvent; set => dragEvent = value; }
        public bool RegisterSwipeEvent { get => canSwiping; set => canSwiping = value; }

        private Vector3 touchBeginPos;
        private Vector3 touchPos;
        private bool isSwiping;

        #region UNITY METHODS

        private void Update()
        {
            if(dragEvent == DragEventType.OnScreen)
            {
#if UNITY_EDITOR
                DetectTouchingWindow();
#else
                DetectTouchingMobile();
#endif
            }
            OnUpdate();
        }
        private void OnMouseDown()
        {
            if(dragEvent == DragEventType.OnCharacter)
            {
                touchBeginPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchPos = touchBeginPos;
                OnBeginDrag();
            }
        }
        private void OnMouseDrag()
        {
            if (dragEvent == DragEventType.OnCharacter)
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                OnDrag();
            }
        }
        private void OnMouseUp()
        {
            if (dragEvent == DragEventType.OnCharacter)
            {
                OnEndDrag();
            }
        }

        #endregion

        private void DetectTouchingWindow()
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchBeginPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchPos = touchBeginPos;
                OnBeginDrag();
            }
            if (Input.GetMouseButton(0))
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                OnDrag();
            }
            if (Input.GetMouseButtonUp(0))
            {
                OnEndDrag();
            }
        }

        private void DetectTouchingMobile()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    touchPos = touch.position;
                    OnDrag();
                }

                if (Input.touchCount == 2)
                {
                    touch = Input.GetTouch(1);

                    if (touch.phase == TouchPhase.Began)
                    {
                        touchBeginPos = touch.position;
                        touchPos = touchBeginPos;
                        OnBeginDrag();
                    }

                    if (touch.phase == TouchPhase.Ended)
                    {
                        OnEndDrag();
                    }
                }
            }
        }



        private void SwipeUp()
        {
            currentSwipingDirection = SwipingDirection.Up;
            OnSwipe();
        }
        private void SwipeDown()
        {
            currentSwipingDirection = SwipingDirection.Down;
            OnSwipe();
        }

        private void OnBeginDrag()
        {
            if (canSwiping)
            {
                isSwiping = false;
            }
        }

        private void OnDrag()
        {
            if(canSwiping)
            {
                if (isSwiping) return;
                if (touchPos.y - touchBeginPos.y > 0.5f)
                {
                    isSwiping = true;
                    SwipeUp();
                }
                else if (-touchPos.y + touchBeginPos.y > 0.5f)
                {
                    isSwiping = true;
                    SwipeDown();
                }
            }
        }

        private void OnEndDrag()
        {
            if (canSwiping)
            {
                isSwiping = false;
            }
        }

    }
}
