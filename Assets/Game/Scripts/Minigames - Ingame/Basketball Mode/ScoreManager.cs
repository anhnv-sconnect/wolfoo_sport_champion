using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        private TMP_Text[] scoreTxts;
        private int count;

        internal void CreateAnim(int total)
        {
            scoreAnims = new Animator[total];
            scoreTxts = new TMP_Text[total];
            for (int i = 0; i < total; i++)
            {
                var scoreAnim = Instantiate(scoreAnimPb, transform);
                scoreAnims[i] = scoreAnim;
                scoreTxts[i] = scoreAnim.GetComponentInChildren<TMP_Text>();
            }
        }
        internal void Setup(int score)
        {
            scoreTxts[count].text = score >= 0 ? $"+{score}" : $"{score}";
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
