using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.TimeProgress
{
    /// <summary>
    /// a generic timer
    /// </summary>
    public abstract class J_GenericCounter : ScriptableObject, jObservable<float>, iDeltaTime
    {
        // --------------- FIELDS AND PROPERTIES --------------- // 
        protected event Action<float> OnTick;

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField] protected Segment _desiredSegment = Segment.Update;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _objectId = -1;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float ThisDeltaTime { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected float CurrentRealSeconds => Time.realtimeSinceStartup;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private CoroutineHandle _handle;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsActive => _handle.IsValid && _handle.IsRunning;

        // --------------- COMMANDS --------------- // 
        protected static T CreateCounter<T>(Segment desiredSegment = Segment.Update)
            where T : J_GenericCounter
        {
            var counter = CreateInstance<T>();
            counter._desiredSegment = desiredSegment;
            return counter;
        }

        // starts the counter
        protected void StartCount()
        {
            //make sure everything is setup correctly and starts the counting
            JLog.Log($"{name} starts counting", JLogTags.TimeProgress, this);
            if (!SanityChecks()) return;
            //complete the setup
            _objectId = this.JUniqueId();
            //starts counting
            Tick();
        }

        // stops the counter
        protected void StopCount()
        {
            JLog.Log($"{name} stops counting", JLogTags.TimeProgress, this);
            Timing.KillCoroutines(_objectId, JCoroutineTags.COROUTINE_CounterTag);
        }

        // --------------- INITIALIZATION --------------- // 
        //make sure this is setup correctly, used in subclasses
        protected virtual bool SanityChecks() => true;

        // --------------- COUNTING --------------- // 
        protected void Tick()
            => _handle = Timing.RunCoroutine(CountOneTick(), _desiredSegment, _objectId, JCoroutineTags.COROUTINE_CounterTag);

        //this counts a single tick
        protected abstract IEnumerator<float> CountOneTick();

        //store the delta time and send the event
        protected void SendTickEvent(float tickValue)
        {
            ThisDeltaTime = tickValue;
            OnTick?.Invoke(tickValue);
        }

        // --------------- SUBSCRIBERS --------------- //
        public void Subscribe(Action<float> action)
        {
            if (!IsActive) StartCount();
            OnTick += action;
        }

        public void UnSubscribe(Action<float> action)
        {
            OnTick -= action;
            if (OnTick == null) StopCount();
        }
    }
}
