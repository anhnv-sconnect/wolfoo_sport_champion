using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Load
{
    public class SoundManager : SoundBase<SoundManager>
    {
        [SerializeField] AudioClip clipTest1;
        private AudioSource test1;

        public void PlayTest1()
        {
            if (test1 == null) test1 = CreateSound(clipTest1, transform);
            test1.Play();
        }
    }
}
