using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhNV.GameBase
{
    public class TutorialDragInRange : TutorialStep
    {
        [SerializeField] Animator animator;
        [SerializeField] string playName;
        [SerializeField] string stopName;
        private string id;
        private Tween dotween;

        public override string TutorialID { get => id; set => id = value; }

        private void OnDestroy()
        {
            dotween?.Kill();   
        }
        public override void Play()
        {
            animator.enabled = true;
            animator.Play(playName, 0, 0);
        }

        public override void Release()
        {
        }

        public override void Stop()
        {
            animator.Play(stopName, 0, 0);
            dotween?.Kill();
            dotween = DOVirtual.DelayedCall(0.5f, () =>
            {
                gameObject.SetActive(false);
            });
        }
        public void Setup(Transform target)
        {
            animator.enabled = false;
        }
    }
}
