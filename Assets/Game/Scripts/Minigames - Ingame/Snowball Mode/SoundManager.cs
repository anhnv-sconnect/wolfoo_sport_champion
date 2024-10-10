using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.SnowballMode
{
    public class SoundManager : SoundGameplayManager
    {
        [SerializeField] private AudioClip throwCorrect;
        [SerializeField] private AudioClip snowingBall;
        private AudioSource correctAus;
        private AudioSource snowingAus;

        public void PlayThrowCorrect()
        {
            if (correctAus == null)
            {
                correctAus = CreateSound(throwCorrect, transform);
            }
            if (correctAus != null)
            {
                correctAus.Play();
            }
        }
        public void PlaySnowingball()
        {
            if (snowingAus == null)
            {
                snowingAus = CreateSound(snowingBall, transform);
            }
            if (snowingAus != null)
            {
                snowingAus.Play();
            }
        }

        protected override void OnChangeVolume(EventKey.OnChangeVolume obj)
        {
            if(obj.isSound)
            {
                if (snowingAus != null) { snowingAus.volume = obj.volume; }
                if (correctAus != null) { correctAus.volume = obj.volume; }
            }

            if(obj.isMusic)
            {

            }
        }
    }
}
