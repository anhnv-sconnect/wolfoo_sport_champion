using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay
{
    public class Barrier : Obstacle
    {
        [SerializeField] Animator animator;
        [SerializeField] string callapseAnimName;
        [SerializeField] ParticleSystem smokeFx;
        private Collider2D myCollider;

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
            if (collision.collider.CompareTag(TAG.PLAYER))
            {
                Holder.PlaySound?.Invoke();
                myCollider.isTrigger = true;
                Callapse();
            }
        }
        private void Start()
        {
            animator.enabled = false;
            gameObject.SetActive(true);
            myCollider = GetComponentInChildren<Collider2D>();
        }
    }
}
