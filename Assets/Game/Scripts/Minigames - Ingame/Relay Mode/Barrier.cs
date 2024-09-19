using AnhNV.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Gameplay.Base;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.RelayMode
{
    public class Barrier : Obstacle
    {
        [SerializeField] Animator animator;
        [SerializeField] string callapseAnimName;
        [SerializeField] ParticleSystem smokeFx;
        private Collider2D myCollider;
        private bool isPassed;
        private float distance;

        #region ANIMATION MEthod
        private void OnAnimCollapseCompleted()
        {
            gameObject.SetActive(false);
        }
        #endregion

        internal void ResetDefault()
        {
            myCollider.isTrigger = false;
            isPassed = false;
            animator.Rebind();
            animator.Update(0);
            animator.enabled = false;
            gameObject.SetActive(true);
        }

        private void PlayCallapseAnim()
        {
            animator.enabled = true;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag(TAG.PLAYER))
            {
                Holder.PlaySound?.Invoke();
                myCollider.isTrigger = true;
                PlayCallapseAnim();
            }
        }
        private void Start()
        {
            animator.enabled = false;
            gameObject.SetActive(true);
            myCollider = GetComponentInChildren<Collider2D>();
        }

        private void OnPlayerIsMoving(Base.Player obj)
        {
            if(!isPassed)
            {
                var player = obj as Player;
                isPassed = player.IsRightMoving ? (player.transform.position.x > transform.position.x) : (player.transform.position.x < transform.position.x);
                distance = Vector2.Distance(transform.position, player.transform.position);

                EventManager.OnBarrierCompareDistanceWithPlayer?.Invoke(this, distance);
            }
        }

        private void OnEnable()
        {
            EventManager.OnPlayerIsMoving += OnPlayerIsMoving;
        }

        private void OnDisable()
        {
            EventManager.OnPlayerIsMoving -= OnPlayerIsMoving;
        }
    }
}
