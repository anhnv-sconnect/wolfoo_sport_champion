using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
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
        private TweenerCore<Color, Color, ColorOptions> _tween;

        public bool IsPlaying { get => isPlaying; }

        // Start is called before the first frame update
        void Start()
        {
            if(spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.DOFade(0, 0);
            }
            initialScale = transform.localScale;
            length = spriteRenderer.sprite.rect.size.y / 100;
        }
        private void OnDestroy()
        {
            _tween?.Kill();
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
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.DOFade(0, 0);
        }
        internal void Play()
        {
            isPlaying = true;
            _tween = spriteRenderer.DOFade(1, 0.5f);
        }
        internal void Stop()
        {
            isPlaying = false;
            _tween = spriteRenderer.DOFade(0, 0.5f);
        }
        internal void Stop(Transform target)
        {
            if (target != player) return;
            isPlaying = false;
            _tween = spriteRenderer.DOFade(0, 0.5f);
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
