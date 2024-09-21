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
        [SerializeField] private TMPro.TMP_Text coinTxt;
        [SerializeField] private Image[] energyImgs;
        [SerializeField] private GameObject energyHolder;
        [SerializeField] private GameObject coinHolder;

        private bool canClick = true;
        public Action ClickBackBtn;
        private PlayerMe playerMe;
        private Sequence animCoin;

        // Start is called before the first frame update
        void Start()
        {
            backBtn.onClick.AddListener(OnClickBackBtn);
            playerMe = DataManager.Instance.localSaveloadData.playerMe;
        }
        private void OnDestroy()
        {
            animCoin?.Kill();
        }
        public void UpdateCoin()
        {
            var total = playerMe.totalCoin;
            string coinStr = total.ToString();

            if (total > 1000) coinStr = (total / 1000) + "." + (total % 1000) + "K";
            if (total > 1000000) coinStr = (total / 1000000) + "." + (total % 1000000) + "M";

            animCoin?.Complete();
            animCoin = DOTween.Sequence()
                .Append(coinTxt.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 2));

            coinTxt.text = coinStr;
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
