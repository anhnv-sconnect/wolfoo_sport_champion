using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhNV.GameBase
{
    public class TutorialClick : TutorialStep
    {
        [SerializeField] Transform focusPanel;
        [SerializeField] Transform pointer;
        [SerializeField] Animator animator;
        [SerializeField] Canvas canvas;

        private string id;

        public override string TutorialID { get => id; set => id = value; }

        public override void Play()
        {
            IsPlaying = true;
            gameObject.SetActive(true);
            animator.Play("Play", 0, 0);
        }

        public override void Stop()
        {
            if (IsPlaying)
                animator.Play("Stop", 0, 0);
            IsPlaying = false;
        }

        public void Setup(Transform target)
        {
            canvas.worldCamera = Camera.main;
            gameObject.SetActive(false);
            animator.transform.position = target.position;
         //   SetPointer(highlightTarget.transform.position);
        }
        public void Setup(Vector3 target)
        {
            canvas.worldCamera = Camera.main;
            gameObject.SetActive(false);
            animator.transform.position = target;
         //   SetPointer(highlightTarget.transform.position);
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
