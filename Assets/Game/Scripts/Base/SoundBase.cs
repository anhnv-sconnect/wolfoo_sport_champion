using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFSport.Base;

namespace WFSport
{
    public class SoundBase<T> : SingletonBind<T> where T : Component
    {
        public float SoundVolume { get => Base.GameController.Instance.PlayerMe.soundVolume; }
        public float MusicVolume { get => Base.GameController.Instance.PlayerMe.musicVolume; }

        protected AudioSource CreateSound(AudioClip clip, Transform parent)
        {
            if(clip == null) return null;

            var pb = SoundBaseManager.Instance.SoundAus;
            var sound = Instantiate(pb, transform);
            sound.name = "Aus - Sound - " + clip.name;
            sound.clip = clip;
            sound.volume = SoundVolume;
            return sound;
        }
    }
}
