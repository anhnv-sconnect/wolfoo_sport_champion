using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Home
{
    public class SoundManager : SoundBase<SoundManager>
    {
        [SerializeField] private AudioClip clipTest1;

        private AudioSource sound1;

        public void PlayTest1()
        {
            if (sound1 == null) { sound1 = CreateSound(clipTest1, transform); }
            sound1.Play();
        }
    }
}
