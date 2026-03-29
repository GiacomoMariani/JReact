using Unity.Mathematics;
using UnityEngine.Assertions;

#if UNITY_DOTS

namespace JReact.JDots
{
    public readonly struct JSmooth
    {
        /// <summary>
        /// Smoothly moves a 2D value toward a target using a critically-damped spring model (SmoothDamp).
        /// Designed to feel like Unity's SmoothDamp, but implemented with Unity.Mathematics for DOTS/Burst.
        /// </summary>
        /// <param name="current">
        /// The current 2D value you want to damp (e.g., current position/offset).
        /// </param>
        /// <param name="target">
        /// The 2D value you want to approach smoothly (e.g., float2.zero).
        /// </param>
        /// <param name="currentVelocity">
        /// Persistent state (units per second) carried between updates.
        /// Pass the same variable/component each frame; it will be modified by this function.
        /// </param>
        /// <param name="smoothTime">
        /// Controls responsiveness / smoothing time scale (seconds).
        /// Smaller = snappier (less smoothing), larger = smoother (slower). Must be > 0.
        /// </param>
        /// <param name="deltaTime">
        /// Time step for this update in seconds (typically SystemAPI.Time.DeltaTime).
        /// Keeps behavior consistent across frame rates.
        /// </param>
        /// <param name="maxSpeed">
        /// Optional maximum speed (units per second) the value is allowed to move toward the target.
        /// Use float.PositiveInfinity for no cap (default). Helpful to prevent large jumps.
        /// </param>
        /// <returns>
        /// The new damped value for this update step.
        /// </returns>
        public float2 SmoothDamp2D(float2 current, float2 target, ref float2 currentVelocity, float smoothTime, float deltaTime,
                                   float  maxSpeed = float.PositiveInfinity)
        {
            Assert.IsTrue(smoothTime > 0.0001f, $"Smooth time must be greater than 0.0001f. Received {smoothTime}");
            float omega = 2f / smoothTime;

            float x = omega * deltaTime;
            // Stable approximation of exp(-omega * dt)
            float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);

            float2 change     = current - target;
            float2 originalTo = target;

            // Clamp maximum speed (by magnitude)
            float maxChange                       = maxSpeed * smoothTime;
            float changeLenSq                     = math.lengthsq(change);
            float maxChangeSq                     = maxChange   * maxChange;
            if (changeLenSq > maxChangeSq) change *= (maxChange / math.sqrt(changeLenSq));

            target = current - change;

            float2 temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;

            float2 output = target + (change + temp) * exp;

            // Prevent overshoot
            float2 toOriginal = originalTo - current;
            float2 toOutput   = output     - originalTo;
            if (math.dot(toOriginal, toOutput) > 0f)
            {
                output          = originalTo;
                currentVelocity = float2.zero;
            }

            return output;
        }

        /// <summary>
        /// Smoothly moves a scalar (float) toward a target using a critically-damped spring model (SmoothDamp).
        /// DOTS/Burst-friendly port of Unity-style SmoothDamp logic using Unity.Mathematics.
        /// </summary>
        /// <param name="current">
        /// The current value you want to damp.
        /// </param>
        /// <param name="target">
        /// The value you want to approach smoothly.
        /// </param>
        /// <param name="currentVelocity">
        /// Persistent state (units per second) carried between updates.
        /// Pass the same variable/component each frame; it will be modified by this function.
        /// </param>
        /// <param name="smoothTime">
        /// Controls responsiveness / smoothing time scale (seconds).
        /// Smaller = snappier (less smoothing), larger = smoother (slower). Must be > 0.
        /// </param>
        /// <param name="deltaTime">
        /// Time step for this update in seconds (typically SystemAPI.Time.DeltaTime).
        /// Keeps behavior consistent across frame rates.
        /// </param>
        /// <param name="maxSpeed">
        /// Optional maximum speed (units per second) the value is allowed to move toward the target.
        /// Use float.PositiveInfinity for no cap (default). Helpful to prevent large jumps.
        /// </param>
        /// <returns>
        /// The new damped value for this update step.
        /// </returns>
        public float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float deltaTime,
                                float maxSpeed = float.PositiveInfinity)
        {
            Assert.IsTrue(smoothTime > 0.0001f, $"Smooth time must be greater than 0.0001f. Received {smoothTime}");
            float omega = 2f / smoothTime;

            float x = omega * deltaTime;
            // Stable approximation of exp(-omega * dt)
            float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);

            float change     = current - target;
            float originalTo = target;

            // Clamp maximum speed
            float maxChange = maxSpeed * smoothTime;
            change = math.clamp(change, -maxChange, maxChange);

            // Adjust target to reflect clamped change
            target = current - change;

            float temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;

            float output = target + (change + temp) * exp;

            // Prevent overshoot
            if ((originalTo - current > 0f) == (output > originalTo))
            {
                output          = originalTo;
                currentVelocity = 0f;
            }

            return output;
        }
    }
}
#endif
