using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.ArcheryMode
{
    public class Bot : Player
    {
        public override void Play()
        {
            base.Play();

            PlayAutoShooting();
        }
        public override void OnTouching(Vector3 position)
        {
            //    base.OnTouching(position);
        }
    }
}
