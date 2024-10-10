using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.LatinDanceMode
{
    public class SoundManager : SoundGameplayManager
    {
        [SerializeField] private AudioClip claimSoundClip;
        [SerializeField] private AudioClip introduceClip;
        [SerializeField] private AudioClip slideClip;

        private AudioSource claimSoundAus;
        private AudioSource introduceAus;
        private AudioSource slidingAus;

        public void PlayThrowCorrect()
        {
            if (claimSoundAus == null)
            {
                claimSoundAus = CreateSound(claimSoundClip, transform);
            }
            if (claimSoundAus != null)
            {
                claimSoundAus.Play();
            }
        }
        public void PlaySnowingball()
        {
            if (introduceAus == null)
            {
                introduceAus = CreateSound(introduceClip, transform);
            }
            if (introduceAus != null)
            {
                introduceAus.Play();
            }
        }
        public void PlaySliding()
        {
            if (slidingAus == null)
            {
                slidingAus = CreateSound(slideClip, transform);
            }
            if (slidingAus != null)
            {
                slidingAus.Play();
            }
        }

        protected override void OnChangeVolume(EventKey.OnChangeVolume obj)
        {
            if(obj.isSound)
            {
                if (introduceAus != null) { introduceAus.volume = obj.volume; }
                if (claimSoundAus != null) { claimSoundAus.volume = obj.volume; }
                if (slidingAus != null) { slidingAus.volume = obj.volume; }
            }

            if(obj.isMusic)
            {

            }
        }
    }
}
