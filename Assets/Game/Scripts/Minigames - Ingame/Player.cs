using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using static WFSport.Gameplay.IPlayer;
using UnityEngine.Rendering;

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
        public abstract void OnDragging(Vector3 force);
        public abstract void OnBeginDrag(DragEventType dragEventType);
        public abstract void OnDrag(DragEventType dragEventType);
        public abstract void OnEndDrag(DragEventType dragEventType);

        [SerializeField] private DragEventType dragEvent;
        [SerializeField] private DetectType detectType;
        [SerializeField][ShowIf("detectType", DetectType.Dragging)] 
        private float dragRange = 0.2f;
        [SerializeField][ShowIf("detectType", DetectType.Swiping)]
        private float swipeRange = 0.5f;
        [SerializeField] bool enableTopDownPosition;

        protected Direction currentSwipingDirection;
        protected DragEventType DragEvent { get => dragEvent; }

        protected bool SwipingMode { get => detectType == DetectType.Swiping; }
        protected bool DraggingMode { get => detectType == DetectType.Dragging; }
        protected DetectType DetectType { get => detectType; set => detectType = value; }
        protected bool EnableTopDown { get => enableTopDownPosition; set => enableTopDownPosition = value; }

        private Vector3 touchBeginPos;
        private Vector3 touchPos;
        private Vector3 lastTouchPos;
        private bool isSwiping;
        private Vector3 dragForce;
        private SortingGroup myLayer;

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
            if(enableTopDownPosition)
            {
                CalculateTopDownPosition();
            }
            OnUpdate();
        }

        private void OnMouseDown()
        {
            if(dragEvent == DragEventType.OnCharacter)
            {
                touchBeginPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchPos = touchBeginPos;
                lastTouchPos = touchPos;
                OnBeginDrag(dragEvent);
                OnBeginDrag();
            }
        }
        private void OnMouseDrag()
        {
            if (dragEvent == DragEventType.OnCharacter)
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                OnDrag(dragEvent);
                OnDrag();
            }
        }
        private void OnMouseUp()
        {
            if (dragEvent == DragEventType.OnCharacter)
            {
                OnEndDrag(dragEvent);
                OnEndDrag();
            }
        }

        #endregion

        private void CalculateTopDownPosition()
        {
            if(myLayer == null)
            {
                myLayer = GetComponent<SortingGroup>();
            }
            myLayer.sortingOrder = (int)((transform.position.y) * -100);
        }
        private void DetectTouchingWindow()
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchBeginPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchPos = touchBeginPos;
                lastTouchPos = touchPos;
                OnBeginDrag(dragEvent);
                OnBeginDrag();
            }
            if (Input.GetMouseButton(0))
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                OnDrag(dragEvent);
                OnDrag();
            }
            if (Input.GetMouseButtonUp(0))
            {
                OnEndDrag(dragEvent);
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
                    OnDrag(dragEvent);
                    OnDrag();
                }

                if (Input.touchCount == 2)
                {
                    touch = Input.GetTouch(1);

                    if (touch.phase == TouchPhase.Began)
                    {
                        touchBeginPos = touch.position;
                        touchPos = touchBeginPos;
                        lastTouchPos = touchPos;
                        OnBeginDrag(dragEvent);
                        OnBeginDrag();
                    }

                    if (touch.phase == TouchPhase.Ended)
                    {
                        OnEndDrag(dragEvent);
                        OnEndDrag();
                    }
                }
            }
        }

        private void SwipeUp()
        {
            currentSwipingDirection = Direction.Up;
            OnSwipe();
        }
        private void SwipeDown()
        {
            currentSwipingDirection = Direction.Down;
            OnSwipe();
        }

        private void OnBeginDrag()
        {
            if (SwipingMode)
            {
                isSwiping = false;
            }
        }

        private void OnDrag()
        {
            if(DraggingMode)
            {
                dragForce = touchPos - lastTouchPos;

                if (dragForce.y > dragRange)
                {
                    OnDragging(dragForce);
                    lastTouchPos = touchPos;
                }
                if (dragForce.y < -dragRange)
                {
                    OnDragging(dragForce);
                    lastTouchPos = touchPos;
                }
                if (dragForce.x > dragRange)
                {
                    OnDragging(dragForce);
                    lastTouchPos = touchPos;
                }
                if (dragForce.x < -dragRange)
                {
                    OnDragging(dragForce);
                    lastTouchPos = touchPos;
                }
            }

            if (SwipingMode)
            {
                if (isSwiping) return;
                if (touchPos.y - touchBeginPos.y > swipeRange)
                {
                    isSwiping = true;
                    SwipeUp();
                }
                else if (-touchPos.y + touchBeginPos.y > swipeRange)
                {
                    isSwiping = true;
                    SwipeDown();
                }
            }
        }

        private void OnEndDrag()
        {
            if (SwipingMode)
            {
                isSwiping = false;
            }
        }

    }
}
