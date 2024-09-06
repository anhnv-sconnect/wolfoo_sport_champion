using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    public class Basket : MonoBehaviour
    {
        [SerializeField] Transform hole;

        private float distanceVerified;
        public Vector3 HolePos { get => hole.position; }

        private void Start()
        {
            EventManager.OnThrow += OnPlayerThrow;
        }
        private void OnDestroy()
        {
            EventManager.OnThrow -= OnPlayerThrow;
        }

        internal void Setup(float distance)
        {
            distanceVerified = distance;
        }

        private void OnPlayerThrow(Player player, Vector3 pos)
        {
            if (Vector2.Distance(pos, HolePos) <= distanceVerified)
            {
                EventManager.OnBallTracking?.Invoke(player, this);
            }
        }
    }
}
