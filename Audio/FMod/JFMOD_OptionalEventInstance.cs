#if FJMOD_HELPER
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Assertions;
using GUID = FMOD.GUID;
using PLAYBACK_STATE = FMOD.Studio.PLAYBACK_STATE;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace JReact.J_Audio.FMod
{
    public struct JFMOD_OptionalEventInstance
    {
        // --------------- INSTANCE --------------- //
        public static readonly JFMOD_OptionalEventInstance Empty = default;
        private readonly EventReference eventRef;

        private EventInstance instance;

        public readonly bool HasEvent => !eventRef.IsNull;
        public readonly bool IsAlive => instance.isValid();

        public static JFMOD_OptionalEventInstance Create(EventReference eventRef)
        {
            var instance = new JFMOD_OptionalEventInstance(eventRef);
            if (instance.IsAlive) { _instances.Add(instance); }

            return instance;
        }

        private JFMOD_OptionalEventInstance(EventReference eventRef)
        {
            this.eventRef = eventRef;
            instance      = !eventRef.IsNull ? RuntimeManager.CreateInstance(eventRef) : default;
        }

        public readonly EventDescription GetDescription()
        {
            if (IsEmpty()) { return default; }

            instance.getDescription(out EventDescription description);
            return description;
        }

        public readonly PLAYBACK_STATE GetPlaybackState()
        {
            if (IsEmpty()) { return PLAYBACK_STATE.STOPPED; }

            instance.getPlaybackState(out PLAYBACK_STATE state);
            return state;
        }

        public JFMOD_OptionalEventInstance Play()
        {
            if (IsEmpty()) { return this; }

            instance.getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.PLAYING ||
                state == PLAYBACK_STATE.STARTING) { return this; }

            instance.start();
            return this;
        }

        public void Stop(STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
        {
            if (IsEmpty()) { return; }

            instance.stop(stopMode);
        }

        public readonly JFMOD_OptionalEventInstance SetParameter(string parameterName, float value)
        {
            if (IsEmpty()) { return this; }

            instance.setParameterByName(parameterName, value);
            return this;
        }

        public readonly JFMOD_OptionalEventInstance PlaceAtPosition(Vector3 position)
        {
            if (IsEmpty()) { return this; }

            var attributes = new FMOD.ATTRIBUTES_3D
            {
                position = position.ToFMODVector(), forward = Vector3.forward.ToFMODVector(), up = Vector3.up.ToFMODVector()
            };

            instance.set3DAttributes(attributes);
            return this;
        }

        public readonly JFMOD_OptionalEventInstance PlaceOnGameObject(GameObject gameObject)
        {
            if (IsEmpty()) { return this; }

            ATTRIBUTES_3D attributes = gameObject.To3DAttributes();
            instance.set3DAttributes(attributes);
            return this;
        }

        public void Release()
        {
            if (!IsAlive) { return; }

            if (_instances.Contains(this)) { _instances.Remove(this); }
            instance.release();
            instance.clearHandle();
        }

        public void ReleaseUniqueInstance(bool releaseAlsoDefault = true)
        {
            EventDescription description = GetDescription();
            description.getID(out GUID guid);

            bool isDefault = _defaultInstances.TryGetValue(guid, out JFMOD_OptionalEventInstance defaultInstance);
            Assert.IsTrue(isDefault, $"Event {eventRef} is not a default instance");

            _defaultInstances.Remove(guid);
            Stop(STOP_MODE.IMMEDIATE);
            Release();
        }

        private readonly bool IsEmpty()
        {
            if (!HasEvent) { return true; }

            Assert.IsTrue(IsAlive, $"Event {eventRef} is not valid");
            return false;
        }

        // --------------- STATIC --------------- //
        private static HashSet<JFMOD_OptionalEventInstance> _instances = new HashSet<JFMOD_OptionalEventInstance>();
        private static Dictionary<GUID, JFMOD_OptionalEventInstance> _defaultInstances =
            new Dictionary<GUID, JFMOD_OptionalEventInstance>();

        public static JFMOD_OptionalEventInstance GetUniqueInstance(EventReference eventRef)
        {
            GUID guid = eventRef.Guid;
            if (_defaultInstances.TryGetValue(guid, out JFMOD_OptionalEventInstance instance)) { return instance; }

            JFMOD_OptionalEventInstance @event = Create(eventRef);
            _defaultInstances[guid] = @event;
            return @event;
        }

        public static void ReleaseEventInstances()
        {
            foreach (JFMOD_OptionalEventInstance @event in _instances)
            {
                @event.Stop(STOP_MODE.IMMEDIATE);
                @event.Release();
            }

            _instances.Clear();
            _defaultInstances.Clear();
        }

        public override string ToString() => $"Instance of: {eventRef.Guid}-{eventRef}.";
    }
}
#endif
