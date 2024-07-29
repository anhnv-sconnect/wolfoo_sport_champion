

using Spine;
using System;

namespace Helper {
    public static class AnimationHelper {
        public static void PlayAnimation(this AnimationState state, string animName, bool isLoop, int track = 0) {
            if(state == null || track < 0 || string.IsNullOrEmpty(animName)) {
                return;
            }
            state.SetAnimation(track, animName, isLoop);
        }

        public static void AddEvent(this AnimationState state, string eventName, Action onEvent) {
            if(state == null || string.IsNullOrEmpty(eventName)) {
                return;
            }
            state.Event += (TrackEntry trackEntry, Event e) => {
                if(e.Data.Name.Equals(eventName)) {
                    onEvent?.Invoke();
                }
            };
        }
       
    }
}
