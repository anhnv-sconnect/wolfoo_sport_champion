using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace WFSport.Gameplay
{
    public class TrafficCone : Obstacle
    {
        [SerializeField] Animator animator;
        [SerializeField] string collsionAnimName;
        [SerializeField] ParticleSystem smokeFx;

#if UNITY_EDITOR
        [OnValueChanged("OnRegisterCreatingEventChanged")]
        [SerializeField] bool isCreating;

        private SpriteRenderer mySpriteRender;
        private void OnRegisterCreatingEventChanged()
        {
            if(isCreating)
            {
                mySpriteRender = GetComponent<SpriteRenderer>();
            }
        }

        private void OnMouseDown()
        {
            if(isCreating)
            {

            }
        }
        private void OnMouseDrag()
        {
            if (isCreating)
            {

            }
        }
        private void OnMouseUp()
        {
            if (isCreating)
            {

            }
        }
#endif

        public void OnCollison()
        {
          //  PlayCollisonAnim();
        }
        #region ANIMATION MEthod
        private void OnAnimCollisionCompleted()
        {
         //   gameObject.SetActive(false);
        }

        #endregion

        private void PlayCollisonAnim()
        {
            Holder.PlayAnim?.Invoke();
            animator.Play(collsionAnimName, 0, 0);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag(Constant.PLAYER_TAG))
            {
                Holder.PlaySound?.Invoke();
                OnCollison();
            }
        }

        public override void Init()
        {
            gameObject.SetActive(true);
        }
    }
}
