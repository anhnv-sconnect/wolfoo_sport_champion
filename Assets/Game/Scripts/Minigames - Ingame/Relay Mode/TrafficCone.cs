using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.RelayMode
{
    public class TrafficCone : Obstacle
    {
        [SerializeField] Animator animator;
        [SerializeField] string collsionAnimName;
        [SerializeField] ParticleSystem smokeFx;

        private Vector3 lastPos;
        private TrafficCone lastCone;
        private SpriteRenderer myLayer;

        [SerializeField] bool isLine1;
        public bool IsLine1 { get => isLine1; }

#if UNITY_EDITOR
        [NaughtyAttributes.OnValueChanged("OnRegisterCreatingEventChanged")]
        [SerializeField] bool isCreating;
        [NaughtyAttributes.ShowIf("isCreating")]
        [SerializeField] float createRange;

        private void Assign(TrafficCone newCone)
        {
            newCone.name = "Obstacle - Traffic Cone " + newCone.GetInstanceID();
            newCone.isCreating = false;
            newCone.SetLayerTopDown();
        }

        private void SetLayerTopDown()
        {
            if (myLayer == null) myLayer = GetComponent<SpriteRenderer>();
            myLayer.sortingOrder = (int)(transform.position.y * -100);
        }

        private void OnRegisterCreatingEventChanged()
        {
            if(isCreating)
            {
                lastCone = this;
            }
        }
        public void OnCreating()
        {
            if (createRange == 0) createRange = 2;
            if (isCreating)
            {
                Debug.Log("IsCreating...");
                if (lastPos == Vector3.zero) lastPos = transform.position;

                if (Vector2.Distance(transform.position, lastPos) >= createRange)
                {
                    if (lastCone == null) lastCone = this;
                    var cone = Instantiate(this, transform.parent);

                    Assign(cone);

                    lastPos = transform.position;
                    lastCone = cone;
                }
            }
        }
#endif

        private void Start()
        {
            name = "Obstacle - Traffic Cone " + GetInstanceID();
            EventManager.OnTriggleWithCone += OnTracking;
        }
        private void OnDestroy()
        {
            EventManager.OnTriggleWithCone -= OnTracking;
        }

        private void OnTracking(TrafficCone cone)
        {
            // Find A Neighbor Accross
            EventManager.OnTracking?.Invoke(this);
        }

        public void OnCollison()
        {
            Holder.PlaySound?.Invoke();
            Holder.PlayAnim?.Invoke();
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
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(TAG.PLAYER))
            {
                OnCollison();
            }
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
        }

        public override void Init()
        {
            gameObject.SetActive(true);
        }
    }
}
