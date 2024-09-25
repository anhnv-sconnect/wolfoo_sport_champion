using SCN.HUD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WFSport.Base;

namespace WFSport.UI
{
    public class DialogSetting : Panel
    {
        [SerializeField] Button backBtn;
        [SerializeField] Slider musicSlider;
        [SerializeField] Slider soundSlider;
        protected override void Start()
        {
            base.Start();
            backBtn.onClick.AddListener(OnClickBackBtn);
            musicSlider.onValueChanged.AddListener(OnSlidingMusic);
            soundSlider.onValueChanged.AddListener(OnSlidingSound);
        }

        private void OnSlidingSound(float value)
        {
            SoundManager.Instance.SoundVolume = value;
        }

        private void OnSlidingMusic(float value)
        {
            SoundManager.Instance.MusicVolume = value;
        }
    }
}
