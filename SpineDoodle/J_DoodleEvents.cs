#if JSPINE_SUPPORT
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;
using Event = Spine.Event;

namespace JReact.JSpineSupport
{
    public sealed class J_DoodleEvents : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_ActorDoodle _doodle;
        private Spine.AnimationState AnimationState => _doodle.AnimationState;

        private readonly Dictionary<EventData, List<Action<TrackEntry, Spine.Event>>> _listeners =
            new Dictionary<EventData, List<Action<TrackEntry, Spine.Event>>>();

        public void AddListener(EventDataReferenceAsset spineEvent, Action<TrackEntry, Spine.Event> action,
                                bool                    allowDuplicates = false)
        {
            if (!_listeners.ContainsKey(spineEvent.EventData))
            {
                _listeners.Add(spineEvent.EventData, new List<Action<TrackEntry, Spine.Event>>());
            }

            List<Action<TrackEntry, Event>> list = _listeners[spineEvent.EventData];
            if (allowDuplicates || !list.Contains(action)) { list.Add(action); }
        }

        public void RemoveListener(EventDataReferenceAsset spineEvent, Action<TrackEntry, Spine.Event> action)
        {
            if (!_listeners.ContainsKey(spineEvent.EventData)) { return; }

            List<Action<TrackEntry, Event>> list = _listeners[spineEvent.EventData];
            if (list.Contains(action)) { list.Remove(action); }
        }

        private void OnSpineEvent(TrackEntry trackEntry, Spine.Event spineEvent)
        {
            if (!_listeners.ContainsKey(spineEvent.Data)) { return; }

            var actions = _listeners[spineEvent.Data];
            for (int i = 0; i < actions.Count; i++) { actions[i].Invoke(trackEntry, spineEvent); }
        }

        private void OnEnable()
        {
            AnimationState.Event -= OnSpineEvent;
            AnimationState.Event += OnSpineEvent;
        }

        private void OnDisable() { AnimationState.Event -= OnSpineEvent; }

        private void OnDestroy() { _listeners.Clear(); }
    }
}
#endif
