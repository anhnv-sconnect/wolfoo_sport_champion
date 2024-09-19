using AnhNV.GameBase;
using AnhNV.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport
{
    public class SnowmanStage : MonoBehaviour
    {
        [SerializeField] SpriteRenderer[] snowballs;
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
        private GameObject[] redCircles;

        public Action OnBuildComplete { get; private set; }
        public Transform GetNextSnowballEmpty
        {
            get
            {
                var idx = 0;
                if (snowballCounting < redCircles.Length) idx = snowballCounting;
                return redCircles[idx].transform;
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
            redCircles = new GameObject[snowballs.Length];
            for (int i = 0; i < snowballs.Length; i++)
            {
                snowballs[i].enabled = false;
                redCircles[i] = snowballs[i].transform.GetChild(0).gameObject;
            }
            for (int i = 0;  i < snowmanIdleRenders.Length;  i++)
            {
                snowmanIdleRenders[i].enabled = false;
            }
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
            snowballs[snowballCounting].enabled = true;

            snowballCounting++;

            if ((snowballCounting % 2) == 0)
            {
                SetupSnowmanData();
                StartCoroutine("PlayRepresentSnowman");
            }
            else
            {
                OnBuildComplete?.Invoke();
            }
        }

        private void OnHide()
        {
            animator.enabled = false;
            animator.gameObject.SetActive(false);

            var snowmanIdle = snowmanIdleRenders[(snowballCounting / 2) - 1];
            snowmanIdle.enabled = true;
            for (int i = 0; i < snowmanIdle.transform.childCount; i++)
            {
                snowmanIdle.transform.GetChild(i).gameObject.SetActive(false);
            }

            OnBuildComplete?.Invoke();
        }

        internal void SetupSnowmanData()
        {
            var sprite = snowmanData[idxSnowman];
            snowmanImg.sprite = sprite;

            var idx = (snowballCounting / 2) - 1;
            snowmanIdleRenders[idx].sprite = sprite;

            idxSnowman++;
            if (idxSnowman == snowmanData.Length) idxSnowman = 0;
        }
        internal IEnumerator PlayRepresentSnowman()
        {
            yield return new WaitForSeconds(1);

            animator.enabled = true;
            animator.gameObject.SetActive(true);
            animator.Play(openAnimName, 0, 0);
            Holder.PlaySound?.Invoke();
        }
        internal void StopRepresent()
        {
            Holder.PlaySound?.Invoke();
            animator.Play(closeAnimName, 0, 0);
        }
    }
}
