using System;
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

        private void Start()
        {
            var canvas = GetComponentInChildren<Canvas>();
            if(canvas != null) canvas.worldCamera = Camera.main;

            EventManager.OnBallShootingTarget += OnBallShootingTarget;
        }
        private void OnDestroy()
        {
            EventManager.OnBallShootingTarget -= OnBallShootingTarget;
        }

        private void OnBallShootingTarget(Ball ball)
        {
            Play(ball.TargetBasket.Score, ball.TargetBasket.HolePos);
        }

        internal void CreateAnim(int total)
        {
            scoreAnims = new Animator[total];
            scoreTxts = new TMP_Text[total];
        }
        internal void Play(int score, Vector3 pos)
        {
            var scoreAnim = scoreAnims[count];
            if(scoreAnim == null)
            {
                scoreAnim = Instantiate(scoreAnimPb, transform);
                scoreAnims[count] = scoreAnim;
                scoreTxts[count] = scoreAnim.GetComponentInChildren<TMP_Text>();
            }

            scoreTxts[count].text = score >= 0 ? $"+{score}" : $"{score}";
            scoreAnim.transform.position = pos;
            scoreAnim.Play(playName, 0, 0);

            count++;
            if (count >= scoreAnims.Length) count = 0;
        }
    }
}
