using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Helper;

namespace WFSport.Gameplay.BasketballMode
{
    public class Basket : MonoBehaviour
    {
        [System.Serializable]
        public struct BonusItemData
        {
            public float timeline;
            public int score;
        }

        [SerializeField] Transform hole;
        [SerializeField] Vector2[] movingTimelines;
        [SerializeField] BonusItemData[] bonusItemData;
        [SerializeField] float[] bombTimelines;
        [SerializeField] Bomb bombPb;
        [SerializeField] BonusItem bonusItemPb;

        private bool isPausing;
        private float distanceVerified;
        private float scaleRange;
        private bool canMoveY;
        private float movingYRange;
        private float movingXRange;
        private Vector2 movingSpeed;
        private float ballFlyTime;
        private bool canMoveX;
        private float holeRange;
        private float beginYPos;
        private float beginXPos;
        private Vector2 screenPixelSize;
        private float yPos;
        private bool isMoving;
        private int countTime;
        private float countX;
        private float countY;
        private float sinX;
        private float cosX;
        private float sinY;
        private float cosY;

        private (int moving, int bomb, int bonus) timelineID;
        private Bomb bomb;
        private BonusItem bonusItem;
        private GameplayConfig config;

        public Vector3 HolePos { get => hole.position; }
        public bool HasBomb { get => bomb.IsShowing; }
        public (bool isPlaying, int score) BonusItem { get => (bonusItem.IsShowing, bonusItemData[timelineID.bonus].score); }

        private void Start()
        {
            EventManager.OnThrow += OnPlayerThrow;
            EventManager.OnGetScore += OnPlayerGetScore;

            holeRange = transform.position.y - hole.position.y;
            screenPixelSize = ScreenHelper.GetMaxPizelSize();

            CreateBombAndBonusItem();
        }
        private void OnDestroy()
        {
            EventManager.OnThrow -= OnPlayerThrow;
            EventManager.OnGetScore -= OnPlayerGetScore;
        }
        private void Update()
        {
            if (isPausing) return;

            var yRange = Camera.main.WorldToScreenPoint(transform.position + Vector3.down * holeRange);
            transform.localScale = Vector3.one - Vector3.one * scaleRange * (yRange.y / screenPixelSize.y);

            if (isMoving)
            {
                Calculate();
                var xPos = canMoveX ? (1 - cosX) * movingXRange / 2 + beginXPos : transform.position.x;
                var yPos = canMoveY ? (1 - cosY) * movingYRange / 2 + beginYPos : transform.position.y;
                transform.position = new Vector3(xPos, yPos, 0);

                countX += 1 * movingSpeed.x * Time.deltaTime;
                countY += 1 * movingSpeed.y * Time.deltaTime;
                if (countY > 2) countY = 0;
                if (countX > 2) countX = 0;
            }
        }

        private void OnPlayerGetScore(Base.Player player, Vector3 vector)
        {
            bomb.Hide();
        }
        private void Calculate()
        {
            sinX = Mathf.Sin(countX * Mathf.PI);
            cosX = Mathf.Cos(countX * Mathf.PI);
            sinY = Mathf.Sin(countY * Mathf.PI);
            cosY = Mathf.Cos(countY * Mathf.PI);
        }
        private IEnumerator CountTime()
        {
            yield return new WaitForSeconds(1);
            countTime++;

            if(countTime >= bombTimelines[timelineID.bomb])
            {
                bomb.Show();
                timelineID.bomb++;
            }

            if(countTime >= movingTimelines[timelineID.moving].x)
            {
                isMoving = true;
            }
            else if (countTime <= movingTimelines[timelineID.moving].y)
            {
                isMoving = false;
                timelineID.moving++;
            }

            if(countTime >= bonusItemData[timelineID.bonus].timeline)
            {
            }
        }
        internal void Setup(GameplayConfig config)
        {
            this.config = config;

            distanceVerified = config.insideDistance;
            scaleRange = config.scaleRange;
            movingSpeed = config.movingSpeed;
            ballFlyTime = config.flyTime;
            canMoveX = config.canMoveX;
            canMoveY = config.canMoveY;

            movingYRange = (config.movingYRange.y - config.movingYRange.x);
            movingXRange = (config.movingXRange.y - config.movingXRange.x);
            beginYPos = config.movingYRange.x;
            beginXPos = config.movingXRange.x;
            countY = Mathf.Abs((transform.position.y - beginYPos) / movingYRange);
            countX = Mathf.Abs((transform.position.x - beginXPos) / movingXRange);
        }

        private void CreateBombAndBonusItem()
        {
            if (bomb == null)
            {
                bomb = Instantiate(bombPb, transform);
                bomb.Setup(config);
            }
            if (bonusItem == null)
            {
                bonusItem = Instantiate(bonusItemPb, transform);
                bonusItem.Setup(config);
            }
        }

        internal void Play()
        {
            isPausing = false;
            StartCoroutine("CountTime");
            bomb.Play();
        }
        internal void Pause()
        {
            isPausing = true;
            StopCoroutine("DelayMoving");
            StopCoroutine("CountTime");
            bomb.Pause();
        }


        private void OnDelayAll(Basket basket)
        {
            StartCoroutine("DelayMoving");
        }

        private IEnumerator DelayMoving()
        {
            isMoving = false;
            yield return new WaitForSeconds(ballFlyTime * 2);
            isMoving = true;
        }

        private void OnPlayerThrow(Player player, Vector3 pos)
        {
            if (Vector2.Distance(pos, HolePos) <= distanceVerified)
            {
                EventManager.OnBallTracking?.Invoke(player, this);
            }
        }
    }
}
