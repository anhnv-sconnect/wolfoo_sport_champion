using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    public class Basket : MonoBehaviour
    {
        [SerializeField] Transform hole;
        [SerializeField] private float[] bombTimeLines = new float[] { };

        private float distanceVerified;
        private float scaleRange;
        private float movingYRange;
        private float movingXRange;
        private Vector2 movingSpeed;
        private float ballFlyTime;
        private float holeRange;
        private float beginYPos;
        private Vector2 screenPixelSize;
        [SerializeField]private float xPos;
        private float yPos;
        [SerializeField] private bool canMoveAround;
        private bool isPlaying;
        private float angle;
        private float angle2;
        private Bomb bomb;
        private int countTime;
        private int timelineIdx;

        public Vector3 HolePos { get => hole.position; }
        public bool HasBomb { get; private set; }

        private void Start()
        {
            EventManager.OnThrow += OnPlayerThrow;

            holeRange = transform.position.y - hole.position.y;
            screenPixelSize = ScreenHelper.GetMaxPizelSize();
            Show();
        }
        private void OnDestroy()
        {
            EventManager.OnThrow -= OnPlayerThrow;
        }

        private void Update()
        {
            if (isPlaying)
            {
                angle += 1 * movingSpeed.x;
                angle2 += 1 * movingSpeed.y;
                //   xPos = Mathf.Sin(angle) * movingXRange;
                yPos = Mathf.Cos(angle2) * movingYRange + movingYRange + beginYPos;
                transform.position = new Vector3(xPos, yPos, 0);

                Show();
            }
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
            CreateBomb(bombPb, config);

            distanceVerified = config.insideDistance;
            scaleRange = config.scaleRange;
            movingYRange = (config.movingYRange.y - config.movingYRange.x)/2;
            movingXRange = (config.movingXRange.y - config.movingXRange.x)/2;
            movingSpeed = config.movingSpeed;
            beginYPos = config.movingYRange.x;
            ballFlyTime = config.flyTime;

            //     angle = Mathf.Asin(transform.position.x / movingXRange) * Mathf.Rad2Deg;
            //    angle2 = Mathf.Acos(transform.position.x / movingYRange) * Mathf.Rad2Deg;
            // Convert Current Pos => Radians
            //     xPos = Mathf.Sin(angle) * movingXRange;
            //      yPos = Mathf.Sin(angle2) * movingYRange;
            xPos = transform.position.x;

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
