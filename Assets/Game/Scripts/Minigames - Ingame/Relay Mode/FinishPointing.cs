using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport
{
    public class FinishPointing : MonoBehaviour
    {
        public bool IsFinished { get; private set; }
        public bool IsFinal { get; private set; }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag(TAG.PLAYER))
            {
                IsFinished = true;
            }  
        }

        internal void SetNormal()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        internal void SetFinal()
        {
            IsFinal = true;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
