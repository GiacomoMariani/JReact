#if UNITY_DOTS
using System;
using System.Runtime.CompilerServices;
using JMath2D.JPhysics;
using JReact;
using Unity.Entities;
using Unity.Mathematics;

namespace JMath2D
{
    /// <summary>
    /// tuning for the curved aim assist, all angles in radians
    /// the curve maps the normalized angular offset (0 = aiming dead on the target, 1 = edge of the assist cone)
    /// to a correction fraction (0 = no assist, 1 = aim bent fully onto the target)
    /// the curve is expected monotonic decreasing with value 0 at time 1, so the assist stays continuous at the cone edge
    /// </summary>
    public struct JAimAssistSettings : IDisposable
    {
        //the assist cone is the target angular size scaled by this, then clamped between min and max
        public readonly float ConeSizeMultiplier;
        public readonly float MinConeHalfAngle;
        public readonly float MaxConeHalfAngle;
        //hard cap on the bend applied to the aim, no matter what the curve returns; 0 or less disables the assist
        public readonly float MaxCorrectionAngle;
        //not readonly so Dispose can release the blob; copied by value is safe — it is a handle, not the blob itself
        public BlobAssetReference<JECS_CurveBlob> Curve;

        //the blob has been baked (used for caching); validity additionally requires a positive correction cap
        public readonly bool IsCreated => Curve.IsCreated;
        public readonly bool IsValid => Curve.IsCreated && MaxCorrectionAngle > 0f;

        public JAimAssistSettings(float coneSizeMultiplier, float minConeHalfAngle, float maxConeHalfAngle,
                                  float maxCorrectionAngle, BlobAssetReference<JECS_CurveBlob> curve)
        {
            ConeSizeMultiplier = coneSizeMultiplier;
            MinConeHalfAngle   = minConeHalfAngle;
            MaxConeHalfAngle   = maxConeHalfAngle;
            MaxCorrectionAngle = maxCorrectionAngle;
            Curve              = curve;
        }

        public void Dispose()
        {
            if (Curve.IsCreated) { Curve.Dispose(); }
        }
    }

    public readonly struct JAimAssistResult
    {
        public readonly float2 AssistedAimPosition;
        //fraction of the angular offset actually corrected, 0 = untouched aim, 1 = aim moved fully onto the target
        //this is the applied fraction after the MaxCorrectionAngle cap, so it can be lower than the curve value
        public readonly float Strength;

        public JAimAssistResult(float2 assistedAimPosition, float strength)
        {
            AssistedAimPosition = assistedAimPosition;
            Strength            = strength;
        }

        public static JAimAssistResult NoAssist(float2 rawAimPosition) => new(rawAimPosition, 0f);
    }

    /// <summary>
    /// curved aim assist as a continuous remap of the aim angle around a pivot (no snapping)
    /// inside the assist cone the aim direction is bent toward the target by a curve driven fraction of its offset,
    /// outside the cone (and on any degenerate input) the raw aim is returned untouched
    /// </summary>
    public static class JAimAssist
    {
        private const float MinLengthSqr = 1e-12f;
        private const float MinAngle = 1e-6f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JAimAssistResult Calculate(float2 pivotPosition, float2 rawAimPosition, JCircle2D targetCircle,
                                                 in JAimAssistSettings settings)
        {
            if (!settings.IsValid) { return JAimAssistResult.NoAssist(rawAimPosition); }

            float2 toAim             = rawAimPosition - pivotPosition;
            float  aimDistanceSqr    = math.lengthsq(toAim);
            float2 toTarget          = targetCircle.Center - pivotPosition;
            float  targetDistanceSqr = math.lengthsq(toTarget);
            //no valid aim or target direction => nothing to bend (negated form so NaN inputs also fail into the early out)
            if (!(aimDistanceSqr    >= MinLengthSqr) ||
                !(targetDistanceSqr >= MinLengthSqr)) { return JAimAssistResult.NoAssist(rawAimPosition); }

            float  aimDistance     = math.sqrt(aimDistanceSqr);
            float  targetDistance  = math.sqrt(targetDistanceSqr);
            float2 inputDirection  = toAim    / aimDistance;
            float2 targetDirection = toTarget / targetDistance;

            float coneHalfAngle = CalculateConeHalfAngle(targetCircle.Radius, targetDistance, settings);

            //positive => target is clockwise from the aim (matching SignedDeltaRadians convention)
            float offsetToTarget = inputDirection.SignedDeltaRadians(targetDirection);
            float absOffset      = math.abs(offsetToTarget);
            //negated form so a NaN offset or cone also fails into the early out, keeping GetValue input inside [0,1)
            if (!(absOffset < coneHalfAngle)) { return JAimAssistResult.NoAssist(rawAimPosition); }

            float normalizedOffset   = absOffset / coneHalfAngle;
            float correctionFraction = math.saturate(settings.Curve.Value.GetValue(normalizedOffset));

            //dead on target: no bend needed, still report the assist strength for feedback
            if (absOffset < MinAngle) { return new JAimAssistResult(rawAimPosition, correctionFraction); }

            float  correctionAngle   = math.min(correctionFraction * absOffset, settings.MaxCorrectionAngle);
            float2 assistedDirection = RotateClockwise(inputDirection, math.sign(offsetToTarget) * correctionAngle);
            float2 assistedPosition  = pivotPosition + assistedDirection * aimDistance;

            return new JAimAssistResult(assistedPosition, correctionAngle / absOffset);
        }

        //exposed so debug views can draw the exact cone the assist uses (single source of truth)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalculateConeHalfAngle(float targetRadius, float targetDistance, in JAimAssistSettings settings)
        {
            //angular half size of the target as seen from the pivot, PI/2 when the pivot is inside the circle
            float sinAngularSize  = math.saturate(targetRadius / targetDistance);
            float angularHalfSize = math.asin(sinAngularSize);
            return math.clamp(angularHalfSize * settings.ConeSizeMultiplier, settings.MinConeHalfAngle, settings.MaxConeHalfAngle);
        }

        //clockwise rotation to match the SignedDeltaRadians convention (positive = clockwise)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float2 RotateClockwise(float2 vector, float radians)
        {
            math.sincos(radians, out float sin, out float cos);
            return new float2(vector.x * cos + vector.y * sin, vector.y * cos - vector.x * sin);
        }
    }
}
#endif
