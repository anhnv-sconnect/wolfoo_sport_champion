using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.MagnetCoin
{
    public class ManagerDemo : MonoBehaviour
    {
        [SerializeField] Transform trans;
        [SerializeField] MagnetCoinControl magnetCoin;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                magnetCoin.Play();
            }
        }
    }
}