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
        [SerializeField] private GlassManager glassManager;
        [SerializeField] private Player player;

        private int totalFruit;
        private int totalStraw;
        private int countFruitInBlender;
        private int totalGlass => glassManager.TotalGlass;

        private Sprite[] fruitData;
        private Sprite[] strawData;
        private FruitScrollItem[] fruitScrollItems;
        private StrawScrollItem[] strawScrollItems;
        private IMinigame.Data myData;
        private int grindingCount;
        private int countStraw;
        private Glass orderingGlass;

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
                        glassManager.GetNextGlassofWater((glass) =>
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
                                if (grindingCount == totalGlass)
                                {
                                    blender.MoveOut();
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
            if (countStraw > totalGlass) return;
            if (countStraw == totalGlass)
            {
                player.MoveIn(() =>
                {
                    glassManager.EnableDrag();
                });
                return;
            }
            countStraw++;

            var straw = Instantiate(strawPb);
            straw.transform.position = scrollItem.transform.position;
            straw.Setup(scrollItem.Icon);
            straw.JumpTo(orderingGlass.transform.position, orderingGlass.transform, () =>
            {
                orderingGlass.JumpBacktoTray(() =>
                {
                    glassManager.GetNextGlassofWater((glass) =>
                    {
                        orderingGlass = glass;
                    });
                });
            });
        }

        private void Init()
        {
            fruitData = asset.fruitData;
            totalFruit = fruitData.Length;
            fruitScrollInfinity.Setup(totalFruit, this);
            fruitScrollInfinity.MoveOut(true);

            strawData = asset.strawData;
            totalStraw = strawData.Length;
            strawScrollInfinity.Setup(totalStraw, this);
            strawScrollInfinity.MoveOut(true);

            blender.Setup();

            glassManager.SetUp(config);
            glassManager.OnEndDrag = OnGlassEndDrag;
        }

        private void OnGlassEndDrag(Glass glass)
        {
            player.Drink();
        }

        private IEnumerator InitFruitDataScroll()
        {
            yield return new WaitUntil(() =>
            {
                return fruitScrollInfinity.ListItem.Count == totalFruit;
            });
            yield return new WaitForEndOfFrame();

            var items = fruitScrollInfinity.MaskTrans.GetComponentsInChildren<FruitScrollItem>(true);
            fruitScrollItems = new FruitScrollItem[items.Length];
            var count = 0;
            foreach (var item in items)
            {
                item.Setup(fruitData[count],blender.transform.position);
                fruitScrollItems[count] = item;
                item.OnDragInSide += CreateFruit;
                count++;
            }
            fruitScrollInfinity.MoveIn();
        }
        private IEnumerator InitStrawDataScroll()
        {
            yield return new WaitUntil(() =>
            {
                return strawScrollInfinity.ListItem.Count == totalStraw;
            });
            yield return new WaitForEndOfFrame();

            var items = strawScrollInfinity.MaskTrans.GetComponentsInChildren<StrawScrollItem>(true);
            strawScrollItems = new StrawScrollItem[items.Length];

            glassManager.GetNextGlassofWater((glass) =>
            {
                orderingGlass = glass;
                // Move StrawScroll in Screen
                var count = 0;
                foreach (var item in items)
                {
                    item.Setup(strawData[count], glass.transform.position);
                    strawScrollItems[count] = item;
                    item.OnDragInSide += CreateStraw;
                    count++;
                }
                strawScrollInfinity.MoveIn();
            });
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
            StartCoroutine("InitStrawDataScroll");
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
