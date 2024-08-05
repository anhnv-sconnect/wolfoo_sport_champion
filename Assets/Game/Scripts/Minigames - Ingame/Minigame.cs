using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport.Gameplay
{
    public abstract class Minigame : MonoBehaviour
    {
        [SerializeField] Button backBtn;

        protected IMinigame.GameState CurrentState;
        private IMinigame.Data dataExport;

        private void OnEnable()
        {
            RegisterEvent();
        }

        private void OnDisable()
        {
            RemoveEvent();
        }

        protected virtual void RegisterEvent()
        {
            // On PausingPanel Opened


            backBtn.onClick.AddListener(OnClickBackBtn);
        }

        protected virtual void RemoveEvent()
        {
            // On PausingPanel Opened


            backBtn.onClick.RemoveListener(OnClickBackBtn);
        }

        protected virtual void OnClickBackBtn()
        {
            CurrentState = IMinigame.GameState.Pausing;
            // Begin Open Pause Panel
        }
    }
}
