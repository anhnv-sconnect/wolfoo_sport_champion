using UnityEngine;

namespace WFSport.Gameplay.FurnitureMode
{
    public class ToyScrollItem : ScrollItem
    {
        private Vector4 myLimit;

        public void Setup(Vector4 limitPos)
        {
            myLimit = limitPos;
        }
        protected override void OnEndDrag()
        {
            if(transform.position.x > myLimit.x && transform.position.x < myLimit.z
                && transform.position.y < myLimit.y && transform.position.y > myLimit.w)
            {
                OnDragInSide?.Invoke(this);
            }
        }
    }
}
