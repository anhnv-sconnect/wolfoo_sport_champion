using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AnhNV.GameBase
{
    public class TutorialWithBG : TutorialStep
    {
        [SerializeField] Transform focusPanel;
        [SerializeField] Transform pointer;
        [SerializeField] Animator animator;

        private string tutorialID;

        public override string TutorialID { get => tutorialID; set => tutorialID = value; }

        private void Start()
        {
        }

        public override void Play()
        {
            IsPlaying = true;
            animator.Play("Play", 0, 0);
        }

        public override void Stop()
        {
            if (IsPlaying)
                animator.Play("Stop", 0, 0);
            IsPlaying = false;
        }
        public void OnAnimComplete()
        {
        }

        public void Setup(Transform highlightTarget)
        {
            gameObject.SetActive(false);
            SetPointer(highlightTarget.transform.position);
        }
        private void SetPointer(Vector3 endPos)
        {
            pointer.position = endPos;

            if (pointer.position.x < 0 && pointer.position.y < 0)
            {
                pointer.rotation = Quaternion.Euler(Vector3.forward * 120f);
            }
            else if (pointer.position.x > 0 && pointer.position.y < 0)
            {
                pointer.rotation = Quaternion.Euler(Vector3.forward * -160f);
            }
            else if (pointer.position.x > 0 && pointer.position.y > 0)
            {
                pointer.rotation = Quaternion.Euler(Vector3.forward * -40);
            }
            else
            {
                pointer.rotation = Quaternion.Euler(Vector3.zero);
            }
        }

        public override void Release()
        {
            Destroy(gameObject);
        }
    }
}
