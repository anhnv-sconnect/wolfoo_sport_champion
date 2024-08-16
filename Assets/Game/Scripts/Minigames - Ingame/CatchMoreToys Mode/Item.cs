using DG.Tweening;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    public abstract class Item : MonoBehaviour
    {
        protected abstract void Init();

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameObject trail;

        private Vector3 initialPosition;
        private float count;
        private float amplitude;
        private float speedX;

        private SpriteRenderer spriteRenderer;
        private Sequence _sequence;
        private Transform cart;
        private bool isFlying;
        private GameplayConfig config;
        private bool isTemporaryValue;
        protected bool isStoped;

        internal void Assign(GameplayConfig config, bool isTemporaryValue)
        {
            this.config = config;
            this.isTemporaryValue = isTemporaryValue;
        }

        private void Start()
        {
            initialPosition = transform.position;
            amplitude = UnityEngine.Random.Range(config.amplitudeRange.x, config.amplitudeRange.y);
            speedX = !isTemporaryValue ? UnityEngine.Random.Range(-config.speed.x, config.speed.x)
                : UnityEngine.Random.Range(-config.tempurareSpeed.x, config.tempurareSpeed.x);
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            Init();

            EventDispatcher.Instance.RegisterListener<Gameplay.EventKey.OnGameResume>(OnGameResume);
            EventDispatcher.Instance.RegisterListener<Gameplay.EventKey.OnGamePause>(OnGameStop);
        }
        private void OnDestroy()
        {
            _sequence?.Kill();

            EventDispatcher.Instance.RemoveListener<Gameplay.EventKey.OnGameResume>(OnGameResume);
            EventDispatcher.Instance.RemoveListener<Gameplay.EventKey.OnGamePause>(OnGameStop);
        }

        private void OnGameResume(EventKey.OnGameResume obj)
        {
            isFlying = true;
        }

        private void OnGameStop(EventKey.OnGamePause obj)
        {
            isFlying = false;
        }

        private void Update()
        {
            if (!isFlying || isStoped) return;

            float timeY = count * config.speed.y;
            float timeX = count * amplitude;
            float newX = initialPosition.x + speedX * Mathf.Sin(timeX);
            float newY = initialPosition.y - timeY;

            transform.position = new Vector2(newX, newY);
            transform.Rotate(Vector3.forward * (UnityEngine.Random.Range(config.torqueForceRange.x, config.torqueForceRange.y)));

            count += Time.deltaTime;

            EventManager.OnToyIsFlying?.Invoke(this);
        }
        protected virtual void OnTouchedGround()
        {
            StopFlying();
            isStoped = true;
            trail.SetActive(false);
            _sequence?.Kill();
            _sequence = DOTween.Sequence()
                .Append(spriteRenderer.DOFade(0, 1))
                .Join(transform.DOShakeScale(0.5f, 1, 2, 30))
                .AppendInterval(1)
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }
        protected void MoveToCart(Transform cart)
        {
            if (this.cart == null)
            {
                this.cart = cart;
                StopFlying();
                trail.SetActive(false);
                spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                transform.SetParent(this.cart);
            }
            transform.position = Vector2.Lerp(transform.position, this.cart.position, 0.1f);
        }

        internal virtual void StartFlying()
        {
            isFlying = true;
        }
        internal virtual void StopFlying()
        {
            isFlying = false;
        }
        public void Setup(Sprite sprite)
        {
            Init();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            CircleCollider2D circleCollider = spriteRenderer.GetComponent<CircleCollider2D>();
            circleCollider.radius = Mathf.Min(spriteRenderer.sprite.rect.width, spriteRenderer.sprite.rect.height) / 200;

            name = "Toy - " + sprite.name;
        }
    }
}
