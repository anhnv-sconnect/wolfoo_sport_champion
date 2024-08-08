using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport
{
    public class FinishPointing : MonoBehaviour
    {
        public bool IsFinal { get; private set; }

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
