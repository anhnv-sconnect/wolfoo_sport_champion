using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WFSport.Helper;

namespace WFSport.Home
{
    public class EllipseLayout : MonoBehaviour
    {
        [Header("================ CONFIG ================")]
        [SerializeField] private float speed;
        [SerializeField] private float a, b;
        [SerializeField] private Vector2 limit;
        [Range(-1, 1)][SerializeField] private int direction = 1;
        [SerializeField] private bool run;

        private float range;
        private float[] itemCounts;
        private Vector3 lastPos;
        private Vector3 curPos;

        private Transform[] items;
        private EventTrigger trigger;
        private Vector3 center => transform.position;
        internal Transform ItemHolder { get => transform; }
        
        private void Start()
        {
            RegisterDragEvent();
        }
        private void OnDestroy()
        {
            RemoveDragEvent();
        }
        private void Update()
        {
            if (!run) return;
            for (int i = 0; i < items.Length; i++)
            {
                Count(ref itemCounts[i]);
                items[i].position = CalculatePos(itemCounts[i] + range * i);
            }
        }
        internal void Setup(Transform[] items)
        {
            this.items = items;
            InitItem();
        }

        private void RegisterDragEvent()
        {
            if (trigger == null) trigger = GetComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.BeginDrag;
            entry.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;
            entry.callback.AddListener((data) => { OnEndDrag((PointerEventData)data); });
            trigger.triggers.Add(entry);
        }
        private void RemoveDragEvent()
        {
            if (trigger != null) trigger.triggers.Clear();
        }

        void InitItem()
        {
            range = (limit.y - limit.x) / (items.Length - 1);
            itemCounts = new float[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                var normalizeValue = range * (i);
                itemCounts[i] = normalizeValue;
                items[i].position = CalculatePos(itemCounts[i] + range * i);
            }
        }

        private void Count(ref float value)
        {
            value += 1 * speed * Time.deltaTime * direction;
      //      if (value >= limit.y) value = limit.x;
       //     if (value <= limit.x) value = limit.y;
        }

        private Vector3 CalculatePos(float value)
        {
            var x = a * Mathf.Cos(value * Mathf.PI);
            var y = b * Mathf.Sin(value * Mathf.PI);

            return new Vector3(x, y, 0) + center;
        }

        private void OnBeginDrag(PointerEventData eventData)
        {
            curPos = ScreenHelper.GetMousePos();
            lastPos = curPos;
            run = true;
        }

        private void OnDrag(PointerEventData eventData)
        {
            curPos = ScreenHelper.GetMousePos();
            if (curPos.x < lastPos.x) direction = 1;
            else direction = -1;

            if (Vector2.Distance(curPos, lastPos) > 0.15f)
            {
                lastPos = curPos;
                run = true;
            }
            else
            {
                run = false;
            }

        }

        private void OnEndDrag(PointerEventData eventData)
        {
            run = false;
        }
    }
}
