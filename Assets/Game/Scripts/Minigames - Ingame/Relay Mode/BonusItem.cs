using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Gameplay.RelayMode;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay
{
    public abstract class BonusItem : MonoBehaviour
    {
        protected abstract void OnTriggerWithPlayer();

        [Dropdown("GetRoadValues")]
        [OnValueChanged("OnChangeLine")]
        public float line;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tweenIdling;

        private DropdownList<float> GetRoadValues()
        {
            return new DropdownList<float>()
            {
                { "Line1", Constant.LINE1 },
                { "Line2", Constant.LINE2 },
                { "Line3", Constant.LINE3 }
            };
        }
        //[NaughtyAttributes.Button]
        //private void Line1() { line = Constant.LINE1; OnChangeLine(); }
        //[NaughtyAttributes.Button]
        //private void Line2() { line = Constant.LINE2; OnChangeLine(); }
        //[NaughtyAttributes.Button]
        //private void Line3() { line = Constant.LINE3; OnChangeLine(); }

        void OnChangeLine()
        {
            transform.position = new Vector3(transform.position.x, line, transform.position.z);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(TAG.PLAYER))
            {
                OnTriggerWithPlayer();
            }
        }
        protected void PlayIdleAnim()
        {
            _tweenIdling?.Kill();
            _tweenIdling = transform.DOMove(transform.position + Vector3.up * 0.5f, 1)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
        }
        protected void StopIdleAnim()
        {
            _tweenIdling?.Kill();
        }
    }
}
