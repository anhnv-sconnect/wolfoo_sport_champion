using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.RelayMode
{
    public class SoundManager : SoundGameplayManager
    {
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip rollingBallClip;
        [SerializeField] private AudioClip boostClip;
        [SerializeField] private AudioClip rideaBikeClip;

        private AudioSource jumpingAus;
        private AudioSource boostingAus;
        private AudioSource rollingBallAus;
        private AudioSource ridingaBikeAus;

        public void PlayThrowCorrect()
        {
            if (jumpingAus == null)
            {
                jumpingAus = CreateSound(jumpClip, transform);
            }
            if (jumpingAus != null)
            {
                jumpingAus.Play();
            }
        }
        public void PlaySnowingball()
        {
            if (boostingAus == null)
            {
                boostingAus = CreateSound(boostClip, transform);
            }
            if (boostingAus != null)
            {
                boostingAus.Play();
            }
        }
        public void PlaySliding()
        {
            if (rollingBallAus == null)
            {
                rollingBallAus = CreateSound(rollingBallClip, transform);
            }
            if (rollingBallAus != null)
            {
                rollingBallAus.Play();
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
                if (boostingAus != null) { boostingAus.volume = obj.volume; }
                if (jumpingAus != null) { jumpingAus.volume = obj.volume; }
                if (rollingBallAus != null) { rollingBallAus.volume = obj.volume; }
                if (ridingaBikeAus != null) { ridingaBikeAus.volume = obj.volume; }
            }

            if(obj.isMusic)
            {

            }
        }
    }
}
