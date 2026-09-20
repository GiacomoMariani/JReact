using System.Collections.Generic;
using JReact.Singleton;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UTime = UnityEngine.Time;

namespace JReact.TimeProgress
{
    public class J_St_Time : J_MonoSingleton<J_St_Time>
    {
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _multiplier = DefaultMultiplier;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private double _elapsed;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private double _elapsedOrigin;

        // Cached consumers use this monotonic clock for deadlines, independent of ResetElapsed.
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public double GameTime => _elapsed;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float ScaledDeltaTime => UTime.deltaTime * _multiplier;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsPaused => _multiplier <= 0f;

        // --------------- STATIC CALL SITE (use these in place of UnityEngine.Time) --------------- //
        public static float Multiplier             => GetInstanceSafe()._multiplier;
        public static double Elapsed
        {
            get
            {
                var time = GetInstanceSafe();
                return time._elapsed - time._elapsedOrigin;
            }
        }

        public static float DeltaTime              => UTime.deltaTime              * Multiplier;
        public static float FixedDeltaTime         => UTime.fixedDeltaTime         * Multiplier;
        public static float SmoothDeltaTime        => UTime.smoothDeltaTime        * Multiplier;
        public static float UnscaledDeltaTime      => UTime.unscaledDeltaTime;
        public static float UnscaledFixedDeltaTime => UTime.fixedUnscaledDeltaTime;
        public static float UnityTime              => UTime.time;
        public static float UnscaledTime           => UTime.unscaledTime;
        public static float RealtimeSinceStartup   => UTime.realtimeSinceStartup;
        public static int   FrameCount             => UTime.frameCount;

        // --------------- TICK --------------- //
        private void Update() { _elapsed += (double)UTime.deltaTime * _multiplier; }

        // --------------- MEC WAIT ON OUR CLOCK --------------- //
        /// <summary>
        /// Waits on accumulated game time, including frames between SlowUpdate resumptions.
        /// Pausing or changing the multiplier affects waits already in progress.
        /// </summary>
        public IEnumerator<float> WaitForSeconds(float seconds)
        {
            double deadline = _elapsed + seconds;
            while (_elapsed < deadline)
            {
                yield return Timing.WaitForOneFrame;
            }
        }

        /// <summary>
        /// Real-time variant of <see cref="WaitForSeconds"/>
        /// </summary>
        public IEnumerator<float> WaitForSecondsUnscaled(float seconds)
        {
            double deadline = UTime.unscaledTimeAsDouble + seconds;
            while (UTime.unscaledTimeAsDouble < deadline)
            {
                yield return Timing.WaitForOneFrame;
            }
        }

        // --------------- COMMANDS --------------- //
        public void SetMultiplier(float multiplier) { _multiplier = multiplier; }
        // Reset the reported origin without moving deadlines of active waits.
        public void ResetElapsed() { _elapsedOrigin = _elapsed; }

        protected internal override void InitThis()
        {
            base.InitThis();
            JLog.Log(Ready, JLogTags.TimeProgress, this);
        }

        private const float  DefaultMultiplier = 1f;
        private const string Ready             = "J_St_Time: ready.";
    }
}
