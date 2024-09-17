using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay.BasketballMode
{
    public class Bot : Player
    {
        public override void Play()
        {
            base.Play();
            PlayAutoThrowing();
        }
        internal override void Setup(GameplayConfig config, LevelConfig.Mode mode, GameplayManager gameManager)
        {
            base.Setup(config, mode, gameManager);
            reloadTime = level.botReloadTime;
        }
        internal override void Create(CharacterWorldAnimation pb)
        {
            base.Create(pb);
            characterAnim.transform.rotation = Quaternion.Euler(Vector3.up * 180);
        }
        public override void Pause(bool isSystem)
        {
            base.Pause(isSystem);
            if(isSystem)
            {
                StopAutoThrowing();
            }
        }
        public override void Lose()
        {
            base.Lose();
            StopAutoThrowing();
        }
    }
}
