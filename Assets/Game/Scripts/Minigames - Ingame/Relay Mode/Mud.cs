using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.RelayMode
{
    public class Mud : Obstacle
    {
        public override void Init()
        {

        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Constant.PLAYER_TAG))
            {
                Holder.PlaySound?.Invoke();
            }
        }
    }
}
