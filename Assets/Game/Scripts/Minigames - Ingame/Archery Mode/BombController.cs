using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.ArcheryMode
{
    public class BombController : MonoBehaviour
    {
        [SerializeField] private Bomb[] bombs;
        [SerializeField] private SpriteRenderer circleSpriteRender;

        private GameplayConfig config;
        private Sequence _anim;

        private void OnDestroy()
        {
            _anim?.Kill();
        }
        internal void CreateBomb()
        {
            var idx = UnityEngine.Random.Range(0, bombs.Length);
            var bomb = bombs[idx];

            if(bomb != null && !bomb.IsShowing)
            {
                bomb.Setup(config.bombUsedTime);
                bomb.Show();
            }
            else
            {
                CreateBomb();
            }
        }

        internal void Setup(GameplayConfig config)
        {
            this.config = config;
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
