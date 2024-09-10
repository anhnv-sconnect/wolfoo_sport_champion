using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    /// <summary>
    /// ScoreManager is manager anim when Player get score
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private Animator scoreAnimPb;
        [SerializeField] private string playName;

        private Animator[] scoreAnims;
        private int count;

        internal void CreateAnim(int total)
        {
            scoreAnims = new Animator[total];
            for (int i = 0; i < total; i++)
            {
                var scoreAnim = Instantiate(scoreAnimPb, transform);
                scoreAnims[i] = scoreAnim;
            }
        }
        internal void Play(Vector3 pos)
        {
            var score = scoreAnims[count];
            score.transform.position = pos;
            score.Play(playName, 0, 0);
            count++;
            if (count >= scoreAnims.Length) count = 0;
        }
    }
}
