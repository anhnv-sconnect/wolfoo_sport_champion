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
        [SerializeField] private Image fillBar;
        [SerializeField] private TMP_Text timeTxt;
        [SerializeField] private Button backBtn;
        [SerializeField] private Button backBtn2;
        [SerializeField] private Image[] starImgs;
        
        private int totalTime;
        private int totalStarClaimed;
        private TweenerCore<float, float, FloatOptions> _tweenLoadingBar;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tweenStar;

        private void Start()
        {
            backBtn.onClick.AddListener(OnClickBackBtn);
            backBtn2.onClick.AddListener(OnClickBackBtn2);
            EventManager.OnInitGame += InitScreen;
        }

        private void OnDestroy()
        {
            _tweenLoadingBar?.Kill();
            EventManager.OnInitGame -= InitScreen;
        }

        private void OnClickBackBtn()
        {
            Holder.OpenDialog?.Invoke("PauseDialog");
            EventDispatcher.Instance.Dispatch(new EventKeyBase.OpenDialog { dialog = PopupManager.DialogName.Pause});
        }
        private void OnClickBackBtn2()
        {
            EventDispatcher.Instance.Dispatch(new EventKeyBase.OnClosingDialog { dialog = PopupManager.DialogName.Pause });
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

        internal void Setup(int time)
        {
            /// Init
            fillBar.fillAmount = 0;
            foreach (var star in starImgs)
            {
                star.transform.localScale = Vector3.zero;
            }


            /// Anim Setup Timing
            totalTime = time;
            Holder.PlayAnim?.Invoke();
        }

        private void InitScreen()
        {
            fillBar.fillAmount = 0;
        }

        internal void UpdateLoadingBar(float value)
        {
            _tweenLoadingBar?.Kill();
            _tweenLoadingBar = fillBar.DOFillAmount(value, 0.5f).OnComplete(() =>
            {
                if(fillBar.fillAmount >= (totalStarClaimed + 1f) / (starImgs.Length + 0))
                {
                    // Continue Here
                    totalStarClaimed++;
                    if(totalStarClaimed <= starImgs.Length)
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
            });
        }
    }
}
