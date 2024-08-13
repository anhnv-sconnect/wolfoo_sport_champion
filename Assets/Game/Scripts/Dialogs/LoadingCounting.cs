using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport
{
    public class LoadingCounting : LoadingPanel
    {
        [SerializeField] GameObject[] countingObjs;
        [SerializeField] CharacterUIAnimation kat;
        [SerializeField] Transform[] characterMoves;

        private Sequence _sequence;
        private int count;
        private bool isInited;

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
        }

        public override void Show()
        {
        }
        private void Init()
        {
            if (isInited) return;
            isInited = true;

            kat.transform.position = characterMoves[0].position;
            foreach (var item in countingObjs)
            {
                item.SetActive(false);
                item.transform.localScale = Vector3.one * 1.5f;
            }
        }

        //private IEnumerator DelayCounting()
        //{

        //    gameObject.SetActive(true);
        //    _sequence = DOTween.Sequence();
        //    _sequence.Append(kat.transform.DOMoveX(characterMoves[1].position.x, 1).OnStart(() =>
        //    {
        //        kat.PlayRunAnim();
        //    })).OnComplete(() =>
        //    {
        //        for (int i = 0; i < countingObjs.Length; i++)
        //        {
        //            int index = i;
        //            countingObjs[index].SetActive(i == count);
        //            count++;
        //            _sequence.Append(countingObjs[index].transform.DOScale(1, 1).OnStart(() =>
        //            {
        //                kat.PlayIdleAnim();
        //                kat.PlayWaveHandAnim();
        //                Debug.Log("1");
        //            }));
        //        }
        //    });
        //}

        public void ShowToHide()
        {
            Init();

            gameObject.SetActive(true);
            _sequence = DOTween.Sequence()
                .Append(kat.transform.DOMoveX(characterMoves[1].position.x, 1).OnStart(() =>
                {
                    kat.PlayRunAnim();
                }))
                .Append(countingObjs[0].transform.DOScale(1, 1).OnStart(() =>
                {
                    kat.PlayIdleAnim();
                    kat.PlayWaveHandAnim();
                    countingObjs[count].SetActive(true);
                }))
                .AppendCallback(() => count++)
                .Append(countingObjs[1].transform.DOScale(1, 1).OnStart(() =>
                {
                    kat.PlayIdleAnim();
                    kat.PlayWaveHandAnim();
                    countingObjs[count].SetActive(true);
                    countingObjs[count - 1].SetActive(false);
                }))
                .AppendCallback(() => count++)
                .Append(countingObjs[2].transform.DOScale(1, 1).OnStart(() =>
                {
                    kat.PlayIdleAnim();
                    kat.PlayWaveHandAnim();
                    countingObjs[count].SetActive(true);
                    countingObjs[count - 1].SetActive(false);
                }))
                .AppendCallback(() => count++)
                .Append(countingObjs[3].transform.DOScale(1, 1).OnStart(() =>
                {
                    kat.PlayIdleAnim();
                    kat.PlayWaveHandAnim();
                    countingObjs[count].SetActive(true);
                    countingObjs[count - 1].SetActive(false);
                }))
                .AppendCallback(() =>
                {
                    OnHide?.Invoke();
                    gameObject.SetActive(false);
                });
        }
    }
}
