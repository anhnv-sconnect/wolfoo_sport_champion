using NaughtyAttributes;
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
        public float Lane { get => line; }

        [SerializeField] bool isCreating;
        [NaughtyAttributes.ShowIf("isCreating")]
        [SerializeField] float createRange;

        protected override DropdownList<float> GetRoadValues()
        {
            base.GetRoadValues();
            return new DropdownList<float>()
            {
                { "Line1", Constant.CONE_LINE1 },
                { "Line2", Constant.CONE_LINE2 },
                { "Line3", Constant.CONE_LINE3 }
            };
        }

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

        private void Start()
        {
            name = "Obstacle - Traffic Cone " + GetInstanceID();
            EventManager.OnTriggleWithCone += OnTracking;
            SetLayerTopDown();
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
    }
}
