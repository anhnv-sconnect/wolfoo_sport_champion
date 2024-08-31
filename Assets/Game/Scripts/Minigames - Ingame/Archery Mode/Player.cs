using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.ArcheryMode
{
    public class Player : Base.Player
    {
        [SerializeField] private GameplayConfig config;
        [SerializeField] private Bow bow;
        [SerializeField] private Arrow arrowPb;
        [SerializeField] private CharacterWorldAnimation wolfoo;

        private IMinigame.GameState gameState;

        private float flyTime;
        private bool canShooting;
        private int count = 0;
        private Arrow[] arrows;
        private Arrow currentArrow;
        private bool isSpecialMode;
        private bool isReloading;

        protected override IMinigame.GameState GameplayState { get => gameState; set => gameState = value; }
        public Vector3 BowPos { get => bow.transform.position; }

        #region UNITY METHODS

        #endregion

        #region MY METHODS

        internal void PlayWithSpecialItem()
        {
            isSpecialMode = true;
            StopCoroutine("CountDownAliveSpecialTime");
            StartCoroutine("CountDownAliveSpecialTime");
        }
        private IEnumerator CountDownAliveSpecialTime()
        {
            yield return new WaitForSeconds(config.specialAliveTime);
            isSpecialMode = false;
        }

        private void Shoot(Vector3 endPos)
        {
            Holder.PlaySound?.Invoke();
            wolfoo.PlayIdleAnim();
            wolfoo.PlayThrowbackAnim(false);
            bow.Shoot(endPos);
            currentArrow.Release(endPos, transform);

        }
        private void GetNextArrow()
        {
            var arrow = arrows[count];
            if (arrow == null)
            {
                arrow = Instantiate(arrowPb, arrowPb.transform.position, arrowPb.transform.rotation, arrowPb.transform.parent);
                arrow.Init();
            }
            else arrow = arrows[count];

            if (isSpecialMode) arrow.SetupSpecial(config.specialAliveTime);
            arrow.Setup(config.flySpeed, config.bowDrawingTime);
            currentArrow = arrow;
            arrows[count] = currentArrow;

            count++;
            if (count >= arrows.Length) count = 0;
        }
        private IEnumerator ReloadArrow()
        {
            isReloading = true;
            yield return new WaitForSeconds(config.reloadTime);
            isReloading = false;
        }
        private float GetFlyTime()
        {
            var maxOffset = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
            var maxX = Mathf.Max(maxOffset.x - bow.transform.position.x, bow.transform.position.x);
            var maxY = Mathf.Max(maxOffset.y - bow.transform.position.y, bow.transform.position.y);
            var maxDistance = Mathf.Max(maxX, maxY);
            var flyTime = maxDistance / config.flySpeed;

            return flyTime;
        }
        #endregion

        #region OVERRIDE METHODS
        public override void Init()
        {
            flyTime = config.flySpeed;
            // last 1 : interval time deactive
            var total = (flyTime / config.reloadTime) + 1 + 0.5f + config.bowDrawingTime;
            total = total == float.NaN ? 1 : total;

            arrows = new Arrow[(int)total];
            arrowPb.gameObject.SetActive(false);
            GetNextArrow();

            bow.Setup(config.bowDrawingTime);
        }

        public override void Lose()
        {
        }

        public override void OnDragging(Vector3 force)
        {
        }

        public override void OnSwipe()
        {
        }

        public override void OnTouching(Vector3 position)
        {
            if (gameState == IMinigame.GameState.Pausing) return;
            if (!canShooting) return;
            if (isReloading) return;

            if (position.y < bow.transform.position.y) return;

            position.z = 0;
            StartCoroutine("ReloadArrow");
            Shoot(position);
            GetNextArrow();
        }

        public override void OnUpdate()
        {
        }

        public override void Pause(bool isSystem)
        {
            canShooting = false;
        }

        public override void Play()
        {
            GameplayState = IMinigame.GameState.Playing;
            canShooting = true;
        }

        public override void ResetDefault()
        {
        }
        #endregion

    }
}
