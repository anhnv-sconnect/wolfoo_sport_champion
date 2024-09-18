using AnhNV.GameBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WFSport.Home
{
    public class HomeManager : MonoBehaviour
    {
        [SerializeField] private EllipseLayout ellipseLayout;
        [SerializeField] private ModeItem modePb;
        [SerializeField] private Sprite[] modeAsset;
        [SerializeField] private Button settingBtn;

        private ModeItem[] modeItems;
        private void Start()
        {
            settingBtn.onClick.AddListener(OnClickSetting);
            InitData();
        }

        private void OnClickSetting()
        {
        }

        private void InitData()
        {
            modeItems = new ModeItem[modeAsset.Length];
            var records = new Transform[modeAsset.Length];
            for (int i = 0; i < modeAsset.Length; i++)
            {
                var mode = Instantiate(modePb, ellipseLayout.ItemHolder);
                mode.Setup(i, modeAsset[i]);
                records[i] = mode.transform;
            }
            ellipseLayout.Setup(records);
        }
    }
}
