using System;
using System.Collections.Generic;
using UnityEngine;

namespace JReact.Events
{
    public class JEvent : CustomYieldInstruction
    {
        public override bool keepWaiting => !IsDone;

        public readonly bool IsPermanent;
        public bool IsDone { get; protected set; }
        protected List<Action> EventsToTrigger = new List<Action>();

        public JEvent(bool isPermanent) => IsPermanent = isPermanent;

        public bool Contains(Action action) => EventsToTrigger.Contains(action);
        
        public JEvent AndThen(Action action)
        {
            EventsToTrigger.Add(action);
            return this;
        }

        public JEvent ResetThis()
        {
            EventsToTrigger.Clear();
            IsDone = false;
            return this;
        }

        public void Process()
        {
            IsDone = true;
            for (int i = 0; i < EventsToTrigger.Count; i++) { EventsToTrigger[i]?.Invoke(); }
        }

        public override string ToString() => $"{GetType().Name} (Done:{IsDone} - Permanent: {IsPermanent}";
    }
    
    public class JEvent<T> : CustomYieldInstruction
    {
        public override bool keepWaiting => !IsDone;

        public readonly bool IsPermanent;
        public bool IsDone { get; protected set; }
        protected List<Action<T>> EventsToTrigger = new List<Action<T>>();

        public JEvent(bool isPermanent) => IsPermanent = isPermanent;

        public bool Contains(Action<T> action) => EventsToTrigger.Contains(action);
        
        public JEvent<T> AndThen(Action<T> action)
        {
            EventsToTrigger.Add(action);
            return this;
        }

        public JEvent<T> ResetThis()
        {
            EventsToTrigger.Clear();
            IsDone = false;
            return this;
        }

        public void Process(T value)
        {
            IsDone = true;
            for (int i = 0; i < EventsToTrigger.Count; i++) { EventsToTrigger[i]?.Invoke(value); }
        }

        public override string ToString() => $"{GetType().Name}_{typeof(T).Name} (Done:{IsDone} - Permanent: {IsPermanent}";
    }
}
