using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.LatinDanceMode
{
    public class SpotlightGroup : MonoBehaviour
    {
        [SerializeField] Spotlight[] spotlights;
        [SerializeField] SpriteRenderer blackBg;
        [SerializeField] GameObject globe;
        private Sequence _sequence;

        private void OnDestroy()
        {
            _sequence?.Kill();
        }
        internal void Play(Transform target)
        {
            gameObject.SetActive(true);

            blackBg.gameObject.SetActive(false);
            globe.gameObject.SetActive(false);
            for (int i = 0; i < spotlights.Length; i++)
            {
                if (!spotlights[i].IsPlaying)
                {
                    spotlights[i].Setup(target);
                    spotlights[i].Play();
                    break;
                }
            }
        }
        internal void Stop(Transform target)
        {
            if(globe.transform.position.y == 10)
            {
                _sequence?.Kill();
                _sequence = DOTween.Sequence()
                     .Append(globe.transform.DOMoveY(10, 0.25f))
                     .AppendCallback(() =>
                     {
                         for (int i = 0; i < spotlights.Length; i++)
                         {
                             spotlights[i].Stop(target);
                         }
                         globe.gameObject.SetActive(false);
                         blackBg.gameObject.SetActive(false);
                     });
            }
            else
            {
                for (int i = 0; i < spotlights.Length; i++)
                {
                    spotlights[i].Stop(target);
                }
                globe.gameObject.SetActive(false);
                blackBg.gameObject.SetActive(false);
            }
        }
        internal void PlayFocus(Transform target)
        {
            blackBg.gameObject.SetActive(true);
            globe.gameObject.SetActive(true);
            foreach (var spotlight in spotlights)
            {
                spotlight.Setup(target);
                spotlight.Play();
            }
            _sequence?.Kill();
            _sequence = DOTween.Sequence()
                 .Append(globe.transform.DOMoveY(6, 0.25f));
        }
    }
}
