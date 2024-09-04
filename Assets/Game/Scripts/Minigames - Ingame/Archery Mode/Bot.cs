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

            isAutoShooting = true;
            PlayAutoShooting();
        }
        public override void Pause(bool isSystem)
        {
            base.Pause(isSystem);

            isAutoShooting = false;
            StopAutoShooting();
        }
        public override void OnTouching(Vector3 position)
        {
            //    base.OnTouching(position);
        }
    }
}
