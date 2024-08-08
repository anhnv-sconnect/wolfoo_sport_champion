using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Gameplay.RelayMode;

namespace WFSport
{
    public abstract class Obstacle : MonoBehaviour
    {
        [Dropdown("GetRoadValues")]
        [OnValueChanged("OnChangeLine")]
        [SerializeField] public float line;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tweenIdling;

        protected virtual DropdownList<float> GetRoadValues()
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
