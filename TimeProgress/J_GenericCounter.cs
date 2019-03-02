using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.TimeProgress
{
    /// <summary>
    /// a generic timer
    /// </summary>
    public abstract class J_GenericCounter : J_Service, iObservable<float>, iDeltaTime
    {
        #region FIELDS AND PROPERTIES
        protected event JGenericDelegate<float> OnTick;

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] protected Segment _desiredSegment = Segment.Update;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected int _objectId = -1;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _destroyAtDisable;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float ThisDeltaTime { get; private set; }
        #endregion

        #region COMMANDS
        /// <summary>
        /// creates a new counter and starts counting
        /// </summary>
        /// <returns>the new counter created</returns>
        public static T CreateNewTimer<T>(bool destroyAtDisable = true)
            where T : J_GenericCounter
        {
            var timer = CreateInstance<T>();
            timer._destroyAtDisable = destroyAtDisable;
            timer.Activate();
            return timer;
        }

        //starts the counter
        public override void Activate()
        {
            base.Activate();
            //make sure everything is setup correctly and starts the counting
            JConsole.Log($"{name} starts counting", JLogTags.TimeProgress, this);
            if (!SanityChecks()) return;
            //complete the setup
            _objectId = GetInstanceID();
            //starts counting
            Timing.RunCoroutine(CountOneTick(), _desiredSegment, _objectId, JCoroutineTags.COROUTINE_TimerTag);
        }

        //stops the timer
        public override void End()
        {
            base.End();
            JConsole.Log($"{name} stops counting", JLogTags.TimeProgress, this);
            Timing.KillCoroutines(_objectId, JCoroutineTags.COROUTINE_TimerTag);
        }
        #endregion

        #region INITIALIZATION
        //make sure this is setup correctly, used in subclasses
        protected virtual bool SanityChecks() => true;
        #endregion

        #region COUNTING
        //this counts a single tick
        protected abstract IEnumerator<float> CountOneTick();

        //store the delta time and send the event
        protected void SendTickEvent(float tickValue)
        {
            ThisDeltaTime = tickValue;
            OnTick?.Invoke(tickValue);
        }
        #endregion

        #region SUBSCRIBERS
        public void Subscribe(JGenericDelegate<float> action)
        {
            if (!IsActive) Activate();
            OnTick += action;
        }

        public void UnSubscribe(JGenericDelegate<float> action) { OnTick -= action; }
        public void SubscribeToCounter(JGenericDelegate<float> action) { Subscribe(action); }
        public void UnSubscribeToCounter(JGenericDelegate<float> action) { UnSubscribe(action); }
        #endregion

        #region DISABLE AND RESET
        public override void ResetThis()
        {
            base.ResetThis();
            if (_destroyAtDisable) Destroy(this);
        }
        #endregion
    }
}
