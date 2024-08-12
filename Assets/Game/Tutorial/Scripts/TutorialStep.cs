using SCN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static AnhNV.GameBase.AnimatorHelper;

namespace AnhNV.GameBase
{
    public abstract class TutorialStep : MonoBehaviour
    {
        public abstract string TutorialID { get; set; }
        public abstract void Play();
        public abstract void Stop();
        public abstract void Release();

        public bool IsPlaying { get; protected set; }
        public System.Action OnTutorialComplete;
        public System.Action OnClickTutorial;

        [SerializeField] EventTrigger eventTrigger;
        private bool isSwiping;
        private Vector3 touchBeginPos;
        private Vector3 touchPos;

        protected void RegisterSwipeEvent()
        {
            if(eventTrigger == null)
            {
                Debug.LogError("Event Trigger is NOT Declared !");
                return;
            }

            CreateEntry(EventTriggerType.BeginDrag, OnBeginDrag);
            CreateEntry(EventTriggerType.Drag, OnDrag);
            CreateEntry(EventTriggerType.EndDrag, OnEndDrag);
        }
        private void CreateEntry(EventTriggerType id, UnityAction<BaseEventData> method)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = id;
            entry.callback.AddListener(method);
            eventTrigger.triggers.Add(entry);
        }
        protected void RemoveSwipeEvent()
        {
            if (eventTrigger == null)
            {
                Debug.LogError("Event Trigger is NOT Declared !");
                return;
            }

            for (int i = 0; i < eventTrigger.triggers.Count; i++)
            {
                eventTrigger.triggers.RemoveAt(i);
            }
        }

        protected virtual void OnSwiping(Direction direction)
        {

        }

        private void OnBeginDrag(BaseEventData data)
        {
            touchBeginPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchPos = touchBeginPos;

            isSwiping = false;
        }

        private void OnDrag(BaseEventData data)
        {
            touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (isSwiping) return;
            if (touchPos.y - touchBeginPos.y > 2)
            {
                isSwiping = true;
                OnSwiping(Direction.Up);
            }
            else if (-touchPos.y + touchBeginPos.y > 2)
            {
                isSwiping = true;
                OnSwiping(Direction.Down);
            }
        }

        private void OnEndDrag(BaseEventData data)
        {
            isSwiping = false;
        }
    }
}
