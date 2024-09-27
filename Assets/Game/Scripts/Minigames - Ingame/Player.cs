using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Rendering;
using static WFSport.Gameplay.IPlayer;
using static WFSport.Gameplay.IMinigame;
using WFSport.Helper;

namespace WFSport.Gameplay.Base
{
    public abstract class Player: MonoBehaviour
    {
        public enum DragEventType
        {
            OnScreen,
            OnCharacter
        }
        public abstract void Play();
        public abstract void Lose();
        public abstract void Pause(bool isSystem);
        public abstract void Init();
        public abstract void ResetDefault();
        public abstract void OnUpdate();
        public abstract void OnSwipe();
        public abstract void OnDragging(Vector3 force);
        /// <summary>
        /// Call When first Touching on screen
        /// </summary>
        /// <param name="position"></param>
        public abstract void OnTouching(Vector3 position);
        /// <summary>
        /// Pause & Stop State will stop everything in Player
        /// </summary>
        protected abstract GameState GameplayState { get; set; }

        [SerializeField] private DragEventType dragEvent;
        [SerializeField] private DetectType detectType;
        [SerializeField][ShowIf("detectType", DetectType.Dragging)] 
        private float dragRange = 0.2f;
        [SerializeField][ShowIf("detectType", DetectType.Swiping)]
        private float swipeRange = 0.5f;
        [SerializeField] bool enableTopDownPosition;

        protected Direction currentSwipingDirection;

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
            if (GameplayState != GameState.Playing) return;
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
            if (GameplayState != GameState.Playing) return;
            if(dragEvent == DragEventType.OnCharacter)
            {
                touchBeginPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchPos = touchBeginPos;
                lastTouchPos = touchPos;
                OnBeginDrag();
            }
        }
        private void OnMouseDrag()
        {
            if (GameplayState != GameState.Playing) return;
            if (dragEvent == DragEventType.OnCharacter)
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                OnDrag();
            }
        }
        private void OnMouseUp()
        {
            if (GameplayState != GameState.Playing) return;
            if (dragEvent == DragEventType.OnCharacter)
            {
                OnEndDrag();
            }
        }

        #endregion
        public static int SortingLayer(Vector2 position)
        {
            return (int)(position.y * -100);
        }

        private void CalculateTopDownPosition()
        {
            if(myLayer == null)
            {
                myLayer = GetComponent<SortingGroup>();
            }
            myLayer.sortingOrder = SortingLayer(transform.position);
        }
        private void DetectTouchingWindow()
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchBeginPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchPos = touchBeginPos;
                lastTouchPos = touchPos;
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

                if(detectType == DetectType.Dragging || detectType == DetectType.Swiping)
                {
                    Debug.Log("Dragging Mode");

                    if (touch.phase == TouchPhase.Began)
                    {
                        Debug.Log("Began Dragging");
                        touchBeginPos = ScreenHelper.GetMousePos();
                        touchPos = touchBeginPos;
                        lastTouchPos = touchPos;
                        OnBeginDrag();
                    }
                    if (touch.phase == TouchPhase.Ended)
                    {
                        Debug.Log("End Dragging");
                        OnEndDrag();
                    }
                    if (touch.phase == TouchPhase.Moved)
                    {
                        Debug.Log("Dragging");
                        touchPos = ScreenHelper.GetMousePos();
                        OnDrag();
                    }
                }
                else if (detectType == DetectType.Clicking)
                {
                    Debug.Log("Click Mode");
                    if(touch.phase == TouchPhase.Stationary)
                    {
                    Debug.Log("Click Stationary");
                        touchPos = touchBeginPos;
                        touchPos.z = 0;
                        OnTouching(touchPos);
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

        protected virtual void OnBeginDrag()
        {
            OnTouching(touchPos);
            if (SwipingMode)
            {
                isSwiping = false;
            }
        }

        protected virtual void OnDrag()
        {
            OnTouching(touchPos);
            if(DraggingMode)
            {
                dragForce = (touchPos - lastTouchPos) * Time.deltaTime * 10000;

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

        protected virtual void OnEndDrag()
        {
            OnTouching(touchPos);
            if (SwipingMode)
            {
                isSwiping = false;
            }
        }

    }
}
