using AnhNV.GameBase;
using AnhNV.Helper;
using DG.Tweening;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport.Base
{
    public class MinigameSystemUI : MonoBehaviour
    {
        [SerializeField] private Button backBtn;
        [SerializeField] private Button settingBtn;
        [SerializeField] private Button energyBtn;
        [SerializeField] private TMPro.TMP_Text coinTxt;
        [SerializeField] private Image[] energyImgs;
        [SerializeField] private GameObject energyHolder;
        [SerializeField] private GameObject coinHolder;
        [SerializeField] private MagnetCoinControl coinControl;
        [SerializeField] private Energy[] energies;

        private bool canClick = true;
        public Action ClickBackBtn;
        private int totalCoin { get => GameController.Instance.PlayerMe.totalCoin; }
        private int totalEnergy { get => GameController.Instance.PlayerMe.totalEnergy; }
        private Sequence animCoin;
        private Sequence animEnergy;
        private Image holderEnergyImg;

        private void Awake()
        {
            var a = GameController.Instance;
        }

        // Start is called before the first frame update
        void Start()
        {
            settingBtn.onClick.AddListener(OnClickSetting);
            backBtn.onClick.AddListener(OnClickBackBtn);
            energyBtn.onClick.AddListener(OnClickEnergyBtn);
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        private void OnDestroy()
        {
            animCoin?.Kill();
            animEnergy?.Kill();
        }

        private void OnClickEnergyBtn()
        {
            StopAnimOutofEnergy();
            EventDispatcher.Instance.Dispatch(new EventKeyBase.ChangeScene { gameplay = true, minigame = Minigame.CreateEnergy });
        }

        internal void Setup()
        {
            for (int i = 0; i < energies.Length; i++)
            {
                if (i < totalEnergy)
                {
                    energies[i].Show(null, true);
                }
                else
                {
                    energies[i].Hide(null, true);
                }
            }
            UpdateCoin();
        }
        private void OnClickSetting()
        {
            EventDispatcher.Instance.Dispatch(new EventKeyBase.OpenDialog { dialog = PopupManager.DialogName.Setting });
        }
        public void PlayAnimOutOfEnergy()
        {
            if (holderEnergyImg == null) holderEnergyImg = energyHolder.GetComponent<Image>();
            StopAnimOutofEnergy();
            animEnergy = DOTween.Sequence()
                .Append(holderEnergyImg.DOColor(Color.red, 0.25f).SetLoops(1, LoopType.Yoyo).SetEase(Ease.Flash))
                .Join(energyHolder.transform.DOPunchScale(Vector3.one * 0.3f, 0.5f, 3))
                .Append(holderEnergyImg.DOColor(Color.white, 0.25f).SetEase(Ease.Flash))
                .AppendInterval(1)
                .SetLoops(-1, LoopType.Restart);
        }
        public void StopAnimOutofEnergy()
        {
            animEnergy?.Kill();
            if (holderEnergyImg == null) holderEnergyImg = energyHolder.GetComponent<Image>();
            holderEnergyImg.color = Color.white;
            energyHolder.transform.localScale = Vector3.one;
        }
        public void PlayAnimOutOfCoin()
        {
            animCoin?.Kill();
            coinTxt.transform.localScale = Vector3.one;
            coinTxt.color = Color.white;

            animCoin = DOTween.Sequence()
                .Append(coinTxt.DOColor(Color.red, 0.25f).SetLoops(1, LoopType.Yoyo).SetEase(Ease.Flash))
                .Join(coinTxt.transform.DOPunchScale(Vector3.one * 1, 0.25f, 3))
                .Append(coinTxt.DOColor(Color.white, 0.25f).SetEase(Ease.Flash))
                .SetLoops(1, LoopType.Restart);
        }
        public void PlayAnimPurchasingCoin(Transform target, System.Action OnComplete)
        {
            var isACtive = coinHolder.activeSelf;
            coinHolder.SetActive(true);
            coinControl.MagnetTrans = target;
            coinControl.Play(coinHolder.transform, null, null, () =>
            {
                coinHolder.SetActive(isACtive);
                OnComplete?.Invoke();
            });
        }
        public void PlayAnimEarningCoin(Vector3 beginPos, System.Action OnComplete, System.Action OnUpdate = null)
        {
            var isACtive = coinHolder.activeSelf;

            coinControl.transform.position = beginPos;
            coinHolder.SetActive(true);
            coinControl.MagnetTrans = coinHolder.transform;
            coinControl.Play(coinHolder.transform, null, () =>
            {
                OnUpdate?.Invoke();
            }, () =>
            {
                coinHolder.SetActive(isACtive);
                OnComplete?.Invoke();
            });
        }
        public void PlayAnimInCreaseEnergy(System.Action OnComplete)
        {
            var isActive = energyHolder.activeSelf;
            var total = totalEnergy;
            if (total <= energies.Length)
            {
                energyHolder.SetActive(true);
                energies[total - 1].Show(() =>
                {
                    energyHolder.SetActive(isActive);
                    OnComplete.Invoke();
                });
            }
        }
        public void PlayAnimDecreaseEnergy(System.Action OnComplete)
        {
            var isActive = energyHolder.activeSelf;
            var total = totalEnergy;
            if (total > 0)
            {
                energyHolder.SetActive(true);
                energies[total - 1].Hide(() =>
                {
                    energyHolder.SetActive(isActive);
                    OnComplete.Invoke();
                });
            }
        }
        public void UpdateCoin(bool usingAnim = false)
        {
            var total = totalCoin;
            string coinStr = total.ToString();

            if (total > 1000) coinStr = (total / 1000) + "." + (total % 1000) + "K";
            if (total > 1000000) coinStr = (total / 1000000) + "." + (total % 1000000) + "M";

            if (usingAnim)
            {
                PlayAnimCoinIndexJumping(coinStr);
            }
            else
            {
                coinTxt.text = coinStr;
            }
        }
        private void PlayAnimCoinIndexJumping(string coinStr)
        {
            var isACtive = coinHolder.activeSelf;
            coinHolder.SetActive(true);
            animCoin?.Complete();
            animCoin = DOTween.Sequence()
                .Append(coinTxt.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 2))
                .Join(coinTxt.DOColor(Color.green, 0.3f))
                .Join(DOVirtual.DelayedCall(0.25f, () =>
                {
                    coinTxt.text = coinStr;
                }));
            animCoin.OnComplete(() =>
            {
                coinTxt.DOColor(Color.white, 0);
                coinHolder.SetActive(isACtive);
            });
        }
        public void Setup(bool showCoin, bool showEnergy)
        {
            coinHolder.SetActive(showCoin);
            energyHolder.SetActive(showEnergy);
        }

        private IEnumerator DelayClick()
        {
            canClick = false;
            yield return new WaitForSeconds(0.5f);
            canClick = true;
        }

        private void OnClickBackBtn()
        {
            if (!canClick) return;
            StartCoroutine("DelayClick");

            Holder.PlaySound?.Invoke();
            ClickBackBtn?.Invoke();
        }
    }
}
