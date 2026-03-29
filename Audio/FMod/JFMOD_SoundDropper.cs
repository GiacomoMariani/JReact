#if FJMOD_HELPER
using System.Collections.Generic;
using FMOD.Studio;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.J_Audio.FMod
{
    public sealed class JFMOD_SoundDropper : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private List<JFMOD_OptionalEventInstance> _sounds = new List<JFMOD_OptionalEventInstance>();

        private void LateUpdate()
        {
            for (int i = _sounds.Count - 1; i >= 0; i--)
            {
                JFMOD_OptionalEventInstance sound = _sounds[i];
                PLAYBACK_STATE              state = sound.GetPlaybackState();

                if (state != PLAYBACK_STATE.STOPPED) { continue; }

                _sounds.RemoveAt(i);
                ReleaseSound(sound);
            }
        }

        public void StopAndDrop(JFMOD_OptionalEventInstance soundInstance)
        {
            if (!soundInstance.HasEvent) { return; }

            PLAYBACK_STATE state = soundInstance.GetPlaybackState();

            if (state != PLAYBACK_STATE.STOPPING &&
                state != PLAYBACK_STATE.STOPPED) { soundInstance.Stop(STOP_MODE.ALLOWFADEOUT); }

            _sounds.Add(soundInstance);
        }

        public void StopWhenComplete(JFMOD_OptionalEventInstance soundInstance)
        {
            if (!soundInstance.HasEvent) { return; }
            _sounds.Add(soundInstance);
        }

        private void ReleaseSound(JFMOD_OptionalEventInstance sound) { sound.Release(); }
    }
}
#endif
