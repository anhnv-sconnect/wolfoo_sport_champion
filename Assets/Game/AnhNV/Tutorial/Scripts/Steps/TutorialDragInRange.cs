using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhNV.GameBase
{
    public class TutorialDragInRange : TutorialStep
    {
        [SerializeField] private Transform pointer;
        [SerializeField] private Transform beginTarget;
        [SerializeField] private Transform endTarget;
        [SerializeField] private float speed;

        private string id;
        private bool isMoving;
        private int count;
        private bool canMove;

        public override string TutorialID { get => id; set => id = value; }

        private void Start()
        {
            Play();
        }
        private void OnDestroy()
        {
            Stop();
        }

        public void Setup(Transform begin, Transform end, float speed = 1)
        {
            beginTarget = begin;
            endTarget = end;
            this.speed = speed;
        }

        public override void Play()
        {
            canMove = true;
            StartCoroutine("Moving");
        }

        private IEnumerator Moving()
        {
            isMoving = true;
            pointer.position = beginTarget.position;
            count = 0;

            while (isMoving && canMove)
            {
                yield return null;
                pointer.gameObject.SetActive(true);
                pointer.position = Vector3.Lerp(beginTarget.position, endTarget.position, count * speed * 0.01f);
                count += 1;

                if (Vector2.Distance(pointer.position, endTarget.position) <= 0.01f)
                {
                    count = 0;
                    pointer.position = beginTarget.position;
                    pointer.gameObject.SetActive(false);
                }
            }

            isMoving = false;
        }

        public override void Release()
        {
        }

        public override void Stop()
        {
            canMove = false;
            StopCoroutine("Moving");
            pointer.gameObject.SetActive(false);
        }
    }
}
