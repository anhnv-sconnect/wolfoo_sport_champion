using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport.Base
{
    public class Energy : MonoBehaviour
    {
        private Sequence anim;

        private void OnDestroy()
        {
            anim?.Kill();
        }
        public void Show(System.Action OnComplete = null, bool isImmediate = false)
        {
            anim?.Complete();
            anim = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.zero, 0))
                .Append(transform.DOScale(Vector3.one, isImmediate ? 0 : 0.5f).SetEase(Ease.OutBack));
            anim.OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        }
        public void Hide(System.Action OnComplete = null, bool isImmediate = false)
        {
            anim?.Complete();
            anim = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one, 0))
                .Append(transform.DOScale(Vector3.zero, isImmediate ? 0 : 0.5f).SetEase(Ease.OutBack));
            anim.OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        }
    }
}
