using SCN.HUD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport
{
    public class DialogPause : Panel
    {
        [SerializeField] Button backBtn;

        protected override void Start()
        {
            backBtn.onClick.AddListener(OnClickBackBtn);
        }
    }
}
