using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WFSport.Gameplay
{
    public abstract class SoundGameplayManager : SoundBase<SoundGameplayManager>
    {
        protected abstract void OnChangeVolume(EventKey.OnChangeVolume obj);

        protected virtual void Start()
        {
            EventDispatcher.Instance.RegisterListener<EventKey.OnChangeVolume>(OnChangeVolume);
        }

    }
}
