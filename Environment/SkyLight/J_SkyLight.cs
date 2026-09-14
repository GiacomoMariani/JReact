using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Environment.SkyLight
{
    public enum SkyLightKind { Invalid, None, Sun, Moon }
    public enum SkyLightStrength { Invalid, None, Low, Medium, Strong }

    /// <summary>
    /// The sky light DATA: direction, brightness, shadow multiplier and kind, published to listeners on
    /// every change. It holds no motion - author the serialized start values for a static light, or let
    /// a driver (J_SkyLightMover) feed it through the public setters.
    /// </summary>
    public class J_SkyLight : MonoBehaviour, jObservable<J_SkyLight>
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private Vector2 _startDirection = Vector2.down;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0f)] private float _startBrightness = 1f;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0f)] private float _startShadowMultiplier = 1f;
        [BoxGroup("Setup", true, true, 0), SerializeField] private SkyLightKind _startKind = SkyLightKind.Sun;
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _lowThreshold = 0.1f;
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _mediumThreshold = 0.4f;
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _strongThreshold = 0.7f;

        // --------------- STATE --------------- //
        /// <summary>Light amount 0..1; drivers should keep it at or above _lowThreshold.</summary>
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float Brightness { get; private set; }
        /// <summary>Normalized direction the shadow is cast along.</summary>
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Vector2 LightDirection { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float ShadowMultiplier { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public SkyLightKind SkyLightKind { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public SkyLightStrength SkyLightStrength { get; private set; }
        /// <summary>Seconds between expected publishes from a driver; 0 = untimed (static light).</summary>
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float TransitionInterval { get; private set; }

        private readonly List<Action<J_SkyLight>> _listeners = new List<Action<J_SkyLight>>();

        // --------------- COMMANDS --------------- //
        /// <summary>Sets direction, brightness and shadow multiplier as ONE change with a single publish.</summary>
        public void SetLight(Vector2 direction, float brightness, float shadowMultiplier)
        {
            bool changed = false;
            changed |= ApplyDirection(direction);
            changed |= ApplyBrightness(brightness);
            changed |= ApplyShadowMultiplier(shadowMultiplier);
            if (changed) { TriggerEvent(); }
        }

        public void SetDirection(Vector2 direction)
        {
            if (ApplyDirection(direction)) { TriggerEvent(); }
        }

        public void SetBrightness(float brightness)
        {
            if (ApplyBrightness(brightness)) { TriggerEvent(); }
        }

        public void SetShadowMultiplier(float shadowMultiplier)
        {
            if (ApplyShadowMultiplier(shadowMultiplier)) { TriggerEvent(); }
        }

        public void SetKind(SkyLightKind kind)
        {
            if (SkyLightKind == kind) { return; }

            SkyLightKind = kind;
            TriggerEvent();
        }

        /// <summary>Cadence hint only - stored for consumers, does not publish.</summary>
        public void SetTransitionInterval(float seconds) => TransitionInterval = Mathf.Max(0f, seconds);

        // --------------- APPLIERS --------------- //
        private bool ApplyDirection(Vector2 direction)
        {
            Vector2 normalized = direction.sqrMagnitude > 0.000001f ? direction.normalized : Vector2.down;
            if (LightDirection == normalized) { return false; }

            LightDirection = normalized;
            return true;
        }

        private bool ApplyBrightness(float brightness)
        {
            brightness = Mathf.Max(0f, brightness);
            if (Mathf.Approximately(Brightness, brightness)) { return false; }

            Brightness       = brightness;
            SkyLightStrength = StrengthFor(brightness);
            return true;
        }

        private bool ApplyShadowMultiplier(float shadowMultiplier)
        {
            shadowMultiplier = Mathf.Max(0f, shadowMultiplier);
            if (Mathf.Approximately(ShadowMultiplier, shadowMultiplier)) { return false; }

            ShadowMultiplier = shadowMultiplier;
            return true;
        }

        private SkyLightStrength StrengthFor(float brightness)
        {
            if (brightness >= _strongThreshold) { return SkyLightStrength.Strong; }

            if (brightness >= _mediumThreshold) { return SkyLightStrength.Medium; }

            if (brightness >= _lowThreshold) { return SkyLightStrength.Low; }

            return SkyLightStrength.None;
        }

        private void TriggerEvent()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--) { _listeners[i].Invoke(this); }
        }

        // --------------- LIFECYCLE --------------- //
        private void OnEnable()
        {
            SkyLightKind = _startKind;
            ApplyDirection(_startDirection);
            ApplyBrightness(_startBrightness);
            ApplyShadowMultiplier(_startShadowMultiplier);
            TriggerEvent();
        }

        public void Subscribe(Action<J_SkyLight>   actionToSend) => _listeners.Add(actionToSend);
        public void UnSubscribe(Action<J_SkyLight> actionToSend) => _listeners.Remove(actionToSend);
    }
}
