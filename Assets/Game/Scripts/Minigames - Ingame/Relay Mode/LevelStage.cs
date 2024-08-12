using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.RelayMode
{
    public class LevelStage : MonoBehaviour
    {
        [SerializeField] private Player.Mode mode;
        [SerializeField] private Transform beginPoint;
        [SerializeField] private FinishPointing finisher;

        private bool isDispatched;
        private Base.Player player;
        private bool canTracking = true;

        public bool IsFinal { get; private set; }
        public Vector2 HalfPos => ((finisher.transform.position - beginPoint.position) / 2f);
        public Transform FinisherPoint { get => finisher.transform; }
        public Transform BeginerPoint { get => beginPoint; }
        public Player.Mode Mode { get => mode; }

        private void OnEnable()
        {
            EventManager.OnPlayerIsMoving += OnPlayerIsMoving;
        }
        private void OnDisable()
        {
            EventManager.OnPlayerIsMoving -= OnPlayerIsMoving;
        }

        private void OnPlayerIsMoving(Base.Player player)
        {
            if (!canTracking || this.player != player) return;
            StartCoroutine("OnTrackingPlayer");
        }
        private IEnumerator OnTrackingPlayer()
        {
            canTracking = false;
            yield return new WaitForEndOfFrame();
            canTracking = true;

            if (!isDispatched && player.transform.position.x > HalfPos.x)
            {
                isDispatched = true;
                EventManager.OnPlayerIsPassedHalfStage?.Invoke(player);
            }
        }

        internal void Assign(Player player)
        {
            this.player = player;
        }
        internal void Assign(bool isLastStage)
        {
            if (isLastStage) finisher.SetFinal();
            else finisher.SetNormal();

            IsFinal = isLastStage;
        }
    }
}
