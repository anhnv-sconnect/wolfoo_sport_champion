using AnhNV.Helper;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WFSport;

namespace AnhNV.Dialog
{
    public class LoadingCounting : LoadingPanel
    {
        [SerializeField] GameObject[] countingObjs;
        [SerializeField] CharacterUIAnimation kat;
        [SerializeField] Transform[] characterMoves;
        [SerializeField] Image clockImg;
        [SerializeField] TMP_Text timeTxt;

        private Sequence _sequence;
        private int count;
        private bool isInited;
        private Vector3 clockTarget;

        private void Start()
        {
            Init();
        }
        private void OnDestroy()
        {
            _sequence?.Kill();
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }
        private void Init()
        {
            if (isInited) return;
            isInited = true;

            count = 0;
            kat.transform.position = characterMoves[0].position;
            foreach (var item in countingObjs)
            {
                item.SetActive(false);
                item.transform.localScale = Vector3.one * 1.5f;
            }
        }
        public void Setup(Vector3 clockArea, string timeString)
        {
            clockTarget = clockArea;
            clockImg.DOFade(0, 0);
            clockImg.gameObject.SetActive(false);

            timeTxt.text = timeString;
        }

        public void ShowToHide()
        {
            Init();
            Show();

            _sequence = DOTween.Sequence()
                .Append(kat.transform.DOMoveX(characterMoves[1].position.x, 1).OnStart(() =>
                {
                    kat.PlayRunAnim();
                }))
                .Append(countingObjs[0].transform.DOScale(1, 1).OnStart(() =>
                {
                    kat.PlayIdleAnim();
                    kat.PlayPointingAnim();
                    countingObjs[count].SetActive(true);
                    Holder.PlaySound?.Invoke();
                }))
                .AppendCallback(() => count++)
                .Append(countingObjs[1].transform.DOScale(1, 1).OnStart(() =>
                {
                    kat.PlayIdleAnim();
                    kat.PlayPointingAnim();
                    countingObjs[count].SetActive(true);
                    countingObjs[count - 1].SetActive(false);
                    Holder.PlaySound?.Invoke();
                }))
                .AppendCallback(() => count++)
                .Append(countingObjs[2].transform.DOScale(1, 1).OnStart(() =>
                {
                    kat.PlayIdleAnim();
                    kat.PlayPointingAnim();
                    countingObjs[count].SetActive(true);
                    countingObjs[count - 1].SetActive(false);
                    Holder.PlaySound?.Invoke();
                }))
                .AppendCallback(() => count++)
                .Append(countingObjs[3].transform.DOScale(1, 1).OnStart(() =>
                {
                    kat.PlayIdleAnim();
                    kat.PlayPointingAnim();
                    countingObjs[count].SetActive(true);
                    countingObjs[count - 1].SetActive(false);
                    Holder.PlaySound?.Invoke();

                }))
                .AppendCallback(() =>
                {
                    countingObjs[count].SetActive(false);
                    clockImg.gameObject.SetActive(true);
                })
                .Append(clockImg.DOFade(1, 0.25f))
                .Join(clockImg.transform.DOMoveY(1.5f, 0.5f))
                .Append(clockImg.transform.DORotate(Vector3.forward * 10, 0.25f))
                .Append(clockImg.transform.DORotate(Vector3.forward * -10, 0.5f))
                .Join(clockImg.transform.DOPunchScale(Vector3.one * 0.1f, 0.25f, 3))
                .Append(clockImg.transform.DOJump(clockTarget, 2, 1, 0.5f));

            _sequence.OnComplete(() =>
            {
                OnHided?.Invoke();
                Hide();
            });
        }
    }
}
