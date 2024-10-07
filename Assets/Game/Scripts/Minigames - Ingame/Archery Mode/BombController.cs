using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.ArcheryMode
{
    public class BombController : MonoBehaviour
    {
        [SerializeField] private Bomb bombPb;
        [SerializeField] private SpriteRenderer circleSpriteRender;

        private Bomb[] bombs;
        private GameplayConfig config;
        private Sequence _anim;
        private int bombIdx;
        private int maxBomb;

        private void OnDestroy()
        {
            _anim?.Kill();
        }
        internal Bomb CreateBomb(Marker holder)
        {
            var bomb = bombs[bombIdx];

            if(bomb == null)
            {
                bomb = Instantiate(bombPb, holder.transform);
                bombs[bombIdx] = bomb;
                bomb.transform.localPosition = Vector3.up * 2;
            }

            bomb.Setup(config.bombUsedTime, holder);
            bomb.Show();

            bombIdx++;
            bombIdx = bombIdx >= maxBomb ? 0 : bombIdx;

            return bomb;
        }

        internal void Setup(GameplayConfig config, int maxBomb)
        {
            this.config = config;
            this.maxBomb = maxBomb;
            bombs = new Bomb[maxBomb];
        }

        internal bool IsArrowShooted(Arrow arrow)
        {
            foreach (var bomb in bombs)
            {
                if (!arrow.IsAttached && bomb.IsInside(arrow.transform.position))
                {
                    arrow.IsAttached = true;
                    bomb.OnHitCorrect(arrow.transform.position);
                    PlayAnimExploring(arrow.transform.position);
                    return true;
                }
            }

            return false;
        }

        private void PlayAnimExploring(Vector3 pos)
        {
            _anim?.Kill();

            circleSpriteRender.transform.position = pos;
            circleSpriteRender.transform.localScale = Vector3.zero;
            circleSpriteRender.DOFade(0.5f, 0);
            var totalAnimTime = config.bombUsedTime - 0.2f;

            _anim = DOTween.Sequence()
                .Append(circleSpriteRender.transform.DOScale(Vector3.one * 100, totalAnimTime / 2).SetEase(Ease.Flash))
                .Join(circleSpriteRender.DOFade(1, totalAnimTime / 2).SetEase(Ease.Flash))
                .AppendInterval(0.2f)
                .Append(circleSpriteRender.transform.DOScale(Vector3.zero, totalAnimTime / 2).SetEase(Ease.Linear))
                .Join(circleSpriteRender.DOFade(0, totalAnimTime / 2).SetEase(Ease.Linear));

        }
    }
}
