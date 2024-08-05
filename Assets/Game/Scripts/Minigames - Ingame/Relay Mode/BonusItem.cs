using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay
{
    public abstract class BonusItem : MonoBehaviour
    {
        protected abstract void OnTriggerWithPlayer();

        [Dropdown("GetRoadValues")]
        [OnValueChanged("OnChangeLine")]
        public float line;

        private DropdownList<float> GetRoadValues()
        {
            return new DropdownList<float>()
            {
                { "Line1", Constant.LINE1 },
                { "Line2", Constant.LINE2 },
                { "Line3", Constant.LINE3 }
            };
        }
        void OnChangeLine()
        {
            transform.position = new Vector3(transform.position.x, line, transform.position.z);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Constant.PLAYER_TAG))
            {
                OnTriggerWithPlayer();
            }
        }
    }
}
