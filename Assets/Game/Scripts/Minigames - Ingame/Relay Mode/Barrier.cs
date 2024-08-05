using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay
{
    public class Barrier : Obstacle
    {
        [SerializeField] Animator animator;
        [SerializeField] string callapseAnimName;
        [SerializeField] ParticleSystem smokeFx;

        public void Callapse()
        {
            PlayCallapseAnim();
        }
        #region ANIMATION MEthod
        private void OnAnimCollapseCompleted()
        {
            gameObject.SetActive(false);
        }

        #endregion

        private void PlayCallapseAnim()
        {
            animator.enabled = true;
        //    animator.Play(callapseAnimName, 0, 0);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag(Constant.PLAYER_TAG))
            {
                Holder.PlaySound?.Invoke();
                Callapse();
            }
        }

        public override void Init()
        {
            animator.enabled = false;
            gameObject.SetActive(true);
        }
    }
}
