using AnhNV.GameBase;
using DG.Tweening;
using SCN;
using SCN.IAP;
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
        [SerializeField] private ParticleSystem lightingFx;
        private ConfigDataManager.GameplayConfigData[] gameplayData;
        private LocalDataFurniture furnitureData;
        private Gameplay.FurnitureMode.AssetConfig furnitureAsset;
        private Sequence animRefresh;

        private void Start()
        {
            counterBtn.onClick.AddListener(OnClickCounter);
            createdToyBtn.onClick.AddListener(OnClickCreatedToy);

            InitData();
            PlayAnimRefreshFurniture();
        }
        private void OnDestroy()
        {
            animRefresh?.Kill();
        }
        public void OpenIAP()
        {
            IAPManager.Instance.OpenRemoveAdsPanel();
        }

        private void PlayAnimRefreshFurniture()
        {
            lightingFx.transform.position = new Vector3(lightingFx.transform.position.x, 4.3f, 0);
            animRefresh = DOTween.Sequence()
                .Append(lightingFx.transform.DOMoveY(-0.6f, 2).SetEase(Ease.Linear));
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

            furnitureAsset = GameController.Instance.OrderAsset<Gameplay.FurnitureMode.AssetConfig>(Minigame.Furniture);

            if (furnitureAsset == null)
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

            yield return new WaitForEndOfFrame();
            ellipseLayout.Representation();
        }
        private void CreateToy(Sprite icon, Vector3 localPos)
        {
                SoundManager.Instance.PlayTest1();
            var toy = Instantiate(toypb, toyArea);
            toy.transform.localPosition = localPos;
            toy.sprite = icon;
            toy.transform.localScale = Vector3.one;
        }
    }
}
