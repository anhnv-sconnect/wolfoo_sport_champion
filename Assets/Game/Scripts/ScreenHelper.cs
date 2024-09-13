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
    }
}
