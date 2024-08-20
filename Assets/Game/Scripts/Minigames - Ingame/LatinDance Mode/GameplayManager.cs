using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.LatinDanceMode
{
    public class GameplayManager : MonoBehaviour, IMinigame
    {
        [SerializeField] BonusItem itemPb;
        [SerializeField] BoxCollider2D limited;
        [SerializeField] Transform itemHolder;
        [SerializeField] float radius;
        [SerializeField] float padding;

        private IMinigame.Data myData;
        public IMinigame.Data ExternalData { get => myData; set => myData = value; }

        GameObject[][] matrix;

        [NaughtyAttributes.Button]
        private void CreateRandomItems()
        {
            var size = new Vector2((int)(limited.size.x / (radius + padding / 2)), 
                (int)(limited.size.y / (radius + padding / 2)));

        }
        [NaughtyAttributes.Button]
        private void Clear()
        {
            foreach (var item in itemHolder.GetComponentsInChildren<BonusItem>())
            {
                DestroyImmediate(item.gameObject);
            }
        }
        [NaughtyAttributes.Button]
        private void CreateMatrix()
        {
            var dimension = (radius * 2 + padding / 2);
            var rangeX = (int) (limited.size.x / dimension);
            var rangeY = (int) (limited.size.y / dimension);

            var size = new Vector2((rangeX), (rangeY));

            matrix = new GameObject[(int)size.y][];
            for (int c = 0; c < size.y; c++)
            {
                if (matrix[c] == null) matrix[c] = new GameObject[(int)size.x];
                for (int r = 0; r < size.x; r++)
                {

                    var pos = new Vector2(r / size.x + rangeX, c / size.y + rangeY);
                    matrix[c][r] = Instantiate(itemPb, itemHolder).gameObject;
                    matrix[c][r].transform.position = pos;
                }
            }
        }

        private void Start()
        {
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
