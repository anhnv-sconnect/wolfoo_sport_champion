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

        protected override IMinigame.GameState GameplayState { get => gameState; set => gameState = value; }
        internal Transform Cart { get => cart; }

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
                .Append(cart.DOShakeScale(0.5f, 1, 2, 30).OnStart(() => cart.transform.localScale = Vector3.one));
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
           //     OnGetItem();
            }
            if (collision.CompareTag(TAG.STAR))
            {
                score++;
                UpdateScore();
          //      OnGetItem();
            }
            if (collision.CompareTag(TAG.OBSTACLE))
            {
                Debug.Log("Collider with Obstacle");
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
        }

        public override void Lose()
        {
        }

        public override void OnDragging(Vector3 force)
        {
            if (isStunning) return;

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

            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].PlayIdleAnim();
            }
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
        }

        public override void ResetDefault()
        {
        }
        #endregion
    }
}
