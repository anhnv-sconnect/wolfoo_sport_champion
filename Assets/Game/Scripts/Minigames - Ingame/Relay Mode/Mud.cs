using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TAG = WFSport.Base.Constant.TAG;

namespace WFSport.Gameplay.RelayMode
{
    public class Mud : Obstacle
    {
        public override void Init()
        {

        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(TAG.PLAYER))
            {
                Holder.PlaySound?.Invoke();
            }
        }
    }
}
