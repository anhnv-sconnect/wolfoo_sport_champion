using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.FurnitureMode
{
    public class Chair : DecorItem
    {
        private Sequence animTwinlink;
        protected override void OnDestroy()
        {
            base.OnDestroy();
            animTwinlink?.Kill();
        }
        public override void Replace(Sprite icon)
        {
            PlayAnimReplace(icon);
        }
        public void StopAnimTwinlink()
        {
            animTwinlink?.Kill();
            spriteRenderer.DOFade(0.5f, 0);
        }
        public void PlayAnimTwinlink()
        {
            animTwinlink = DOTween.Sequence()
                .Append(spriteRenderer.DOFade(1, 0.5f));
            animTwinlink.SetLoops(-1, LoopType.Yoyo);
        }
    }
}
