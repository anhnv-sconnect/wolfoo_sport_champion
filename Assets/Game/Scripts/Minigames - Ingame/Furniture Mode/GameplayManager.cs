using AnhNV.Helper;
using DG.Tweening;
using SCN;
using SCN.UIExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Base;
using WFSport.Gameplay.CatchMoreToysMode;

namespace WFSport.Gameplay.FurnitureMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] private Topic[] topics;
        [SerializeField] private VerticalScrollInfinity[] scrollInfinitys;
        [SerializeField] private DecorItem toyPb;
        [SerializeField] private Chair chair;
        [SerializeField] private Sprite[] toyData;
        [SerializeField] private Sprite[] chairData;
        [SerializeField] private Sprite[] otherData;
        [SerializeField] private Transform limitToyArea;
        [SerializeField] private Transform toyArea;
        [SerializeField] private float toyReplaceRange;

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
            if(data.chair != null)
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
                        localData.ToysCreated[count] = (toy.transform.position, item.Order, toy.TopicKind);
                        return;
                    }
                    count++;
                }
                var toy2 = CreateToy(item.Icon, item.transform.position, item.TopicKind);
                toysCreated.Add(toy2);
                localData.ToysCreated.Add((toy2.transform.position, item.Order, toy2.TopicKind));

                localData.Save();
            }
            else
            {
                chair.Replace(item.Icon);
            }
        }

        DecorItem CreateToy(Sprite icon, Vector3 position, Topic.Kind kind)
        {
            var decorItem = Instantiate(toyPb, position, toyPb.transform.rotation, toyArea);
            decorItem.Setup(icon, kind);
            decorItem.Spawn(position);

            return decorItem;
        }

        private IEnumerator InitToyScrollData()
        {
            var scrollview = scrollInfinitys[topicId];
            scrollview.Setup(toyData.Length, this);

            yield return new WaitUntil(() => scrollview.MaskTrans.childCount == toyData.Length);

            var records = scrollview.MaskTrans.GetComponentsInChildren<ToyScrollItem>();
            toyScrollItems = records;
            for (int i = 0; i < records.Length; i++) 
            {
                records[i].Setup(toyData[i], i < localData.toyUnlocked.Length ? localData.toyUnlocked[i] : null, Topic.Kind.Toy);
                records[i].Setup(limitToyPos);
                records[i].OnDragInSide = OnScrollItemDragInside;
            }
        }

        private IEnumerator InitChairScrollData()
        {
            var scrollview = scrollInfinitys[topicId];
            scrollview.Setup(chairData.Length, this);

            yield return new WaitUntil(() => scrollview.MaskTrans.childCount == chairData.Length);

            var records = scrollview.MaskTrans.GetComponentsInChildren<ChairScrollItem>();
            chairScrollItems = records;
            for (int i = 0; i < records.Length; i++) 
            {
                records[i].Setup(chairData[i], i < localData.chairUnlocked.Length ? localData.chairUnlocked[i] : null, Topic.Kind.Chair);
                records[i].Setup(chair.transform.position);
                records[i].OnDragInSide = OnScrollItemDragInside;
            }
        }
        private IEnumerator InitOtherScrollData()
        {
            var scrollview = scrollInfinitys[topicId];
            scrollview.Setup(otherData.Length, this);

            yield return new WaitUntil(() => scrollview.MaskTrans.childCount == otherData.Length);

            var records = scrollview.MaskTrans.GetComponentsInChildren<OtherScrollItem>();
            otherScrollItems = records;
            for (int i = 0; i < records.Length; i++) 
            {
                records[i].Setup(otherData[i], i < localData.otherUnlocked.Length ? localData.otherUnlocked[i] : null, Topic.Kind.Other);
                records[i].Setup(limitToyPos);
                records[i].OnDragInSide = OnScrollItemDragInside;
            }
        }
        private IEnumerator InitToysCreated()
        {
            yield return new WaitForSeconds(1);

            yield return StartCoroutine("InitOtherScrollData");
            yield return StartCoroutine("InitToyScrollData");

            toysCreated = new List<DecorItem>(localData.ToysCreated.Count);
            Debug.Log("Total Toys Data: " + toysCreated.Count);
            foreach (var toyCreated in localData.ToysCreated)
            {
                if (toyCreated.TopicKind == Topic.Kind.Toy)
                {
                    var toy2 = CreateToy(toyScrollItems[toyCreated.id].Icon, toyCreated.position, toyCreated.TopicKind);
                    toysCreated.Add(toy2);
                }
                else if (toyCreated.TopicKind == Topic.Kind.Other)
                {
                    var toy2 = CreateToy(otherScrollItems[toyCreated.id].Icon, toyCreated.position, toyCreated.TopicKind);
                    toysCreated.Add(toy2);
                }
            }

            yield return new WaitForEndOfFrame();
            OnGameStart();
        }

        private void Init()
        {
            cam = Camera.main;

            for (int i = 0; i < topics.Length; i++)
            {
                topics[i].Setup(i, i == 0);
                topics[i].Click = OnClickTopic;
            }
            localData = DataManager.Instance.localSaveloadData.furnitureData;
            limitSpriteRender = limitToyArea.GetComponentsInChildren<SpriteRenderer>();

            StartCoroutine("InitToysCreated");
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
