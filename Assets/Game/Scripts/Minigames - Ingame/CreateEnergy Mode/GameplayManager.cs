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
        [SerializeField] private GameplayConfig config;
        [SerializeField] private GlassManager glasManager;

        private int totalFruit;
        private int countFruitInBlender;

        private FruitScrollItem[] scrollItems;
        private IMinigame.Data myData;
        public IMinigame.Data ExternalData { get => myData; set => myData = value; }

        private void Start()
        {
            Init();
            OnGameStart();
        }
        private void OnDestroy()
        {
            StopCoroutine("InitDataScroll");
            if (scrollItems != null)
            {
                for (int i = 0; i < scrollItems.Length; i++)
                {
                    if (scrollItems[i] != null) scrollItems[i].OnDragInSide -= CreateFruit;
                }
            }
        }

        private void CreateFruit(FruitScrollItem scrollItem)
        {
            if (countFruitInBlender >= config.maxFruitInBlender) return;
            countFruitInBlender++;

            var fruit = Instantiate(fruitPb);
            fruit.transform.position = transform.position;
            fruit.Setup(scrollItem.Icon);
            fruit.JumpTo(blender.FruitArea.position, blender.FruitArea.holder, () =>
            {
                if (countFruitInBlender == config.maxFruitInBlender)
                {
                    blender.Grind(() =>
                    {
                        // Pouring
                        glasManager.GetNextGlassofWater((glass) =>
                        {
                            blender.Pouring(glass.transform.position,
                            () =>
                            {
                                glass.OnPouringWater();
                            },
                            () =>
                            {
                                countFruitInBlender = 0;
                                glass.JumpBacktoTray(null);
                            });
                        });
                    });
                    return;
                }
            });
        }

        private void Init()
        {
            totalFruit = tempData.Length;
            scrollInfinity.Setup(tempData.Length, this);
            blender.Setup();
            glasManager.SetUp(config);
            StartCoroutine("InitDataScroll");
        }
        private IEnumerator InitDataScroll()
        {
            yield return new WaitForEndOfFrame();
            if(scrollInfinity.ListItem.Count != totalFruit) { StartCoroutine("InitDataScroll"); }
            var count = 0;
            var items = scrollInfinity.MaskTrans.GetComponentsInChildren<FruitScrollItem>(true);
            scrollItems = new FruitScrollItem[items.Length];
            foreach (var item in items)
            {
                item.Setup(tempData[count],blender.transform.position);
                items[count] = item;
                item.OnDragInSide += CreateFruit;
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
            blender.OpenLid(null);
        }

        public void OnGameStop()
        {
        }

        public void OnGameWining()
        {
        }
    }
}
