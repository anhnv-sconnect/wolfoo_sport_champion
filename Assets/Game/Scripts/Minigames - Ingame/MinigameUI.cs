using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace WFSport.Gameplay
{
    public class MinigameUI : MonoBehaviour
    {
        [SerializeField] Image fillBar;
        private TweenerCore<float, float, FloatOptions> _tweenLoadingBar;

        private void Start()
        {
        }
        private void OnDestroy()
        {
            _tweenLoadingBar?.Kill();
        }
        private void OnEnable()
        {
            EventManager.OnInitGame += InitScreen;
        }
        private void OnDisable()
        {
            EventManager.OnInitGame -= InitScreen;
        }

        private void InitScreen()
        {
            fillBar.fillAmount = 0;
        }

        internal void UpdateLoadingBar(float value)
        {
            _tweenLoadingBar?.Kill();
            _tweenLoadingBar = fillBar.DOFillAmount(value, 0.5f);
        }
    }
}
