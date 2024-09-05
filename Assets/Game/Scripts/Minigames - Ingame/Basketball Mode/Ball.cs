using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] float flyTime;
        private Vector3 targetPos;
        private bool isFlying;

        private void Update()
        {
            if(isFlying)
            {
                var pos = Vector2.Lerp(transform.position, targetPos, );
            }
        }
        internal void FlyTo(Vector3 endPos)
        {
            targetPos = endPos;
            isFlying = true;

        }
    }
}
