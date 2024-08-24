using AnhNV.GameBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport
{
    public class SnowmanStage : MonoBehaviour
    {
        [SerializeField] GameObject[] redCircles;
        [SerializeField] SpriteRenderer[] snowmanIdleRenders;
        [SerializeField] Sprite[] snowmanData;
        [SerializeField] Animator animator;
        [SerializeField] string openAnimName;
        [SerializeField] string closeAnimName;
        [SerializeField] AnimatorHelper animatorHelper;
        [SerializeField] Button backBtn;
        [SerializeField] Image snowmanImg;
        private int snowballCounting;
        private int idxSnowman;

        public Action OnBuildComplete { get; private set; }
        public Vector3 GetNextSnowballPos
        {
            get
            {
                var idx = 0;
                if (snowballCounting < redCircles.Length) idx = snowballCounting;
                return redCircles[idx].transform.position;
            }
        }

        private void Start()
        {
            animatorHelper.OnCloseComplete += OnHide;
            backBtn.onClick.AddListener(StopRepresent);
            Init();
        }
        private void OnDestroy()
        {
            animatorHelper.OnCloseComplete -= OnHide;
        }

        private void Init()
        {
            idxSnowman = UnityEngine.Random.Range(0, snowmanData.Length);
        }

        internal void BuildNextSnowball(System.Action OnCompleted)
        {
            OnBuildComplete = OnCompleted;

            if(snowballCounting >= redCircles.Length)
            {
                OnBuildComplete?.Invoke();
                return;
            }
            redCircles[snowballCounting].SetActive(false);
            snowballCounting++;

            if ((snowballCounting % 2) == 0)
            {
                SetupSnowmanData();
                PlayRepresentSnowman();
            }
        }

        private void OnHide()
        {
            animator.enabled = false;
            animator.gameObject.SetActive(false);

            OnBuildComplete?.Invoke();
        }

        internal void SetupSnowmanData()
        {
            var sprite = snowmanData[idxSnowman];
            snowmanImg.sprite = sprite;

            var idx = snowballCounting - 1;
            snowmanIdleRenders[idx].sprite = sprite;

            idxSnowman++;
            if (idxSnowman == snowmanData.Length) idxSnowman = 0;
        }
        internal void PlayRepresentSnowman()
        {
            animator.Play(openAnimName, 0, 0);
            animator.enabled = true;
            animator.gameObject.SetActive(true);
        }
        internal void StopRepresent()
        {
            animator.Play(closeAnimName, 0, 0);
        }
    }
}
