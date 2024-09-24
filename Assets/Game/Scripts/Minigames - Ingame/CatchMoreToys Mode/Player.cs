using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    public class Player : Base.Player
    {
        [SerializeField] private GameplayConfig config;
        [SerializeField] private TMPro.TMP_Text scoreTxt;
        [SerializeField] private Transform cart;
        [SerializeField] private CharacterWorldAnimation[] characters;

        private int score;
        private IMinigame.GameState gameState;
        private Vector3 inititalPos;
        private bool isStunning;
        private Sequence _sequence;
        private bool isPausing;

        protected override IMinigame.GameState GameplayState { get => gameState; set => gameState = value; }
        internal Transform Cart { get => cart; }
        public int Score { get => score; }

        #region MY METHODS
        private void UpdateScore()
        {
            scoreTxt.text = score < 10 ? $"0{score}" : $"{score}";
        }
        private IEnumerator OnStunning()
        {
            isStunning = true;
            foreach (var item in characters)
            {
                item.PlayDizzyAnim(true);
            }
            yield return new WaitForSeconds(config.stunningTime);
            isStunning = false;
            foreach (var item in characters)
            {
                item.PlayIdleAnim();
            }
        }
        private void OnGetItem()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence()
                .AppendInterval(0.2f)
                .Append(cart.DOScale(0.8f, 0.25f).OnStart(() => cart.localScale = Vector3.one))
                .Append(cart.DOScale(1, 0.25f));
        }
        #endregion

        #region Unity Methods
        void Start()
        {
            Init();
        }
        private void OnDestroy()
        {
            _sequence?.Kill();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(TAG.BONUSITEM))
            {
                score += 2;
                UpdateScore();
                OnGetItem();
                EventManager.OnPlayerClaimNewStar?.Invoke(this, true);
            }
            else if (collision.CompareTag(TAG.STAR))
            {
                score++;
                UpdateScore();
                OnGetItem();
                EventManager.OnPlayerClaimNewStar?.Invoke(this, false);
            }
            else if(collision.CompareTag(TAG.OBSTACLE))
            {
                OnGetItem();
                StopCoroutine("OnStunning");
                StartCoroutine("OnStunning");
            }
        }
        #endregion

        #region Override Methods
        public override void Init()
        {
            inititalPos = transform.position;
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].PlayIdleAnim();
            }
            score = 0;
            UpdateScore();
        }

        public override void Lose()
        {
        }

        public override void OnDragging(Vector3 force)
        {
            if (isStunning || isPausing) return;

            var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(position.x, inititalPos.y, inititalPos.z);
            if (transform.position.x < config.limitLeft) transform.position = new Vector3(config.limitLeft, inititalPos.y, inititalPos.z);
            if (transform.position.x > config.limitRight) transform.position = new Vector3(config.limitRight, inititalPos.y, inititalPos.z);

            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].PlayPushAnim();
            }
        }
        protected override void OnEndDrag()
        {
            base.OnEndDrag();

            if (isStunning || isPausing) return;

            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].PlayIdleAnim();
            }
        }

        public override void Pause(bool isSystem)
        {
            isPausing = true;
        }

        public override void OnSwipe()
        {
        }

        public override void OnTouching(Vector3 position)
        {
        }

        public override void OnUpdate()
        {
        }

        public override void Play()
        {
            isPausing = false;
            gameState = IMinigame.GameState.Playing;
        }

        public override void ResetDefault()
        {
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].PlayIdleAnim();
            }
            score = 0;
            UpdateScore();

            foreach (var item in cart.GetComponentsInChildren<Item>())
            {
                Destroy(item.gameObject);
            }
        }
        #endregion
    }
}
