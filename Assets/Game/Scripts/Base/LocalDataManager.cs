using AnhNV.GameBase;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Gameplay.BasketballMode;

namespace WFSport.Base
{
    public class LocalDataManager :  SingletonBind<LocalDataManager>
    {
        public LocalDataCreateEnergy createEnergyData;

        private void Start()
        {
            EventDispatcher.Instance.RegisterListener<Gameplay.EventKey.UnlockLocalData>(OnUnlockLocalData);
            createEnergyData.Load();
        }

        private void OnUnlockLocalData(Gameplay.EventKey.UnlockLocalData data)
        {
            if(data.isFruit)
            {
                createEnergyData.UnlockFruit(data.id);
            }
            else if(data.isStraw)
            {
                createEnergyData.UnlockStraw(data.id);
            }
        }
    }
}
