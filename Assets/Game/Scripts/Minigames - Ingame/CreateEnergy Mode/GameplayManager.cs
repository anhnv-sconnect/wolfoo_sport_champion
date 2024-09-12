using SCN.UIExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CreateEnergyMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] private Sprite[] tempData;
        [SerializeField] private VerticalScrollInfinity scrollInfinity;
        [SerializeField] private Blender blender;
        [SerializeField] private Fruit fruitPb;
        private IMinigame.Data myData;
        private int totalFruit;

        public IMinigame.Data ExternalData { get => myData; set => myData = value; }

        private void Start()
        {
            Init();
            OnGameStart();
        }
        private void OnDestroy()
        {
            StopCoroutine("InitDataScroll");
        }

        private void Init()
        {
            totalFruit = tempData.Length;
            scrollInfinity.Setup(tempData.Length, this);
            StartCoroutine("InitDataScroll");
        }
        private IEnumerator InitDataScroll()
        {
            yield return new WaitForEndOfFrame();
            if(scrollInfinity.ListItem.Count != totalFruit) { StartCoroutine("InitDataScroll"); }
            var count = 0;
            foreach (var item in scrollInfinity.MaskTrans.GetComponentsInChildren<FruitScrollItem>(true))
            {
                item.Setup(tempData[count], blender.transform, blender.FruitArea, blender.transform.position, fruitPb);
                count++;
            }
        }

        public void OnGameLosing()
        {
        }

        public void OnGamePause()
        {
        }

        public void OnGameResume()
        {
        }

        public void OnGameStart()
        {
        }

        public void OnGameStop()
        {
        }

        public void OnGameWining()
        {
        }
    }
}
