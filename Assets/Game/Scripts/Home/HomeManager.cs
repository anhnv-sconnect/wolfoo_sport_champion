using AnhNV.GameBase;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WFSport.Base;

namespace WFSport.Home
{
    public class HomeManager : MonoBehaviour
    {
        [SerializeField] private EllipseLayout ellipseLayout;
        [SerializeField] private ModeItem modePb;
        [SerializeField] private Button counterBtn;
        [SerializeField] private Button createdToyBtn;
        [SerializeField] private Transform toyArea;
        [SerializeField] private SpriteRenderer toypb;
        private ConfigDataManager.GameplayConfigData[] gameplayData;
        private LocalDataFurniture furnitureData;
        private Gameplay.FurnitureMode.Asset furnitureAsset;

        private void Start()
        {
            counterBtn.onClick.AddListener(OnClickCounter);
            createdToyBtn.onClick.AddListener(OnClickCreatedToy);
            InitData();
        }

        private void OnClickCreatedToy()
        {
            EventDispatcher.Instance.Dispatch(new EventKeyBase.ChangeScene
            {
                minigame = Minigame.Furniture,
                gameplay = true,
            });
        }

        private void OnClickCounter()
        {
            EventDispatcher.Instance.Dispatch(new EventKeyBase.ChangeScene
            {
                minigame = Minigame.CreateEnergy,
                gameplay = true,
            });
        }

        private void InitData()
        {
            var gameplayData = GameController.Instance.OrderAllMainGameplay();
            furnitureData = GameController.Instance.FurnitureData;

            var array = new Transform[gameplayData.Count()];
            var count = 0;
            foreach (var item in gameplayData)
            {
                var mode = Instantiate(modePb, ellipseLayout.ItemHolder);
                mode.Setup(item.icon, item.Mode);
                array[count] = mode.transform;
                count++;
            }

            ellipseLayout.Setup(array);

            StartCoroutine(SpawnCreatedToy());
        }
        private IEnumerator SpawnCreatedToy()
        {
            if (!GameController.Instance.IsLoadLocalDataCompleted)
            {
                yield return new WaitForSeconds(0.5f);
            }

            furnitureAsset = GameController.Instance.OrderAsset<Gameplay.FurnitureMode.Asset>(Minigame.Furniture);

            if (furnitureAsset.Equals(default(Gameplay.FurnitureMode.Asset)))
            {
                Debug.LogError("<!> Your Asset is Empty <!>");
            }
            else
            {
                foreach (var toyCreated in furnitureData.ToysCreated)
                {
                    if (toyCreated.TopicKind == Gameplay.FurnitureMode.Topic.Kind.Toy)
                    {
                        CreateToy(furnitureAsset.toyData[toyCreated.Id], toyCreated.Position);
                    }
                    else if (toyCreated.TopicKind == Gameplay.FurnitureMode.Topic.Kind.Other)
                    {
                        CreateToy(furnitureAsset.otherData[toyCreated.Id], toyCreated.Position);
                    }
                }
                createdToyBtn.image.sprite = furnitureAsset.chairData[furnitureData.createdChairId];
            }
        }
        private void CreateToy(Sprite icon, Vector3 localPos)
        {
            var toy = Instantiate(toypb, toyArea);
            toy.transform.localPosition = localPos;
            toy.sprite = icon;
            toy.transform.localScale = Vector3.one;
        }
    }
}
