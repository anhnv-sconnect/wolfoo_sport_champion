using SCN.UIExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CreateEnergyMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] private VerticalScrollInfinity fruitScrollInfinity;
        [SerializeField] private VerticalScrollInfinity strawScrollInfinity;
        [SerializeField] private Blender blender;
        [SerializeField] private Fruit fruitPb;
        [SerializeField] private Straw strawPb;
        [SerializeField] private GameplayConfig config;
        [SerializeField] private AssetConfig asset;
        [SerializeField] private GlassManager glasManager;

        private int totalFruit;
        private int totalStraw;
        private int countFruitInBlender;

        private Sprite[] fruitData;
        private Sprite[] strawData;
        private FruitScrollItem[] fruitScrollItems;
        private StrawScrollItem[] strawScrollItems;
        private IMinigame.Data myData;
        private int grindingCount;

        public IMinigame.Data ExternalData { get => myData; set => myData = value; }

        private void Start()
        {
            Init();
            OnGameStart();
        }
        private void OnDestroy()
        {
            StopCoroutine("InitFruitDataScroll");
            if (fruitScrollItems != null)
            {
                for (int i = 0; i < fruitScrollItems.Length; i++)
                {
                    if (fruitScrollItems[i] != null) fruitScrollItems[i].OnDragInSide -= CreateFruit;
                }
            }
            if (strawScrollItems != null)
            {
                for (int i = 0; i < strawScrollItems.Length; i++)
                {
                    if (strawScrollItems[i] != null) strawScrollItems[i].OnDragInSide -= CreateStraw;
                }
            }
        }

        private void CreateFruit(FruitScrollItem scrollItem)
        {
            if (countFruitInBlender >= config.maxFruitInBlender) return;
            countFruitInBlender++;

            var fruit = Instantiate(fruitPb);
            fruit.transform.position = scrollItem.transform.position;
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

                                grindingCount++;
                                if (grindingCount == glasManager.TotalGlass)
                                {
                                    StartCoroutine("InitStrawDataScroll");
                                }
                            });
                        });
                    });
                    return;
                }
            });
        }
        private void CreateStraw(StrawScrollItem scrollItem)
        {
            if (countFruitInBlender >= config.maxFruitInBlender) return;
            countFruitInBlender++;

            var straw = Instantiate(strawPb);
            straw.transform.position = scrollItem.transform.position;
            straw.Setup(scrollItem.Icon);
        }

        private void Init()
        {
            totalFruit = fruitData.Length;
            totalStraw = strawData.Length;
            fruitScrollInfinity.Setup(totalFruit, this);
            strawScrollInfinity.Setup(totalStraw, this);
            blender.Setup();
            glasManager.SetUp(config);
            StartCoroutine("InitFruitDataScroll");
        }
        private IEnumerator InitFruitDataScroll()
        {
            yield return new WaitForEndOfFrame();
            if(fruitScrollInfinity.ListItem.Count != totalFruit) { StartCoroutine("InitFruitDataScroll"); }
            var count = 0;
            var items = fruitScrollInfinity.MaskTrans.GetComponentsInChildren<FruitScrollItem>(true);
            fruitScrollItems = new FruitScrollItem[items.Length];
            foreach (var item in items)
            {
                item.Setup(fruitData[count],blender.transform.position);
                items[count] = item;
                item.OnDragInSide += CreateFruit;
                count++;
            }
        }
        private IEnumerator InitStrawDataScroll()
        {
            yield return new WaitForEndOfFrame();
            if(strawScrollInfinity.ListItem.Count != totalFruit) { StartCoroutine("InitStrawDataScroll"); }
            var count = 0;
            var items = strawScrollInfinity.MaskTrans.GetComponentsInChildren<StrawScrollItem>(true);
            fruitScrollItems = new FruitScrollItem[items.Length];
            foreach (var item in items)
            {
                item.Setup(fruitData[count],blender.transform.position);
                items[count] = item;
                item.OnDragInSide += CreateStraw;
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
