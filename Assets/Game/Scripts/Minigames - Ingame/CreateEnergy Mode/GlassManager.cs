using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CreateEnergyMode
{
    public class GlassManager : MonoBehaviour
    {
        [SerializeField] Glass[] allGlass;
        private int countGlass = -1;

        public int TotalGlass { get; private set; }

        internal void SetUp(GameplayConfig config)
        {
            foreach (var glass in allGlass)
            {
                glass.Setup(config.pouringTime);
            }
        }

        internal void GetNextGlassofWater(System.Action<Glass> OnCompleted)
        {
            countGlass++;
            if (countGlass >= allGlass.Length) return;

            var endPos = new Vector2(-4, -2);
            allGlass[countGlass].transform.SetParent(transform.parent);
            allGlass[countGlass].JumpOutOfTray(endPos, () =>
            {
                OnCompleted?.Invoke(allGlass[countGlass]);
            });
        }
        internal void PouringWater()
        {
            allGlass[countGlass].OnPouringWater();
        }
    }
}
