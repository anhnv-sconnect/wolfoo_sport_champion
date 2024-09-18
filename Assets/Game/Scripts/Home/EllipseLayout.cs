using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.Home
{
    public class EllipseLayout : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Transform[] items;
        public float speed;
        public float a, b;
        public Vector2 limit;
        public int direction = 1;
        public bool run;

        private float count;
        private float x, y;
        private float range;
        private Vector3 center => transform.position;
        private float[] itemCounts;

        private void OnDrawGizmos()
        {
            //Gizmos.color = Color.yellow;
            //for (float i = limit.x; i <= limit.y; i += 0.001f * speed)
            //{
            //    var x = a * Mathf.Cos(i * Mathf.PI);
            //    var y = b * Mathf.Sin(i * Mathf.PI);
            //    Gizmos.DrawLine(transform.position, new Vector3(x, y) + transform.position);
            //}
        }
        private void Start()
        {
            range = (limit.y - limit.x) / items.Length;
            itemCounts = new float[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                var normalizeValue = range * i;
                items[i].position = CalculatePos(normalizeValue);
                itemCounts[i] = normalizeValue;
            }
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

        private void Count(ref float value)
        {
            value += 1 * speed * Time.deltaTime * direction;
            if (value >= limit.y) count = limit.x;
            if (value <= limit.x) count = limit.y;
        }

        private Vector3 CalculatePos(float value)
        {
            var x = a * Mathf.Cos(value * Mathf.PI);
            var y = b * Mathf.Sin(value * Mathf.PI);

            return new Vector3(x, y, 0) + center;
        }
    }
}
