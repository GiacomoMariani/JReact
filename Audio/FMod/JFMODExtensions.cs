#if FJMOD_HELPER
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using GUID = FMOD.GUID;

namespace JReact.J_Audio.FMod
{
    public static class JFMODExtensions
    {
        private static HashSet<StudioEventEmitter> _emitters = new HashSet<StudioEventEmitter>();
        
        // --------------- EMITTERS --------------- //
        public static void PlayOnEmitterSafe(this StudioEventEmitter emitter, EventReference eventRef)
        {
            if (eventRef.IsNull) { return; }

            PlayOnEmitter(emitter, eventRef);
        }

        public static void PlayOnEmitter(this StudioEventEmitter emitter, EventReference eventRef)
        {
            _emitters.Add(emitter);
            emitter.EventReference = eventRef;
            emitter.Play();
        }

        public static void StopEmitter(this StudioEventEmitter emitter)
        {
            if (_emitters.Contains(emitter)) { _emitters.Remove(emitter); }

            _emitters.Remove(emitter);
            emitter.Stop();
        }

        public static void StopAllEmitters()
        {
            foreach (StudioEventEmitter emitter in _emitters) { emitter.Stop(); }
        }

        // --------------- ONE SHOT PLAY --------------- //
        public static void PlayOneShotSafe(this EventReference eventRef, Vector3 pos)
        {
            if (eventRef.IsNull) { return; }

            PlayOneShot(eventRef, pos);
        }

        public static void PlayOneShot(this EventReference eventRef, Vector3 pos) { RuntimeManager.PlayOneShot(eventRef, pos); }

        public static void PlayOneShotAttachedSafe(this EventReference eventRef, GameObject obj)
        {
            if (eventRef.IsNull) { return; }

            RuntimeManager.PlayOneShotAttached(eventRef, obj);
        }

        public static void PlayOneShotAttached(this EventReference eventRef, GameObject obj)
        {
            RuntimeManager.PlayOneShotAttached(eventRef, obj);
        }
    }
}
#endif
