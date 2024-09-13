using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Gameplay.IMinigame;

namespace WFSport.Helper
{
    public abstract class PlayerInputHelper : MonoBehaviour
    {
        public abstract void OnUpdate();
        public abstract void OnSwipe();
        public abstract void OnDragging(Vector3 force);
        public abstract void OnTouching(Vector3 position);

        private Vector3 touchBeginPos;
        private Vector3 touchPos;
        private Vector3 lastTouchPos;
        private bool isSwiping;
        private Vector3 dragForce;

        public GameState GameplayState { get; private set; }

        #region UNITY METHODS

        private void Update()
        {
            if (GameplayState != GameState.Playing) return;
            OnUpdate();
        }

        private void OnMouseDown()
        {
            if (GameplayState != GameState.Playing) return;

            touchBeginPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchPos = touchBeginPos;
            lastTouchPos = touchPos;
            OnBeginDrag();
        }
        private void OnMouseDrag()
        {
            if (GameplayState != GameState.Playing) return;
            touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            OnDrag();
        }
        private void OnMouseUp()
        {
            if (GameplayState != GameState.Playing) return;
            OnEndDrag();
        }

        #endregion
        public static int SortingLayer(Vector2 position)
        {
            return (int)(position.y * -100);
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

                if (touch.phase == TouchPhase.Stationary)
                {
                    touchPos = touchBeginPos;
                    touchPos.z = 0;
                    OnTouching(touchPos);
                }

                if (Input.touchCount == 2)
                {
                    touch = Input.GetTouch(1);

                    if (touch.phase == TouchPhase.Began)
                    {
                        touchBeginPos = touch.position;
                        touchPos = touchBeginPos;
                        lastTouchPos = touchPos;
                        OnBeginDrag();
                    }

                    if (touch.phase == TouchPhase.Ended)
                    {
                        OnEndDrag();
                    }
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    touchPos = touch.position;
                    OnDrag();
                }
            }
        }


        protected virtual void OnBeginDrag()
        {
            OnTouching(touchPos);
        }

        protected virtual void OnDrag()
        {
            dragForce = (touchPos - lastTouchPos) * Time.deltaTime * 10000;
            OnDragging(dragForce);

        }

        protected virtual void OnEndDrag()
        {
            OnTouching(touchPos);
        }
    }
}
