#if FJMOD_HELPER
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace JReact.J_Audio.FMod
{
    public static class JFMODExtensions
    {
        private static HashSet<EventInstance> _instances = new HashSet<EventInstance>();
        private static HashSet<StudioEventEmitter> _emitters = new HashSet<StudioEventEmitter>();

        private static Dictionary<GUID, EventInstance> _defaultInstances = new Dictionary<GUID, EventInstance>();

        // --------------- EMITTERS --------------- //
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
        public static void PlayOneShot(this EventReference eventRef, Vector3 pos) { RuntimeManager.PlayOneShot(eventRef, pos); }

        public static void PlayOneShotAttached(this EventReference eventRef, GameObject obj)
        {
            RuntimeManager.PlayOneShotAttached(eventRef, obj);
        }

        // --------------- EVENT INSTANCES - LIFE CYCLE --------------- //
        public static EventInstance GetDefaultInstance(this EventReference eventRef)
        {
            GUID guid = eventRef.Guid;
            if (_defaultInstances.TryGetValue(guid, out EventInstance instance)) { return instance; }

            EventInstance @event = RuntimeManager.CreateInstance(eventRef);
            _instances.Add(@event);
            _defaultInstances[guid] = @event;
            return @event;
        }

        public static EventInstance CreateInstance(this EventReference eventRef)
        {
            EventInstance @event = RuntimeManager.CreateInstance(eventRef);
            _instances.Add(@event);
            return @event;
        }

        public static void ReleaseThis(this EventInstance @event, bool releaseAlsoDefault = true)
        {
            @event.getDescription(out EventDescription description);
            description.getID(out GUID guid);

            var isDefault = _defaultInstances.TryGetValue(guid, out EventInstance defaultInstance);
            if (!releaseAlsoDefault && isDefault) { return; }

            if (_instances.Contains(@event)) { _instances.Remove(@event); }

            if (isDefault) { _defaultInstances.Remove(guid); } // (this is the default instance)

            Stop(@event, STOP_MODE.IMMEDIATE);
            @event.release();
        }

        public static void ReleaseEventInstances()
        {
            foreach (EventInstance @event in _instances)
            {
                Stop(@event, STOP_MODE.IMMEDIATE);
                @event.release();
            }

            _instances.Clear();
            _defaultInstances.Clear();
        }


        // --------------- EVENT INSTANCE - COMMANDS --------------- //
        public static EventInstance SetParameter(this EventInstance @event, string parameterId, float value)
        {
            @event.setParameterByName(parameterId, value);
            return @event;
        }
        
        public static void PlaceAtPosition(this EventInstance @event, Vector3 position)
        {
            var attributes = new FMOD.ATTRIBUTES_3D
            {
                position = position.ToFMODVector(),
                forward = Vector3.forward.ToFMODVector(),
                up = Vector3.up.ToFMODVector()
            };

            @event.set3DAttributes(attributes);
        }
        
        public static void PlaceOnGameObject(this EventInstance @event, GameObject gameObject)
        {
            ATTRIBUTES_3D attributes = gameObject.To3DAttributes();
            @event.set3DAttributes(attributes);
        }
        
        public static void Play(this EventInstance @event)
        {
            @event.getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.PLAYING ||
                state == PLAYBACK_STATE.STARTING) { return; }

            @event.start();
        }
        
        public static void PlayAtPosition(this EventInstance @event, Vector3 position)
        {
            @event.getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.PLAYING ||
                state == PLAYBACK_STATE.STARTING) { return; }
            @event.PlaceAtPosition(position);
            @event.start();
        }



        public static void PlayOnGameObject(this EventInstance @event, GameObject gameObject)
        {
            @event.getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.PLAYING ||
                state == PLAYBACK_STATE.STARTING) { return; }
            @event.PlaceOnGameObject(gameObject);
            @event.start();
        }

        public static void Stop(this EventInstance @event, STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT) { @event.stop(stopMode); }
    }
}
#endif
