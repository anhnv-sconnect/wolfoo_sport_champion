using SCN;
using SCN.HUD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WFSport.Base;

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
            if (backBtn != null) backBtn.onClick.AddListener(OnClickBack);
            if (cancelBtn != null) cancelBtn.onClick.AddListener(OnClickBack);
            if (adsBtn != null) adsBtn.onClick.AddListener(OnWatchAds);
        }

        private void OnWatchAds()
        {
            base.Hide();
            if (AdsManager.Instance.HasRewardVideo)
            {
                AdsManager.Instance.ShowRewardVideo(() =>
                {
                    EventDispatcher.Instance.Dispatch(new EventKeyBase.OnWatchAds { });
                });
            }
            else
            {
                EventDispatcher.Instance.Dispatch(new EventKeyBase.OpenDialog { dialog = AnhNV.GameBase.PopupManager.DialogName.NoAds });
            }
        }

        private void OnClickBack()
        {
            base.Hide();
        }
    }
}
