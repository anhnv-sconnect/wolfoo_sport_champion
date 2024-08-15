using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Base;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    public class Toy : Item
    {
        [SerializeField] ParticleSystem lightingFx;
        private Player player;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Constant.TAG.DEATHZONE))
            {
                if (lightingFx != null) lightingFx.Play();
                OnTouchedGround();
            }

        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag(Constant.TAG.PLAYER))
            {
                if(player == null)
                {
                    player = collision.GetComponent<Player>();
                }
                MoveToCart(player.Cart);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (lightingFx != null) lightingFx.Stop();
        }

        protected override void Init()
        {
        }
    }
}
