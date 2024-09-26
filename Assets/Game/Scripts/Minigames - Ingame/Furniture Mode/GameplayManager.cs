using AnhNV.Helper;
using DG.Tweening;
using SCN;
using SCN.UIExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Base;

namespace WFSport.Gameplay.FurnitureMode
{
    [System.Serializable] 
    public struct Asset: IAsset
    {
        public Sprite[] toyData;
        public Sprite[] chairData;
        public Sprite[] otherData;
    }
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] private Topic[] topics;
        [SerializeField] private VerticalScrollInfinity[] scrollInfinitys;
        [SerializeField] private DecorItem toyPb;
        [SerializeField] private Chair chair;
        [SerializeField] private Transform limitToyArea;
        [SerializeField] private Transform toyArea;
        [SerializeField] private float toyReplaceRange;
        [SerializeField] private Canvas ui;

        private Asset myAsset;
        private IMinigame.ResultData result;
        private IMinigame.ConfigData myData;
        private Camera cam;
        private int topicId;
        /// <summary>
        /// x: Left, y: Up, z: Right, w: Down
        /// </summary>
        private Vector4 limitToyPos 
        {
            get => new Vector4(
                limitToyArea.GetChild(0).position.x,
                limitToyArea.GetChild(1).position.y,
                limitToyArea.GetChild(2).position.x,
                limitToyArea.GetChild(3).position.y);
        }
        private List<DecorItem> toysCreated;
        private Sequence camAnim;
        private LocalDataFurniture localData;
        private SpriteRenderer[] limitSpriteRender;
        private Sequence animLimit;
        private ToyScrollItem[] toyScrollItems;
        private OtherScrollItem[] otherScrollItems;
        private ChairScrollItem[] chairScrollItems;

        public IMinigame.ConfigData InternalData { get =>  myData; set => myData = value; }
        IMinigame.ResultData IMinigame.ExternalData { get => result; set => result = value; }

        private void Start()
        {
            EventDispatcher.Instance.RegisterListener<EventKeyBase.Purchase>(OnPurchase);
            Init();
        }

        private void OnDestroy()
        {
            camAnim?.Kill();
            animLimit?.Kill();
            EventDispatcher.Instance.RemoveListener<EventKeyBase.Purchase>(OnPurchase);
        }

        private void OnPurchase(EventKeyBase.Purchase data)
        {
            foreach (var scrollInfinity in scrollInfinitys)
            {
                scrollInfinity.PlayAutoMove();
            }
            if (data.chair != null)
            {
                data.chair.UnLock();
            }
            else if(data.other != null)
            {
                data.other.UnLock();
            }
            else if(data.toy != null)
            {
                data.toy.UnLock();
            }
        }

        private void PlayAnimToyLimit()
        {
            var time = 1;
            StopAnimToyLimit();
            animLimit = DOTween.Sequence()
                .Append(limitSpriteRender[0].DOFade(1, time))
                .Join(limitSpriteRender[1].DOFade(1, time))
                .Join(limitSpriteRender[2].DOFade(1, time))
                .Join(limitSpriteRender[3].DOFade(1, time));
            animLimit.SetLoops(-1, LoopType.Yoyo);
            if (toysCreated != null)
            {
                foreach (var item in toysCreated)
                {
                    item.Show();
                }
            }
        }
        private void StopAnimToyLimit()
        {
            animLimit?.Kill();
            animLimit = DOTween.Sequence()
                .Append(limitSpriteRender[0].DOFade(0, 0))
                .Join(limitSpriteRender[1].DOFade(0, 0))
                .Join(limitSpriteRender[2].DOFade(0, 0))
                .Join(limitSpriteRender[3].DOFade(0, 0));
            if(toysCreated != null)
            {
                foreach (var item in toysCreated)
                {
                    item.Hide();
                }
            }
        }

        private void OnClickTopic(Topic topic)
        {
            foreach (var item in topics)
            {
                if (item == topic) item.Active();
                else item.Deactive();
            }

            var idx = topic.Id;
            topicId = idx;
            if (!scrollInfinitys[idx].IsAlready)
            {
                if (topic.Type == Topic.Kind.Toy) { StartCoroutine("InitToyScrollData"); }
                else if (topic.Type == Topic.Kind.Chair) { StartCoroutine("InitChairScrollData"); }
                else if (topic.Type == Topic.Kind.Other) { StartCoroutine("InitOtherScrollData"); }
            }
            scrollInfinitys[idx].transform.SetAsLastSibling();

            if(topic.Type == Topic.Kind.Chair)
            {
                // Move Cam to Chair Position
                camAnim?.Kill();
                camAnim = DOTween.Sequence()
                    .Append(cam.transform.DOMoveX(chair.transform.position.x, 0.5f).SetEase(Ease.Linear));
                chair.PlayAnimTwinlink();
                StopAnimToyLimit();
            }
            else
            {
                // Move Cam to  Toy Position
                camAnim?.Kill();
                camAnim = DOTween.Sequence()
                    .Append(cam.transform.DOMoveX(limitToyArea.transform.position.x, 0.5f).SetEase(Ease.Linear));
                chair.StopAnimTwinlink();
                PlayAnimToyLimit();
            }
        }
        private void OnScrollItemDragInside(ScrollItem item)
        {
            if (item.TopicKind != Topic.Kind.Chair)
            {
                if (toysCreated == null) toysCreated = new List<DecorItem>();
                var count = 0;
                foreach (var toy in toysCreated)
                {
                    var distance = Vector2.Distance(toy.transform.position, item.transform.position);
                    if (distance <= toyReplaceRange)
                    {
                        toy.Replace(item.Icon);
                        localData.ToysCreated[count] = new CreatedToyData(toy.transform.localPosition, item.Order, toy.TopicKind);
                        return;
                    }
                    count++;
                }
                var localPos = toyArea.InverseTransformPoint(item.transform.position);
                localPos.z = 0;
                var toy2 = CreateToy(item.Icon, localPos, item.TopicKind);
                toysCreated.Add(toy2);
                localData.ToysCreated.Add(new CreatedToyData(localPos, item.Order, toy2.TopicKind));

                localData.Save();
            }
            else
            {
                chair.Replace(item.Icon);
                localData.createdChairId = item.Order;
            }
        }

        DecorItem CreateToy(Sprite icon, Vector3 position, Topic.Kind kind)
        {
            var decorItem = Instantiate(toyPb, toyArea);
            decorItem.transform.localPosition = position;
            decorItem.Setup(icon, kind);
            decorItem.Spawn(position);

            return decorItem;
        }

        private IEnumerator InitToyScrollData()
        {
            Debug.Log("INit Toy Data Begin");
            for (int i = 0; i < topics.Length; i++)
            {
                if (topics[i].Type == Topic.Kind.Toy) topicId = topics[i].Id;
            }
            var scrollview = scrollInfinitys[topicId];
            scrollview.Setup(myAsset.toyData.Length, this);

            yield return new WaitUntil(() => scrollview.MaskTrans.childCount == myAsset.toyData.Length);

            var records = scrollview.MaskTrans.GetComponentsInChildren<ToyScrollItem>();
            toyScrollItems = records;
            for (int i = 0; i < records.Length; i++) 
            {
                records[i].Setup(myAsset.toyData[i], i < localData.toyUnlocked.Length ? localData.toyUnlocked[i] : null, Topic.Kind.Toy);
                records[i].Setup(limitToyPos);
                records[i].OnDragInSide = OnScrollItemDragInside;
            }
            Debug.Log("INit Toy Data Complete");
        }

        private IEnumerator InitChairScrollData()
        {
            Debug.Log("INit Chair Data Begin");
            for (int i = 0; i < topics.Length; i++)
            {
                if (topics[i].Type == Topic.Kind.Chair) topicId = topics[i].Id;
            }
            var scrollview = scrollInfinitys[topicId];
            scrollview.Setup(myAsset.chairData.Length, this);

            yield return new WaitUntil(() => scrollview.MaskTrans.childCount == myAsset.chairData.Length);

            var records = scrollview.MaskTrans.GetComponentsInChildren<ChairScrollItem>();
            chairScrollItems = records;
            for (int i = 0; i < records.Length; i++) 
            {
                records[i].Setup(myAsset.chairData[i], i < localData.chairUnlocked.Length ? localData.chairUnlocked[i] : null, Topic.Kind.Chair);
                records[i].Setup(chair.transform.position);
                records[i].OnDragInSide = OnScrollItemDragInside;
            }
            Debug.Log("INit Chair Data COMpleted");
        }
        private IEnumerator InitOtherScrollData()
        {
            Debug.Log("INit Other Data Begin");
            for (int i = 0; i < topics.Length; i++)
            {
                if (topics[i].Type == Topic.Kind.Other) topicId = topics[i].Id;
            }
            var scrollview = scrollInfinitys[topicId];
            scrollview.Setup(myAsset.otherData.Length, this);

            yield return new WaitUntil(() => scrollview.MaskTrans.childCount == myAsset.otherData.Length);

            var records = scrollview.MaskTrans.GetComponentsInChildren<OtherScrollItem>();
            otherScrollItems = records;
            for (int i = 0; i < records.Length; i++) 
            {
                records[i].Setup(myAsset.otherData[i], i < localData.otherUnlocked.Length ? localData.otherUnlocked[i] : null, Topic.Kind.Other);
                records[i].Setup(limitToyPos);
                records[i].OnDragInSide = OnScrollItemDragInside;
            }
            Debug.Log("INit Other Data COMpleted");
        }
        private IEnumerator InitToysCreated()
        {
            if (!GameController.Instance.IsLoadLocalDataCompleted)
            {
                yield return new WaitForSeconds(0.5f);
            }

            yield return StartCoroutine(InitOtherScrollData());
            yield return StartCoroutine(InitToyScrollData());

            toysCreated = new List<DecorItem>(localData.ToysCreated.Count);
            Debug.Log("Total Toys Data: " + toysCreated.Count);
            foreach (var toyCreated in localData.ToysCreated)
            {
                if (toyCreated.TopicKind == Topic.Kind.Toy)
                {
                    var toy2 = CreateToy(toyScrollItems[toyCreated.Id].Icon, toyCreated.Position, toyCreated.TopicKind);
                    toysCreated.Add(toy2);
                }
                else if (toyCreated.TopicKind == Topic.Kind.Other)
                {
                    var toy2 = CreateToy(otherScrollItems[toyCreated.Id].Icon, toyCreated.Position, toyCreated.TopicKind);
                    toysCreated.Add(toy2);
                }
            }

            yield return new WaitForEndOfFrame();
            OnGameStart();
        }
        private IEnumerator InitCreatedChair()
        {
            if (!GameController.Instance.IsLoadLocalDataCompleted)
            {
                yield return new WaitForSeconds(0.5f);
            }

            yield return StartCoroutine(InitChairScrollData());

            chair.Setup(myAsset.chairData[localData.createdChairId], Topic.Kind.Chair);
        }

        private void Init()
        {
            cam = Camera.main;
            ui.worldCamera = Camera.main;

            for (int i = 0; i < topics.Length; i++)
            {
                topics[i].Setup(i, i == 0);
                topics[i].Click = OnClickTopic;
            }
            localData = GameController.Instance.FurnitureData;
            limitSpriteRender = limitToyArea.GetComponentsInChildren<SpriteRenderer>();

            myAsset = GameController.Instance.OrderAsset<Asset>(WFSport.Base.Minigame.Furniture);
            StartCoroutine(InitToysCreated());
            StartCoroutine(InitCreatedChair());
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
            OnClickTopic(topics[0]);
        }

        public void OnGameStop()
        {
        }

        public void OnGameWining()
        {
        }
    }
}
