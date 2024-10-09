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
using AnhNV.Dialog;
using AnhNV.Helper;
using WFSport.Base;

namespace WFSport.Gameplay
{
    public class MinigameUI : MonoBehaviour
    {
        [SerializeField] protected Image fillBar;
        [SerializeField] private TMP_Text timeTxt;
        [SerializeField] private Image[] starImgs;
        [SerializeField] private LoadingBlur loadingPanelPb;
        [SerializeField] private LoadingCounting countingPanelPb;
        [SerializeField] private Image timer;
        
        private int totalTime;
        private int totalStarClaimed;
        private LoadingBlur loadingPanel;
        private LoadingCounting countingPanel;
        private TweenerCore<float, float, FloatOptions> _tweenLoadingBar;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tweenStar;

        private float[] loadingSteps;
        private Sequence tweenFadeScreen;
        private Sequence _animLowerTime;
        private int lastScore;

        public int TotalStarClaimed { get => totalStarClaimed; }

        protected virtual void Start()
        {
            EventManager.OnInitGame += InitScreen;
        }

        protected virtual void OnDestroy()
        {
            _tweenLoadingBar?.Kill();
            _tweenStar?.Kill();
            _animLowerTime?.Kill();
            tweenFadeScreen?.Kill();
            EventManager.OnInitGame -= InitScreen;
        }

        private void GetTimeUITemplate()
        {
            var minutes = totalTime / 60;
            var seconds = totalTime % 60;

            var minutesStr = minutes < 10 ? "0" + minutes : minutes.ToString();
            var secondsStr = seconds < 10 ? "0" + seconds : seconds.ToString();

            timeTxt.text = $"{minutesStr} : {secondsStr}";
        }

        private IEnumerator CountTime()
        {
            while (totalTime >= 0)
            {
                GetTimeUITemplate();

                totalTime--;
                yield return new WaitForSeconds(1);

                if(totalTime <= 10)
                {
                    PlayAnimTimerisLow();
                }
            }

            EventManager.OnTimeOut?.Invoke();
        }
        private void PlayAnimTimerisLow()
        {
            _animLowerTime?.Kill();
            timer.color = Color.white;
            timer.transform.localScale = Vector2.one;
            _animLowerTime = DOTween.Sequence()
                .Append(timer.DOColor(Color.red, 0.25f))
                .Join(timer.transform.DOPunchScale(Vector2.one * 0.1f, 0.5f, 2))
                .Append(timer.DOColor(Color.white, 0.25f));

        }

        internal void PlayTime()
        {
            StartCoroutine("CountTime");
        }
        internal void PauseTime()
        {
            StopCoroutine("CountTime");
        }
        protected float[] GetLoadingSteps(float[] timelines)
        {
            var total = timelines.Length;
            var loadingSteps = new float[total];
            var step = 1f / total;

            for (int i = 0; i < total; i++)
            {
                if (i > 0)
                {
                    loadingSteps[i] = step / (timelines[i] - timelines[i - 1]);
                }
                else
                {
                    loadingSteps[i] = step / timelines[0];
                }
            }

            return loadingSteps;
        }

        internal virtual void Setup(int time, float[] timelines)
        {
            // Init playtime
            fillBar.fillAmount = 0;
            foreach (var star in starImgs)
            {
                star.transform.localScale = Vector3.zero;
            }

            loadingSteps = GetLoadingSteps(timelines);

            /// Anim Setup Timing
            totalTime = time;
            GetTimeUITemplate();
            
            Holder.PlayAnim?.Invoke();
        }
        internal void OpenCountingToStart(System.Action OnCompleted)
        {
            if (countingPanel == null) countingPanel = Instantiate(countingPanelPb, transform);
            countingPanel.Setup(timeTxt.transform.position, timeTxt.text);
            countingPanel.ShowToHide();
            countingPanel.OnHided = OnCompleted;
        }

        internal void OpenLoading(System.Action OnCompleted, System.Action OnShowing, float delay = 0)
        {
            tweenFadeScreen = DOTween.Sequence()
                .AppendInterval(delay);
            tweenFadeScreen.OnComplete(() =>
            {
                if (loadingPanel == null) loadingPanel = Instantiate(loadingPanelPb, transform);
                loadingPanel.ShowToHide(1);
                loadingPanel.OnHided = OnCompleted;
                loadingPanel.OnShown = OnShowing;
            });
        }

        private void InitScreen()
        {
            fillBar.fillAmount = 0;
        }

        protected void CheckingStar()
        {
            var step = (1f / starImgs.Length) * (totalStarClaimed + 1);
            var value = fillBar.fillAmount;
            if (value >= step)
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

        internal virtual void UpdateLoadingBar(int score)
        {
            if (fillBar.fillAmount >= 1) return;
            _tweenLoadingBar?.Complete();

            var totalStep = score - lastScore;
            lastScore = score;
            var value = fillBar.fillAmount + loadingSteps[totalStarClaimed] * totalStep;

            _tweenLoadingBar = fillBar.DOFillAmount(value, 0.5f).OnComplete(() =>
            {
                CheckingStar();
            });
        }
    }
}
