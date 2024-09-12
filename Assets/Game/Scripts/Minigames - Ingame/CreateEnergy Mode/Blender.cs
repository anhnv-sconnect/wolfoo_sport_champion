using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CreateEnergyMode
{
    public class Blender : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string pouringAnimName;
        [SerializeField] private string grindAnimName;
        [SerializeField] private Transform fruitHolder;
        private Sequence animShaking;
        private Vector3 startPos;
        private int countPos = -2;

        public Vector3 FruitArea
        { 
            get
            {
                countPos++;
                return fruitHolder.position + Vector3.right * countPos * 0.5f;
            }
        }

        private void Start()
        {
            EventManager.OnFruitJumpIn += OnFruitJumpIn;
            startPos = transform.position;
        }
        private void OnDestroy()
        {
            EventManager.OnFruitJumpIn -= OnFruitJumpIn;
            animShaking?.Kill();
        }


        private void OnFruitJumpIn(Fruit fruit)
        {
            animShaking?.Kill();
            transform.position = startPos;
            animShaking = DOTween.Sequence()
                .Append(transform.DOPunchPosition(Vector3.one * 0.1f, 0.5f, 1));
        }
    }
}
