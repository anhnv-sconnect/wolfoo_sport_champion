using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport
{
    public abstract class Obstacle : MonoBehaviour
    {
        public abstract void Init();

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
    }
}
