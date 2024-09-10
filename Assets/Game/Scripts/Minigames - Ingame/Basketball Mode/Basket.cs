using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    public class Basket : MonoBehaviour
    {
        [SerializeField] Transform hole;

        private float distanceVerified;
        private float scaleRange;
        private float movingYRange;
        private float movingXRange;
        private Vector2 movingSpeed;
        private float ballFlyTime;
        private float holeRange;
        private Vector2 screenPixelSize;
        [SerializeField]private float xPos;
        private float yPos;
        [SerializeField] private bool canMoveAround;
        private bool isPlaying;
        private float angle;
        private float angle2;

        public Vector3 HolePos { get => hole.position; }

        private void Start()
        {
            EventManager.OnThrow += OnPlayerThrow;
            //     EventManager.DelayAll += OnDelayAll;

            holeRange = transform.position.y - hole.position.y;
            screenPixelSize = ScreenHelper.GetMaxPizelSize();
            Show();
        }
        private void OnDestroy()
        {
            EventManager.OnThrow -= OnPlayerThrow;
        //    EventManager.DelayAll -= OnDelayAll;
        }

        private void Update()
        {
            if (isPlaying)
            {
                angle += 1 * movingSpeed.x;
                angle2 += 1 * movingSpeed.y;
                xPos = Mathf.Sin(angle) * movingXRange;
                yPos = Mathf.Cos(angle2) * movingYRange;
                transform.position = new Vector3(xPos, yPos, 0);
            }

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

        internal void Setup(GameplayConfig config)
        {
            distanceVerified = config.insideDistance;
            scaleRange = config.scaleRange;
            movingYRange = config.movingYRange.y - config.movingYRange.x;
            movingXRange = config.movingXRange.y - config.movingXRange.x;
            movingSpeed = config.movingSpeed;

            ballFlyTime = config.flyTime;

            angle = Mathf.Asin(transform.position.x / movingXRange) * Mathf.Rad2Deg;
            // Convert Current Pos => Radians
            xPos = Mathf.Sin(angle) * movingXRange;
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
