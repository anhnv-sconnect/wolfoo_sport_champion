using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.SnowballMode
{
    public class Snow : MonoBehaviour
    {
        private Vector2 range;
        private ScratchCardManager myCard;

        private void Start()
        {
            myCard = GetComponent<ScratchCardManager>();
        }
        private void OnDrawGizmos()
        {
            var player = FindAnyObjectByType<Player>();
            if (player == null) return;
            var size = player.GetComponent<BoxCollider2D>().size;
            var direction = player.transform.position - transform.position;
            var angle = Mathf.Atan(direction.x/ direction.y) * Mathf.Rad2Deg;
            var length = (size.y / 2) / (Mathf.Sin(angle));
            var playerRange = direction.normalized * length;

            Debug.Log(angle);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerRange);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag(TAG.PLAYER))
            {
                var direction = collision.bounds.center - transform.position;
                var angle = Vector2.Dot(collision.bounds.center, transform.position) * Mathf.PI;

                //    var range = direction.normalized * (direction.magnitude - Vector2.Distance());
                var range = Vector3.zero;
                Debug.Log(angle);
                var pos = Camera.main.WorldToScreenPoint(collision.transform.position - range);
                myCard.Card.ScratchHole(pos);
            }
        }
        internal void EnableScratch()
        {
            myCard.Card.InputEnabled = true;
        }
        internal void DisableScratch()
        {
            myCard.Card.InputEnabled = false;
        }
    }
}
