using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.RelayMode
{
    public class LevelStage : MonoBehaviour
    {
        [SerializeField] Player.Mode mode;

        private FinishPointing finisher;
        private Vector3 halfPos;
        private bool isDispatched;

        public bool IsFinal { get; private set; }
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
            if(!isDispatched && player.transform.position.x > halfPos.x)
            {
                isDispatched = true;
                EventManager.OnPlayerIsPassedHalfStage?.Invoke(player);
            }
        }

        internal void Assign(bool isLastStage)
        {
            finisher = GetComponentInChildren<FinishPointing>();
            if (isLastStage) finisher.SetFinal();
            else finisher.SetNormal();

            halfPos = finisher.transform.position - transform.position;
            IsFinal = isLastStage;
        }
    }
}
