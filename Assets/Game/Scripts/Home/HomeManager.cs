using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Home
{
    public class HomeManager : MonoBehaviour
    {
        [SerializeField] private EllipseLayout ellipseLayout;
        [SerializeField] private ModeItem modePb;
        [SerializeField] private Sprite[] modeAsset;
        private ModeItem[] modeItems;
        private void Start()
        {
            InitData();
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
