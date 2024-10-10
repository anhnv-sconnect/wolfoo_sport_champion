using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.CatchMoreToysMode
{
    public class SoundManager : SoundGameplayManager
    {
        [SerializeField] private AudioClip catchToyClip;
        [SerializeField] private AudioClip throwToyClip;
        [SerializeField] private AudioClip movingClip;
        [SerializeField] private AudioClip rideaBikeClip;

        private AudioSource catchingToyAus;
        private AudioSource movingAus;
        private AudioSource throwingToyAus;
        private AudioSource ridingaBikeAus;

        public void PlayThrowCorrect()
        {
            if (catchingToyAus == null)
            {
                catchingToyAus = CreateSound(catchToyClip, transform);
            }
            if (catchingToyAus != null)
            {
                catchingToyAus.Play();
            }
        }
        public void PlaySnowingball()
        {
            if (movingAus == null)
            {
                movingAus = CreateSound(movingClip, transform);
            }
            if (movingAus != null)
            {
                movingAus.Play();
            }
        }
        public void PlaySliding()
        {
            if (throwingToyAus == null)
            {
                throwingToyAus = CreateSound(throwToyClip, transform);
            }
            if (throwingToyAus != null)
            {
                throwingToyAus.Play();
            }
        }
        public void PlayRidingaBike()
        {
            if (ridingaBikeAus == null)
            {
                ridingaBikeAus = CreateSound(rideaBikeClip, transform);
            }
            if (ridingaBikeAus != null)
            {
                ridingaBikeAus.Play();
            }
        }

        protected override void OnChangeVolume(EventKey.OnChangeVolume obj)
        {
            if(obj.isSound)
            {
                if (movingAus != null) { movingAus.volume = obj.volume; }
                if (catchingToyAus != null) { catchingToyAus.volume = obj.volume; }
                if (throwingToyAus != null) { throwingToyAus.volume = obj.volume; }
                if (ridingaBikeAus != null) { ridingaBikeAus.volume = obj.volume; }
            }

            if(obj.isMusic)
            {

            }
        }
    }
}
