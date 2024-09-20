using DG.Tweening;
using SCN.UIExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        private List<DecorItem> toysCreateds;
        private Sequence camAnim;

        public IMinigame.ConfigData InternalData { get =>  myData; set => myData = value; }
        IMinigame.ResultData IMinigame.ExternalData { get => result; set => result = value; }

        private void Start()
        {
            Init();
        }
        private void OnDestroy()
        {
            foreach (var topic in topics)
            {
                topic.Click -= OnClickTopic;
            }
            camAnim?.Kill();
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
            }
            else
            {
                // Move Cam to  Toy Position
                camAnim?.Kill();
                camAnim = DOTween.Sequence()
                    .Append(cam.transform.DOMoveX(limitToyArea.transform.position.x, 0.5f).SetEase(Ease.Linear));
            }
        }
        private void OnScrollItemDragInside(ScrollItem item)
        {
            if (item.TopicKind != Topic.Kind.Chair)
            {
                if (toysCreateds == null) toysCreateds = new List<DecorItem>();
                foreach (var toy in toysCreateds)
                {
                    var distance = Vector2.Distance(toy.transform.position, item.transform.position);
                    if (distance <= toyReplaceRange)
                    {
                        toy.Replace(item.Icon);
                        return;
                    }
                }
                var toy2 = CreateToy(item.Icon, item.transform.position);
                toysCreateds.Add(toy2);
            }
            else
            {
                chair.Replace(item.Icon);
            }
        }

        DecorItem CreateToy(Sprite icon, Vector3 position)
        {
            var decorItem = Instantiate(toyPb, position, toyPb.transform.rotation, toyArea);
            decorItem.Setup(icon);
            decorItem.Spawn(position);

            return decorItem;
        }

        private IEnumerator InitToyScrollData()
        {
            var scrollview = scrollInfinitys[topicId];
            scrollview.Setup(toyData.Length, this);

            yield return new WaitUntil(() => scrollview.MaskTrans.childCount == toyData.Length);

            var records = scrollview.MaskTrans.GetComponentsInChildren<ToyScrollItem>();
            for (int i = 0; i < records.Length; i++) 
            {
                records[i].Setup(toyData[i], false, Topic.Kind.Toy);
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
            for (int i = 0; i < records.Length; i++) 
            {
                records[i].Setup(chairData[i], false, Topic.Kind.Chair);
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
            for (int i = 0; i < records.Length; i++) 
            {
                records[i].Setup(otherData[i], false, Topic.Kind.Other);
                records[i].Setup(limitToyPos);
                records[i].OnDragInSide = OnScrollItemDragInside;
            }
        }

        private void Init()
        {
            cam = Camera.main;

            var first = 0;
            for (int i = 0; i < topics.Length; i++)
            {
                topics[i].Setup(i, i == first);
                topics[i].Click += OnClickTopic;
            }
            OnClickTopic(topics[first]);
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
