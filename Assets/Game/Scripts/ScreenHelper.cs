using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Helper
{
    public class ScreenHelper
    {
        public static Vector2 GetMaxPosition()
        {
            var maxXPos = Camera.main.ScreenToWorldPoint(Vector3.right * Camera.main.pixelWidth);
            var maxYPos = Camera.main.ScreenToWorldPoint(Vector3.up * Camera.main.pixelHeight);

            return new Vector2(maxXPos.x, maxYPos.y);
        }
        public static Vector2 GetMaxPizelSize()
        {
            var maxXPos = Camera.main.pixelWidth;
            var maxYPos = Camera.main.pixelHeight;

            return new Vector2(maxXPos, maxYPos);
        }
        public static Vector3 GetMousePos()
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            return pos;
        }
        public static void CheckSize(Action OnNormal, Action OnWide, Action OnLong, Action FuckingLong = null)
        {
            if (Camera.main.aspect >= 2.5f)
            {
                Debug.Log("21:9");
                FuckingLong?.Invoke();
            }
            else if (Camera.main.aspect >= 1.7)
            {
                Debug.Log("16:9");
                OnLong?.Invoke();
            }
            else if (Camera.main.aspect >= 1.5)
            {

                Debug.Log("3:2");
                OnNormal?.Invoke();
            }
            else 
            {
                Debug.Log("4:3");
                OnWide?.Invoke();
            }
        }
    }
}
