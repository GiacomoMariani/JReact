﻿using JReact.TimeProgress.Pause;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.TimeProgress
{
    /// <summary>
    /// this is an event connected to a time
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Time/Progress Event")]
    public class J_ProgressEvent : ScriptableObject, iObservable<J_ProgressEvent>, iResettable
    {
        #region VALUES AND PROPERTIES
        // --------------- EVENTS RELATED TO PROGRESS --------------- //
        private event JGenericDelegate<J_ProgressEvent> OnProgressStart;
        private event JGenericDelegate<J_ProgressEvent> OnProgressTick;
        private event JGenericDelegate<J_ProgressEvent> OnProgressComplete;

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] protected J_Timer _timer;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] private J_Identifier _identifier;
        public J_Identifier Identifier => _identifier;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_PauseEvent _pauseEvent;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _timeRequiredInSeconds;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float SecondsFromStart { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _paused = true;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _destroyAtDisable = false;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsRunning { get; private set; } = false;

        // --------------- BOOK KEEPING --------------- //
        [BoxGroup("Book Keeping", true, true, 10), ReadOnly, ShowInInspector]
        public float ProgressPercentage => SecondsFromStart / _timeRequiredInSeconds;
        [BoxGroup("Book Keeping", true, true, 10), ReadOnly, ShowInInspector]
        public float RemainingSeconds => _timeRequiredInSeconds - SecondsFromStart;
        #endregion

        #region PRE SETUP
        public static J_ProgressEvent CreateProgressEvent(bool destroyAtDisable = true)
        {
            var progress = CreateInstance<J_ProgressEvent>();
            progress._destroyAtDisable = destroyAtDisable;
            return progress;
        }

        //add the identifier when requested
        public void SetIdentifier(J_Identifier identifier)
        {
            if (_identifier != null)
                JConsole.Warning($"{name} has already an identifier ({_identifier.name}. Cannot set {identifier.name})",
                                 JLogTags.TimeProgress, this);
            if (_identifier == null) _identifier = identifier;
        }

        //make sure we have a valid timer
        public void SetTimer(J_Timer timer)
        {
            if (_timer != null)
                JConsole.Warning($"{name} has already a timer ({_timer.name}. Cannot set {timer.name})",
                                 JLogTags.TimeProgress, this);
            if (_timer == null) _timer = timer;
        }
        #endregion

        #region PAUSE
        /// <summary>
        /// connect the progress to a pause event
        /// </summary>
        /// <param name="pauseEvent"></param>
        public void InjectPauseEvent(J_PauseEvent pauseEvent)
        {
            _pauseEvent = pauseEvent;
            TrackPause();
        }

        /// <summary>
        /// used to stop tracking a pause
        /// </summary>
        public void RemovePause()
        {
            Assert.IsNotNull(_pauseEvent, $"{name} has no pause event to remove");
            UnTrackPause();
        }

        private void TrackPause()
        {
            if (_pauseEvent.IsPaused &&
                !_paused) SetPause(true);
            _pauseEvent.SubscribeToPauseStart(Pause);
            _pauseEvent.SubscribeToPauseEnd(UnPause);
        }

        private void UnTrackPause()
        {
            if (_pauseEvent == null) return;
            _pauseEvent.UnSubscribeToPauseStart(Pause);
            _pauseEvent.UnSubscribeToPauseEnd(UnPause);
            _pauseEvent = null;
        }

        private void Pause() { SetPause(true); }
        private void UnPause(int item) { SetPause(false); }
        #endregion

        #region COMMANDS
        /// <summary>
        /// start the progress. Elements may be injected at start
        /// </summary>
        /// <param name="secondsToComplete">time required to complete this progress</param>
        /// <param name="createNewTimer">set to true if we want to create/replace the timer</param>
        public void StartProgress(float secondsToComplete, bool createNewTimer = false)
        {
            // --------------- INITIALIZATION --------------- //
            if (!SanityChecks(secondsToComplete)) return;
            ResetValues();

            // --------------- SETUP --------------- //
            if (createNewTimer) _timer = J_GenericCounter.CreateNewTimer<J_Timer>();
            _timeRequiredInSeconds = (int) secondsToComplete;
            Assert.IsNotNull(_timer, $"{name} has no timer");
            if (!_timer.IsRunning)
            {
                JConsole.Warning($"{_timer.name} on {name} was not running. Force Start.", JLogTags.TimeProgress, this);
                _timer.StartCount();
            }

            // --------------- RUN --------------- //
            IsRunning = true;
            StartEvent();
            _timer.Subscribe(AddTimePerTick);
        }

        /// <summary>
        /// stop the timer definitely and reset to its base values
        /// </summary>
        public void StopProgress() { ResetValues(); }

        /// <summary>
        /// adds a fixed amount of seconds to the progress
        /// </summary>
        /// <param name="secondsToAdd">the seconds to be added</param>
        public void AddTime(float secondsToAdd) { SecondsFromStart += secondsToAdd; }

        /// <summary>
        /// starts and stops the progress
        /// </summary>
        /// <param name="inPause">to set the pause</param>
        public void SetPause(bool inPause) { _paused = inPause; }

        /// <summary>
        /// fast finish the progress
        /// </summary>
        [BoxGroup("Test", true, true, 100), Button("FastFinish", ButtonSizes.Medium)]
        public void FastFinish() { ProgressComplete(); }
        #endregion

        #region SETUP
        //make sure all the fields are correct
        private bool SanityChecks(float secondsToComplete)
        {
            Assert.IsNotNull(_timer, $"{name} has not timer. Command canceled.");
            Assert.IsTrue(secondsToComplete > 0,
                          $"{name} requires positive secondsToComplete. Received: {secondsToComplete}.Command canceled.");
            Assert.IsFalse(IsRunning, $"{name} is already started. Command canceled.");
            return secondsToComplete > 0 && !IsRunning && _timer != null;
        }
        #endregion

        #region MAIN EVENTS
        //sends the start event
        [BoxGroup("Debug", true, true, 100), Button("Start Event", ButtonSizes.Medium)]
        private void StartEvent() { OnProgressStart?.Invoke(this); }

        //sends the tick event
        [BoxGroup("Debug", true, true, 100), Button("Tick Event", ButtonSizes.Medium)]
        private void TickEvent() { OnProgressTick?.Invoke(this); }

        //sends the complete event
        [BoxGroup("Debug", true, true, 100), Button("Complete Event", ButtonSizes.Medium)]
        private void CompleteEvent() { OnProgressComplete?.Invoke(this); }
        #endregion

        #region COUNT AND COMPLETION
        //add the time for each tick
        private void AddTimePerTick(float timePassed)
        {
            //stop if this is not active
            if (_paused) return;

            //add the time to the time passed
            SecondsFromStart += timePassed;
            TickEvent();

            //stop if we reached the end
            if (SecondsFromStart >= _timeRequiredInSeconds) ProgressComplete();
        }

        //this is used to start the construction
        private void ProgressComplete()
        {
            Assert.IsTrue(IsRunning, $"{name} only running progress may complete");
            StopTrackingTime();
            CompleteEvent();
        }
        #endregion

        #region SUBSCRIBE EVENTS
        public void SubscribeToStart(JGenericDelegate<J_ProgressEvent> actionToSend) { OnProgressStart   += actionToSend; }
        public void UnSubscribeToStart(JGenericDelegate<J_ProgressEvent> actionToSend) { OnProgressStart -= actionToSend; }

        public void Subscribe(JGenericDelegate<J_ProgressEvent> actionToSend) { OnProgressTick   += actionToSend; }
        public void UnSubscribe(JGenericDelegate<J_ProgressEvent> actionToSend) { OnProgressTick -= actionToSend; }

        public void SubscribeToComplete(JGenericDelegate<J_ProgressEvent> actionToSend) { OnProgressComplete   += actionToSend; }
        public void UnSubscribeToComplete(JGenericDelegate<J_ProgressEvent> actionToSend) { OnProgressComplete -= actionToSend; }
        #endregion

        #region DISABLE AND RESET
        private void OnDisable() { ResetThis(); }

        public void ResetThis()
        {
            UnTrackPause();
            ResetValues();
            ResetEvents();
            if (_destroyAtDisable) Destroy(this);
        }

        private void ResetValues()
        {
            //reset the fields
            _timeRequiredInSeconds = 0;
            SecondsFromStart       = 0;
            _paused                = false;
            //stop if the progress is not counting
            if (IsRunning) StopTrackingTime();
        }

        private void StopTrackingTime()
        {
            _timer.UnSubscribe(AddTimePerTick);
            IsRunning = false;
        }

        private void ResetEvents()
        {
            OnProgressStart    = null;
            OnProgressComplete = null;
            OnProgressTick     = null;
        }
        #endregion
    }

    //an interface for the progress view
    public interface iProgressView
    {
        void InjectProgress(J_ProgressEvent progress, J_Identifier identifier = null);
    }
}
