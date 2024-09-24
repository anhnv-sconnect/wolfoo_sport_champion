using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using WFSport.Helper;

namespace WFSport.Gameplay.CreateEnergyMode
{
    public class GlassManager : MonoBehaviour
    {
        [SerializeField] Glass[] allGlass;
        private int countGlass = -1;

        public int TotalGlass { get => allGlass.Length; }
        public Action<Glass> OnGlassEndDrag;
        private Sequence moveAnim;
        private Vector2 outSide;
        private Vector3 initPos;

        private void OnDestroy()
        {
            moveAnim?.Kill();
            foreach (var glass in allGlass)
            {
                glass.OnEndDrag -= OnEndDrag;
            }
        }

        internal void SetUp(GameplayConfig config)
        {
            foreach (var glass in allGlass)
            {
                glass.Setup(config.pouringTime);
                glass.OnEndDrag += OnEndDrag;
            }
            outSide = ScreenHelper.GetMaxPosition() + new Vector2(6, 1);
            initPos = transform.position;
        }
        void OnEndDrag(Glass glass)
        {
            OnGlassEndDrag?.Invoke(glass);
        }

        internal void EnableDrag()
        {
            foreach (var glass in allGlass)
            {
                glass.SetupDrag(true);
            }
        }
        internal void DisableDrag()
        {
            foreach (var glass in allGlass)
            {
                glass.SetupDrag(false);
            }
        }
        internal void DisableDrag(Glass glass)
        {
            glass.SetupDrag(false);
        }

        internal void MoveToRight(Action OnComplete)
        {
            moveAnim?.Kill();
            moveAnim = DOTween.Sequence()
                .Append(transform.DOMove(new Vector3(4.5f, -3, 0), 0.5f));
            moveAnim.OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        }
        internal void MoveOut(bool isImmediately)
        {
            moveAnim?.Kill();
            if(isImmediately)
            {
                transform.position = new Vector3(-outSide.x, initPos.y, initPos.z);
                return;
            }
            moveAnim = DOTween.Sequence()
                .Append(transform.DOMoveX(outSide.x, 0.5f));
        }
        internal void MoveIn(bool isImmediately, System.Action OnComplete)
        {
            moveAnim?.Kill();
            if(isImmediately)
            {
                transform.position = initPos;
                OnComplete?.Invoke();
                return;
            }
            moveAnim = DOTween.Sequence()
                .Append(transform.DOMove(initPos, 0.5f));
            moveAnim.OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        }

        internal void GetNextGlassofWaterToPouringWater(System.Action<Glass> OnCompleted)
        {
            countGlass++;
            if (countGlass >= allGlass.Length) countGlass = 0;

            var endPos = new Vector2(-4, -2);
            allGlass[countGlass].transform.SetParent(transform.parent);
            allGlass[countGlass].JumpOutOfTray(endPos, () =>
            {
                OnCompleted?.Invoke(allGlass[countGlass]);
            });
        }
        internal void GetNextGlassofWaterToGetStraw(System.Action<Glass> OnCompleted)
        {
            countGlass++;
            if (countGlass >= allGlass.Length) countGlass = 0;

            var endPos = new Vector2(1.55f, -2.4f);
            allGlass[countGlass].transform.SetParent(transform.parent);
            allGlass[countGlass].JumpOutOfTray(endPos, () =>
            {
                OnCompleted?.Invoke(allGlass[countGlass]);
            });
        }
    }
}
