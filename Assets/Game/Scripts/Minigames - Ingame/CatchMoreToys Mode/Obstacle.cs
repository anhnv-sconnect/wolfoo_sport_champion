using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Base;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    public class Obstacle : Item
    {
        [SerializeField] ParticleSystem smokeFx;
        private Player player;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Constant.TAG.DEATHZONE))
            {
                smokeFx.Stop();
                OnTouchedGround();
            }
            if(collision.CompareTag(Constant.TAG.PLAYER))
            {
                smokeFx.Play();
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag(Constant.TAG.PLAYER))
            {
                if (player == null)
                {
                    player = collision.GetComponent<Player>();
                }
                MoveToCart(player.Cart);
            }
        }

        protected override void Init()
        {
        }
    }
}
