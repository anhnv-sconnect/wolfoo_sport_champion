using AnhNV.GameBase;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WFSport.Base;

namespace WFSport.Home
{
    public class HomeManager : MonoBehaviour
    {
        [SerializeField] private EllipseLayout ellipseLayout;
        [SerializeField] private ModeItem modePb;
        [SerializeField] private Button settingBtn;
        [SerializeField] private Button counterBtn;
        private ConfigDataManager.GameplayConfigData[] gameplayData;

        private void Start()
        {
            settingBtn.onClick.AddListener(OnClickSetting);
            counterBtn.onClick.AddListener(OnClickCounter);
            InitData();
        }

        private void OnClickSetting()
        {
        }
        private void OnClickCounter()
        {
            // Create Energy Mode
            var createEnergyMode = gameplayData[0];
            foreach (var item in gameplayData)
            {
                if(item.Mode == GameController.Minigame.CreateEnergy)
                {
                    createEnergyMode = item;
                }
            }
            if (createEnergyMode.Mode != GameController.Minigame.CreateEnergy) return;
            EventDispatcher.Instance.Dispatch(new EventKeyBase.ChangeScene { gameplay = true, gameplayConfig = createEnergyMode });
        }

        private void InitData()
        {
            gameplayData = DataManager.Instance.configDataManager.GameplayConfig;
            var records = new Transform[gameplayData.Length];
            for (int i = 0; i < gameplayData.Length; i++)
            {
                if (gameplayData[i].icon != null)
                {
                    var mode = Instantiate(modePb, ellipseLayout.ItemHolder);
                    mode.Setup(i, gameplayData[i].icon, gameplayData[i]);
                    records[i] = mode.transform;
                }
            }
            ellipseLayout.Setup(records);
        }
    }
}
