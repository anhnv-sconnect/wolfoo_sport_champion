using SCN.HUD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport
{
    public class DialogSetting : Panel
    {
        [SerializeField] Button backBtn;
        protected override void Start()
        {
            base.Start();
            backBtn.onClick.AddListener(OnClickBackBtn);
        }
    }
}
