using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    public abstract class Item : MonoBehaviour
    {
        protected abstract void Init();

        [SerializeField] private Vector2 amplitudeRange = new Vector2(2,2.5f);
        [SerializeField] private Vector2 speed = new Vector2(3, 4);
        [SerializeField] private Vector2 torqueForceRange = new Vector2(0.5f, 1.5f);
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameObject trail;

        private Vector3 initialPosition;
        private float count;
        private float amplitude;
        private float speedX;

        private SpriteRenderer spriteRenderer;
        private bool isTouchedGround;
        private Sequence _sequence;
        private Transform cart;
        private bool isFlying;

        private void Start()
        {
            initialPosition = transform.position;
            amplitude = UnityEngine.Random.Range(amplitudeRange.x, amplitudeRange.y);
            speedX = UnityEngine.Random.Range(-speed.x, speed.x);
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            Init();
        }
        private void Update()
        {
            if (!isFlying) return;

            float timeY = count * speed.y;
            float timeX = count * speedX;
            float newX = initialPosition.x + amplitude * Mathf.Sin(timeX);
            float newY = initialPosition.y - timeY;

            transform.position = new Vector2(newX, newY);
            transform.Rotate(Vector3.forward * (UnityEngine.Random.Range(torqueForceRange.x, torqueForceRange.y)));

            count += Time.deltaTime;
        }
        private void OnDestroy()
        {
            _sequence?.Kill();
        }
        protected virtual void OnTouchedGround()
        {
            StopFlying();
            trail.SetActive(false);
            isTouchedGround = true;
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
            if (isTouchedGround) return;
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
            if (isTouchedGround) return;
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
