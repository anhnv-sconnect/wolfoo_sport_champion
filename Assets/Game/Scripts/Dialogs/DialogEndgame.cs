using SCN.HUD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WFSport.Base;

namespace WFSport.UI
{
    public class DialogEndgame : Panel
    {
        [SerializeField] ParticleSystem[] conffetieFxs;
        [SerializeField] Button backBTn;
        protected override void Start()
        {
            base.Start();
            backBTn.onClick.AddListener(OnClickBackBtn);
        }
    }
}
