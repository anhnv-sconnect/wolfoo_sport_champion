using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.LatinDanceMode
{
    public class Spotlight : MonoBehaviour
    {
        [SerializeField] Transform player;
        private Vector3 initialScale;
        private float length;
        private SpriteRenderer spriteRenderer;
        private bool isPlaying;

        // Start is called before the first frame update
        void Start()
        {
            initialScale = transform.localScale;
            spriteRenderer = GetComponent<SpriteRenderer>();
            length = spriteRenderer.sprite.rect.size.y / 100;
        }


        // Update is called once per frame
        void Update()
        {
            if (isPlaying)
            {
                CalculateDistance();
                CalculateRotation();
            }
        }

        internal void Setup(Transform target)
        {
            player = target;
        }
        internal void Play()
        {
            isPlaying = true;
        }
        internal void Stop()
        {
            isPlaying = false;
        }

        private void CalculateDistance()
        {
            var distance = (transform.position - player.position).magnitude;
            var value = distance / length;
            transform.localScale = new Vector3(initialScale.x, value, initialScale.z);
        }
        private void CalculateRotation()
        {
            var direction = transform.position - player.position;
            var value = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Quaternion angle = Quaternion.AngleAxis(value, Vector3.forward);
            transform.rotation = angle;
        }
    }
}
