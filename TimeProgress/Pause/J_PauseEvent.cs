using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.TimeProgress.Pause
{
    public interface iPausableObject
    {
        void OnPauseEnter();
        void OnPauseExit(int secondsPassedOffline);
    }

    /// <summary>
    /// calculates and handle pause (could be used also with J_Mono_OutOfFocusPause)
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Time Progress/Pause Event")]
    public class J_PauseEvent : ScriptableObject
    {
        // --------------- EVENTS--------------- //
        private event Action OnPauseStart;
        private event Action<int> OnPauseEnds;

        // --------------- VALUES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected int _startTimeUnix;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected int _timeOffline;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected bool _isPaused;
        internal bool IsPaused => _isPaused;

        // --------------- COMMANDS - START PAUSE --------------- //
        /// <summary>
        /// start the pause event
        /// </summary>
        [Button(ButtonSizes.Medium)]
        public void StartPause()
        {
            //ignore if already in pause
            if (IsPaused)
            {
                JLog.Warning($"{name} The pause is already active. Can't pause again.", JLogTags.TimeProgress);
                return;
            }

            CalculateStartTime();
            StartingPause();
        }

        //process the pause syaty
        private void StartingPause()
        {
            _isPaused = true;
            OnPauseStart?.Invoke();
            JLog.Log($"Pause starts at {_startTimeUnix}");
        }

        // --------------- COMMANDS - END PAUSE --------------- //
        /// <summary>
        /// stops the pause
        /// </summary>
        [Button(ButtonSizes.Medium)]
        public void EndPause()
        {
            //ignore if it was not in pause
            if (!IsPaused)
            {
                JLog.Warning($"{name} Pause was not active. Can't stop pause.", JLogTags.TimeProgress);
                return;
            }

            CalculateSecondsOffline();
            EndingPause();
        }

        //process the pause stop
        private void EndingPause()
        {
            if (OnPauseEnds != null) OnPauseEnds(_timeOffline);
            JLog.Log($"Pause ends at {GetCurrentDate()}. Time passed in pause: {_timeOffline}");
            ResetThis();
        }

        // --------------- HELPER --------------- //
        /// <summary>
        /// calculates the current time
        /// </summary>
        /// <returns>returns the date from the system</returns>
        protected virtual DateTime GetCurrentDate() => DateTime.UtcNow;

        /// <summary>
        /// calculates the time at the start of pause, by default it gets it from the system
        /// </summary>
        protected virtual void CalculateStartTime() { _startTimeUnix = GetCurrentDate().GetUnixTimeStamp(); }

        /// <summary>
        /// calculates the time passed offline
        /// </summary>
        protected virtual void CalculateSecondsOffline() { _timeOffline = GetCurrentDate().GetUnixTimeStamp() - _startTimeUnix; }

        // --------------- SUBSCRIBERS --------------- //
        public void SubscribeToPauseStart(Action   actionToSend) { OnPauseStart += actionToSend; }
        public void UnSubscribeToPauseStart(Action actionToSend) { OnPauseStart -= actionToSend; }

        public void SubscribeToPauseEnd(Action<int>   actionToSend) { OnPauseEnds += actionToSend; }
        public void UnSubscribeToPauseEnd(Action<int> actionToSend) { OnPauseEnds -= actionToSend; }

        // --------------- RESET METHODS --------------- //
        private void OnDisable() { ResetThis(); }

        protected virtual void ResetThis()
        {
            _startTimeUnix = 0;
            _isPaused      = false;
        }
    }
}
