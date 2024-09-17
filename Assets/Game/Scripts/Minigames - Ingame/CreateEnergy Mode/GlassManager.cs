using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace WFSport.Gameplay.CreateEnergyMode
{
    public class GlassManager : MonoBehaviour
    {
        [SerializeField] Glass[] allGlass;
        private int countGlass = -1;

        public int TotalGlass { get => allGlass.Length; }
        public Action<Glass> OnEndDrag;

        internal void SetUp(GameplayConfig config)
        {
            foreach (var glass in allGlass)
            {
                glass.Setup(config.pouringTime);
                glass.OnEndDrag = OnEndDrag;
            }
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

        internal void GetNextGlassofWater(System.Action<Glass> OnCompleted)
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
    }
}
