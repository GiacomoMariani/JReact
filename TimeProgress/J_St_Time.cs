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

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private float _elapsed;

        // --------------- STATIC CALL SITE (use these in place of UnityEngine.Time) --------------- //
        public static float Multiplier             => GetInstanceSafe()._multiplier;
        public static float Elapsed                => GetInstanceSafe()._elapsed;

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
        private void Update() { _elapsed += DeltaTime; }

        // --------------- MEC WAIT ON OUR CLOCK --------------- //
        /// <summary>
        /// Game-time replacement for <c>Timing.WaitForSeconds</c>: waits one frame at a time, subtracting our
        /// scaled <see cref="DeltaTime"/> 
        /// </summary>
        public static IEnumerator<float> WaitForSeconds(float seconds)
        {
            J_St_Time time      = GetInstanceSafe();
            float     remaining = seconds;
            while (remaining > 0f)
            {
                yield return Timing.WaitForOneFrame;
                if (time == null) { yield break; }
                remaining -= UTime.deltaTime * time._multiplier;
            }
        }

        /// <summary>
        /// Real-time variant of <see cref="WaitForSeconds"/>
        /// </summary>
        public static IEnumerator<float> WaitForSecondsUnscaled(float seconds)
        {
            J_St_Time time      = GetInstanceSafe();
            float     remaining = seconds;
            while (remaining > 0f)
            {
                yield return Timing.WaitForOneFrame;
                if (time == null) { yield break; }
                remaining -= UTime.unscaledDeltaTime;
            }
        }

        // --------------- COMMANDS --------------- //
        public static void SetMultiplier(float multiplier) { GetInstanceSafe()._multiplier = multiplier; }
        public static void ResetElapsed() { GetInstanceSafe()._elapsed = 0f; }

        protected internal override void InitThis()
        {
            base.InitThis();
            JLog.Log(Ready, JLogTags.TimeProgress, this);
        }

        private const float  DefaultMultiplier = 1f;
        private const string Ready             = "J_St_Time: ready.";
    }
}
