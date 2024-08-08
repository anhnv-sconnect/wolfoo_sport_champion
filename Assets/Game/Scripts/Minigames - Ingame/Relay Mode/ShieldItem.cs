using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.RelayMode
{
    public class ShieldItem : BonusItem
    {
        protected override void OnTriggerWithPlayer()
        {
            gameObject.SetActive(false);
            StopIdleAnim();
        }
        private void Start()
        {
            PlayIdleAnim();
        }
        private void OnDestroy()
        {
            StopIdleAnim();
        }
    }
}
