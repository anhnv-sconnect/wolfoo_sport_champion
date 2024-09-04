using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using SCN;
using AnhNV.GameBase;

namespace WFSport.Gameplay
{
    public class MinigameUI : MonoBehaviour
    {
        [SerializeField] protected Image fillBar;
        [SerializeField] private TMP_Text timeTxt;
        [SerializeField] private Button backBtn;
        [SerializeField] private Image[] starImgs;
        [SerializeField] private LoadingBlur loadingPanelPb;
        [SerializeField] private LoadingCounting countingPanelPb;
        
        private int totalTime;
        private int totalStarClaimed;
        private LoadingBlur loadingPanel;
        private LoadingCounting countingPanel;
        private TweenerCore<float, float, FloatOptions> _tweenLoadingBar;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tweenStar;

        private float[] timeline;

        protected virtual void Start()
        {
            backBtn.onClick.AddListener(OnClickBackBtn);
            EventManager.OnInitGame += InitScreen;
        }

        protected virtual void OnDestroy()
        {
            _tweenLoadingBar?.Kill();
            _tweenStar?.Kill();
            EventManager.OnInitGame -= InitScreen;
        }

        private void OnClickBackBtn()
        {
            Holder.PlaySound?.Invoke();
            Holder.OpenDialog?.Invoke("PauseDialog");
            EventDispatcher.Instance.Dispatch(new EventKeyBase.OpenDialog { dialog = PopupManager.DialogName.Pause});
        }

        private IEnumerator CountTime()
        {
            while (totalTime >= 0)
            {
                var minutes = totalTime / 60;
                var seconds = totalTime % 60;

                var minutesStr = minutes < 10 ? "0" + minutes : minutes.ToString();
                var secondsStr = seconds < 10 ? "0" + seconds : seconds.ToString();

                timeTxt.text = $"{minutesStr} : {secondsStr}";

                totalTime--;
                yield return new WaitForSeconds(1);
            }

            EventManager.OnTimeOut?.Invoke();
        }

        internal void PlayTime()
        {
            StartCoroutine("CountTime");
        }
        internal void PauseTime()
        {
            StopCoroutine("CountTime");
        }

        internal virtual void Setup(int time, float[] timeline)
        {
            // Init playtime
            fillBar.fillAmount = 0;
            foreach (var star in starImgs)
            {
                star.transform.localScale = Vector3.zero;
            }

            // Init timeline
            float total = timeline[timeline.Length - 1];
            this.timeline = new float[timeline.Length];
            for (int i = 0; i < timeline.Length; i++)
            {
                this.timeline[i] = timeline[i] / total;
            }

            /// Anim Setup Timing
            totalTime = time;
            Holder.PlayAnim?.Invoke();
        }
        internal void OpenCountingToStart(System.Action OnCompleted)
        {
            if (countingPanel == null) countingPanel = Instantiate(countingPanelPb, transform);
            countingPanel.ShowToHide();
            countingPanel.OnHide = OnCompleted;
        }

        internal void OpenLoading(System.Action OnCompleted, System.Action OnShowing )
        {
            if (loadingPanel == null) loadingPanel = Instantiate(loadingPanelPb, transform);
            loadingPanel.ShowToHide(1);
            loadingPanel.OnHide = OnCompleted;
            loadingPanel.OnShow = OnShowing;
        }

        private void InitScreen()
        {
            fillBar.fillAmount = 0;
        }

        protected void CheckingStar(float value)
        {
            if (totalStarClaimed >= timeline.Length) return;
            
            if (value >= timeline[totalStarClaimed])
            {
                totalStarClaimed++;
                if (totalStarClaimed <= starImgs.Length)
                {
                    _tweenStar?.Complete();
                    _tweenStar = starImgs[totalStarClaimed - 1].transform.DOScale(Vector3.one, 0.5f)
                    .SetEase(Ease.OutBounce)
                    .OnComplete(() =>
                    {
                        Holder.PlaySound?.Invoke();
                    });
                }
            }
        }

        internal virtual void UpdateLoadingBar(float value)
        {
            if (value > 1) return;

            _tweenLoadingBar?.Kill();
            _tweenLoadingBar = fillBar.DOFillAmount(value, 0.5f).OnComplete(() =>
            {
                CheckingStar(fillBar.fillAmount);
            });
        }
    }
}
