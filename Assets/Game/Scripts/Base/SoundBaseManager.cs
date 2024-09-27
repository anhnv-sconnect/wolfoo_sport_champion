using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Base
{
    public class SoundBaseManager : SoundBase<SoundBaseManager>
    {
        [SerializeField] private AudioSource soundAus;
        [SerializeField] private AudioSource musicAus;
        [SerializeField] private AudioClip clickClip;
        private AudioSource click;

        public AudioSource SoundAus { get => soundAus; }

        public void PlayMusic(AudioClip clip)
        {
            musicAus.clip = clip;
            musicAus.volume = MusicVolume;
            musicAus.Play();
        }
        public void PlaySoundClick()
        {
            if (click == null) { click = CreateSound(clickClip, transform); }
            if (click != null) click.Play();
        }
    }
}
