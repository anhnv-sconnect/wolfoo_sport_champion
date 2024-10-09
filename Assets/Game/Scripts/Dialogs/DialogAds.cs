using SCN.HUD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport.UI
{
    public class DialogAds : Panel
    {
        [SerializeField] Button backBtn;
        [SerializeField] Button adsBtn;
        [SerializeField] Button cancelBtn;

        protected override void Start()
        {
            base.Start();
            backBtn.onClick.AddListener(OnClickBack);
            cancelBtn.onClick.AddListener(OnClickBack);
            adsBtn.onClick.AddListener(OnWatchAds);
        }

        private void OnWatchAds()
        {

        }

        private void OnClickBack()
        {
            base.Hide();
        }
    }
}
