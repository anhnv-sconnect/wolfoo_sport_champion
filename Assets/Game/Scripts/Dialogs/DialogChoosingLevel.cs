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
    public class DialogChoosingLevel : Panel
    {
        [SerializeField] int totalLevel;
        [SerializeField] Button[] buttons;
        [SerializeField] Button backBtn;

        protected override void Start()
        {
            base.Start();
            Init();
        }

        private void Init()
        {
            backBtn.onClick.AddListener(OnClickBackBtn);
            for (int i = 0; i < buttons.Length; i++)
            {
                var idx = i;
                buttons[idx].onClick.AddListener(() => OnClickLevelItem(idx));
            }
        }

        private void OnClickLevelItem(int idx)
        {
            EventDispatcher.Instance.Dispatch(new EventKeyBase.OnChoosing
            {
                id = idx + 1,
                dialogName = AnhNV.GameBase.PopupManager.DialogName.ChoosingLevel
            });
            base.Hide();
        }
    }
}
