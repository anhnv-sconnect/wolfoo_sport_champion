using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay
{
    public class StarItem : BonusItem
    {
        protected override void OnTriggerWithPlayer()
        {
            gameObject.SetActive(false);
        }
    }
}
