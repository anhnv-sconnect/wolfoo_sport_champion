using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WFSport.Base.Constant;

namespace WFSport.Gameplay.SnowballMode
{
    public class Snow : MonoBehaviour
    {
        [SerializeField] private float completeValue = 0.95f;
        private bool isCompleted;
        private Rigidbody2D rigidBd;
        private ScratchCardManager myCard;
        private TweenerCore<Color, Color, ColorOptions> _tween;

        private void Start()
        {
            rigidBd = GetComponent<Rigidbody2D>();
            myCard = GetComponent<ScratchCardManager>();
            myCard.Progress.OnProgress += OnScratching;
        }
        private void OnDestroy()
        {
            _tween?.Kill();
            myCard.Progress.OnProgress -= OnScratching;
        }

        private void OnScratching(float progress)
        {
            if (isCompleted) return;
            if(progress >= completeValue)
            {
                isCompleted = true;
                rigidBd.simulated = false;
                var card = myCard.SpriteCard.GetComponent<SpriteRenderer>();
                _tween?.Kill();
                _tween = card.DOFade(0, 1).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!isCompleted && collision.CompareTag(TAG.PLAYER))
            {
                var direction = collision.bounds.center - transform.position;
                var range = direction.normalized * collision.bounds.extents.x;
                var pos = Camera.main.WorldToScreenPoint(collision.bounds.center);
                myCard.Card.ScratchHole(pos, collision.bounds.extents.x * collision.transform.localScale.x);
            }
        }
        internal void EnableScratch()
        {
            myCard.Card.InputEnabled = true;
        }
        internal void DisableScratch()
        {
            myCard.Card.InputEnabled = false;
        }
    }
}
