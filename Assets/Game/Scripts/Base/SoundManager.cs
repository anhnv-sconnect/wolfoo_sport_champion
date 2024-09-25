using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Base
{
    public class SoundManager : SingletonBind<SoundManager>
    {
        [SerializeField] private AudioSource musicAus;
        [SerializeField] private AudioSource soundAusPb;

        private float soundVolume;
        private float musicVolume;

        public float SoundVolume { get => DataManager.instance.localSaveloadData.playerMe.soundVolume; set => soundVolume = value; }
        public float MusicVolume { get => DataManager.instance.localSaveloadData.playerMe.musicVolume; set => musicVolume = value; }

        private void Start()
        {
        }
        public AudioSource CreateSound(AudioClip clip, Transform parent)
        {
            var sound = Instantiate(soundAusPb, parent);
            sound.clip = clip;
            sound.volume = SoundVolume;
            return sound;
        }

        public void PlayMusic(AudioClip clip)
        {
            musicAus.clip = clip;
        }
    }
}
