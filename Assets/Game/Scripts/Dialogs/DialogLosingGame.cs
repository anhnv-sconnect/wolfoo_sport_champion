using SCN.HUD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport.UI
{
    public class DialogLosingGame: Panel
    {
        [SerializeField] Button backBtn;
        protected override void Start()
        {
            base.Start();
            backBtn.onClick.AddListener(OnClickBackBtn);
        }
    }
}
