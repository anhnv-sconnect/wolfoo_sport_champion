using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Helper
{
    public class Circle: MonoBehaviour
    {
        [SerializeField] private Transform pb;
        [SerializeField] private int totalStar;
        [SerializeField] private float speed;
        [SerializeField] private Vector2 dimension;
        [SerializeField][Range(-1, 1)] private float direction;

        private Transform[] stars;
        private float[] itemCounts;
        private bool canPlay;
        private float range;

        private void Start()
        {
            range = (dimension.x/2) / (totalStar+1);
            itemCounts = new float[totalStar];
            stars = new Transform[totalStar];

            for (int i = 0; i < totalStar; i++)
            {
                var normalizeValue = range * (i);
                itemCounts[i] = normalizeValue;
                var star = Instantiate(pb, transform);
                stars[i] = star;
                stars[i].position = CalculatePos(itemCounts[i] + range * i);
            }
        }
        private void Update()
        {
            if (!canPlay) return;
            for (int i = 0; i < totalStar; i++)
            {
                Count(ref itemCounts[i]);
                stars[i].position = CalculatePos(itemCounts[i] + range * i);
            }
        }
        private void Count(ref float value)
        {
            value += 1 * speed * Time.deltaTime * direction;
        }

        private Vector3 CalculatePos(float value)
        {
            var x = dimension.x * Mathf.Cos(value * Mathf.PI);
            var y = dimension.y * Mathf.Sin(value * Mathf.PI);

            return new Vector3(x, y, 0) + transform.position;
        }
        [NaughtyAttributes.Button]
        public void Play()
        {
            canPlay = true;
        }
        public void Stop()
        {
            canPlay = false;
        }

    }
}
