using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;
using static UnityEditor.PlayerSettings;

namespace WFSport.Gameplay.BasketballMode
{
    public class Basket : MonoBehaviour
    {
        [SerializeField] Transform hole;
        [SerializeField] private float[] bombTimeLines = new float[] { };
        [SerializeField] private bool canMoveAround;

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
        private bool isPlaying;
        private Bomb bomb;
        private int countTime;
        private int timelineIdx;
        private GameplayConfig config;
        private float countX;
        private float countY;
        private float sinX;
        private float cosX;
        private float sinY;
        private float cosY;

        public Vector3 HolePos { get => hole.position; }
        public bool HasBomb { get; private set; }

        private void Start()
        {
            EventManager.OnThrow += OnPlayerThrow;
            EventManager.OnGetScore += OnPlayerGetScore;

            holeRange = transform.position.y - hole.position.y;
            screenPixelSize = ScreenHelper.GetMaxPizelSize();
            Show();
        }
        private void OnDestroy()
        {
            EventManager.OnThrow -= OnPlayerThrow;
            EventManager.OnGetScore -= OnPlayerGetScore;
        }

        private void OnPlayerGetScore(Base.Player player, Vector3 vector)
        {
            bomb.Hide();
        }

        private void Update()
        {
            if (isPlaying)
            {
                Calculate();
                var xPos = canMoveX ? (1 - cosX) * movingXRange / 2 + beginXPos : transform.position.x;
                var yPos = canMoveY ? (1 - cosY) * movingYRange / 2 + beginYPos : transform.position.y;
                transform.position = new Vector3(xPos, yPos, 0);

                countX += 1 * movingSpeed.x * Time.deltaTime;
                countY += 1 * movingSpeed.y * Time.deltaTime;
                if (countY > 2) countY = 0;
                if (countX > 2) countX = 0;
                Show();
            }
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

            if(countTime >= bombTimeLines[timelineIdx])
            {
                ShowBomb();
                timelineIdx++;
            }
        }
        internal void Setup(GameplayConfig config, Bomb bombPb)
        {
            this.config = config;

            CreateBomb(bombPb, config);

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

            Array.Sort(bombTimeLines);
        }

        private void CreateBomb(Bomb bombPb, GameplayConfig config)
        {
            if (bomb == null)
            {
                bomb = Instantiate(bombPb, transform);
                bomb.Setup(config);
            }
        }

        internal void ShowBomb()
        {
            HasBomb = true;
            bomb.Show();
        }

        internal void PlayMoveAround()
        {
            canMoveAround = true;
            isPlaying = true;
        }
        internal void StopMoveAround()
        {
            canMoveAround = false;
            isPlaying = false;
            StopCoroutine("DelayMoving");
        }

        internal void Show()
        {
            var yRange = Camera.main.WorldToScreenPoint(transform.position + Vector3.down * holeRange);
            transform.localScale = Vector3.one - Vector3.one * scaleRange * (yRange.y / screenPixelSize.y);
        }


        private void OnDelayAll(Basket basket)
        {
            StartCoroutine("DelayMoving");
        }

        private IEnumerator DelayMoving()
        {
            isPlaying = false;
            yield return new WaitForSeconds(ballFlyTime * 2);
            isPlaying = true;
        }

        private void OnPlayerThrow(Player player, Vector3 pos)
        {
            if (Vector2.Distance(pos, HolePos) <= distanceVerified)
            {
                if(canMoveAround)
                {
                    EventManager.DelayAll?.Invoke(this);
                }
                EventManager.OnBallTracking?.Invoke(player, this);
            }
        }
    }
}
