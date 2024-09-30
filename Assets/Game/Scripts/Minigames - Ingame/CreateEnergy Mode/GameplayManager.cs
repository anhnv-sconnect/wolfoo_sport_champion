using SCN;
using SCN.UIExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Base;
using WFSport.Helper;

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
        private int grindingCount;
        private int countStraw;
        private int playerDrinkingCount;
        private Glass orderingGlass;

        private IMinigame.ConfigData myData;
        private IMinigame.ResultData result;
        private LocalDataCreateEnergy localData;

        public IMinigame.ConfigData InternalData { get => myData; set => myData = value; }
        IMinigame.ResultData IMinigame.ExternalData { get => result; set => result = value; }

        private void Start()
        {
            EventDispatcher.Instance.RegisterListener<EventKeyBase.Purchase>(OnPurchase);
            StartCoroutine("Init");
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener<EventKeyBase.Purchase>(OnPurchase);
        }
        private void OnPurchase(EventKeyBase.Purchase data)
        {
            fruitScrollInfinity.PlayAutoMove();
            if (data.fruit != null)
            {
                data.fruit.UnLock();
            }
            else if(data.straw != null)
            {
                data.straw.UnLock();
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
                BlenderGrinding();
            });
        }
        private void BlenderGrinding()
        {
            if(countFruitInBlender == config.maxFruitInBlender)
            {
                blender.Grind();
                return;
            }
            if (countFruitInBlender > 1) 
            {
                blender.SetGrind();
                blender.OnGrindingComplete = OnBlendingCompleted;
            }
        }

        private void OnBlendingCompleted()
        {
            fruitScrollInfinity.MoveOut();
            glassManager.MoveIn(grindingCount > 1, () =>
            {
                // Pouring
                glassManager.GetNextGlassofWaterToPouringWater((glass) =>
                {
                    blender.Pouring(glass.transform.position + new Vector3(-0.8f, 1, 0),
                    () =>
                    {
                        glass.OnGettingWater();
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
                        else
                        {
                            fruitScrollInfinity.MoveIn();
                        }
                    });
                });
            });
        }

        private void CreateStraw(StrawScrollItem scrollItem)
        {
            if (countStraw >= totalGlass) return;
            if (orderingGlass == null) return;
            if (orderingGlass.IsJumping || orderingGlass.HasStraw) return;
            countStraw++;

            var straw = Instantiate(strawPb);
            straw.transform.position = scrollItem.transform.position;
            straw.Setup(scrollItem.Icon);
            straw.JumpTo(orderingGlass.transform.position, orderingGlass.transform, () =>
            {
                orderingGlass.StrawJumpIn();
                GlassJumpBack();
            });
        }
        private void GlassJumpBack()
        {
            orderingGlass.JumpBacktoTray(() =>
            {
                if (countStraw == totalGlass)
                {
                    strawScrollInfinity.MoveOut();
                    glassManager.MoveToRight(() =>
                    {
                        GameController.Instance.SystemUI.Setup(false, true);
                        player.MoveIn(() =>
                        {
                            glassManager.EnableDrag();
                        });
                    });
                    return;
                }
                else
                {
                    glassManager.GetNextGlassofWaterToGetStraw((glass) =>
                    {
                        orderingGlass = glass;
                    });
                }
            });
        }

        private IEnumerator Init()
        {
            myData = DataTransporter.GameplayConfig;
            result = new IMinigame.ResultData();

            yield return new WaitUntil(() => GameController.Instance.IsLoadSceneCompleted);

            localData = GameController.Instance.CreateEnergyData;

            fruitData = asset.fruitData;
            totalFruit = fruitData.Length;
            fruitScrollInfinity.MoveOut(true);

            strawData = asset.strawData;
            totalStraw = strawData.Length;
            strawScrollInfinity.MoveOut(true);

            blender.Setup();

            glassManager.SetUp(config);
            glassManager.MoveOut(true);
            glassManager.OnGlassEndDrag = OnGlassEndDrag;

            OnGameStart();
        }

        private void OnGlassEndDrag(Glass glass)
        {
            if (Vector2.Distance(glass.transform.position, player.MouthPos) < 2)
            {
                glassManager.DisableDrag(glass);
                glass.ReleaseWater(player.MouthPos);
                player.Drink(() =>
                {
                    playerDrinkingCount++;
                    player.PlayWining();
                    GameController.Instance.UpdateEnergy(true, () =>
                    {
                        if (playerDrinkingCount == totalGlass)
                        {
                            OnGameWining();
                        }
                    });
                });
            }
        }

        private IEnumerator InitFruitDataScroll()
        {
            fruitScrollInfinity.Setup(totalFruit, this);
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
                item.Setup(fruitData[count], 
                    blender, 
                    count < localData.fruitUnlocked.Length ? localData.fruitUnlocked[count] : null);
                fruitScrollItems[count] = item;
                item.OnDragInSide = CreateFruit;
                count++;
            }

            fruitScrollInfinity.MoveIn();
            yield return new WaitForSeconds(0.5f);

            blender.OpenLid(null);
        }
        private IEnumerator InitStrawDataScroll()
        {
            strawScrollInfinity.Setup(totalStraw, this);
            yield return new WaitUntil(() =>
            {
                return strawScrollInfinity.ListItem.Count == totalStraw;
            });
            yield return new WaitForEndOfFrame();

            var items = strawScrollInfinity.MaskTrans.GetComponentsInChildren<StrawScrollItem>(true);
            strawScrollItems = new StrawScrollItem[items.Length];

            glassManager.GetNextGlassofWaterToGetStraw((glass) =>
            {
                orderingGlass = glass;
                var count = 0;
                foreach (var item in items)
                {
                    item.Setup(strawData[count], glass.transform.position,
                            count < localData.strawUnlocked.Length ? localData.strawUnlocked[count] : null);
                    strawScrollItems[count] = item;
                    item.OnDragInSide = CreateStraw;
                    count++;
                }
                strawScrollInfinity.MoveIn();
            });
        }

        public void OnGameLosing()
        {
            OnEndgame(IMinigame.MatchResult.Lose);
        }

        public void OnGamePause()
        {
        }

        public void OnGameResume()
        {
        }

        public void OnGameStart()
        {
            StartCoroutine("InitFruitDataScroll");
        }

        public void OnGameStop()
        {
            OnEndgame(IMinigame.MatchResult.Lose);
        }
        private void OnEndgame(IMinigame.MatchResult matchResult)
        {
            result.gamestate = matchResult;
            EventDispatcher.Instance.Dispatch(new EventKey.OnGameStop { data = result });
        }
        public void OnGameWining()
        {
            Debug.Log("ONGame WIning");
            glassManager.MoveOut(false);
            OnEndgame(IMinigame.MatchResult.Win);
        }
    }
}
