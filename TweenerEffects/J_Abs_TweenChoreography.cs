using System;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    public enum JChoreographyState : byte { NotStarted, Waiting, Running, Done }

    /// <summary>A reusable tween choreography that owns its playback and state.</summary>
    public abstract class J_Abs_TweenChoreography : MonoBehaviour
    {
        private Sequence _sequence;
        private JChoreographyState _state = JChoreographyState.NotStarted;
        private float _timeScale = 1f;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public JChoreographyState State
        {
            get
            {
                if (!_sequence.isAlive &&
                    (_state == JChoreographyState.Waiting || _state == JChoreographyState.Running))
                {
                    return JChoreographyState.NotStarted;
                }

                return _state;
            }
        }

        public bool IsRunning => State == JChoreographyState.Running;
        public bool IsRunningOrWaiting => State == JChoreographyState.Running || State == JChoreographyState.Waiting;
        public bool IsDone => State == JChoreographyState.Done;
        public bool IsPaused => _sequence.isAlive && _sequence.isPaused;
        public Sequence TweenSequence => _sequence;

        public void SetPaused(bool paused)
        {
            if (_sequence.isAlive) { _sequence.isPaused = paused; }
        }

        public void SetTimeScale(float timeScale)
        {
            _timeScale = Mathf.Max(0f, timeScale);
            if (_sequence.isAlive) { _sequence.timeScale = _timeScale; }
        }

        protected virtual bool UseUnscaledTime => false;

        /// <summary>Default playback length. Host-timed choreographies complete immediately unless given a duration.</summary>
        public virtual float MotionDuration => 0f;

        [Button]
        public void Play() => Play(0f, MotionDuration);

        /// <summary>Apply the initial appearance before playback, including any layout wait.</summary>
        public void Prepare()
        {
            EnsurePreviousPlaybackReleased();
            _state = JChoreographyState.NotStarted;
            PrepareImpl();
        }

        /// <summary>Play once, with one delay before the whole effect.</summary>
        public virtual void Play(float delay, float duration)
        {
            EnsurePreviousPlaybackReleased();
            delay = Mathf.Max(0f, delay);
            BeforePlayImpl();

            Sequence effect = BuildSequence(duration);
            if (!effect.isAlive && delay == 0f)
            {
                Finish();
                return;
            }

            _sequence = Sequence.Create(useUnscaledTime: UseUnscaledTime);
            _sequence.timeScale = _timeScale;
            _state    = delay > 0f ? JChoreographyState.Waiting : JChoreographyState.Running;

            if (delay > 0f) { _sequence.ChainDelay(delay); }

            if (effect.isAlive)
            {
                _sequence.ChainCallback(this, choreography => choreography._state = JChoreographyState.Running);
                _sequence.Chain(effect);
            }
            _sequence.ChainCallback(this, choreography => choreography.Finish());
        }

        /// <summary>Interrupt playback, preserving the current visual pose.</summary>
        [Button]
        public void Stop()
        {
            _sequence.Stop();
            _state = JChoreographyState.NotStarted;
            StopImpl();
        }

        /// <summary>Finish immediately. Also applies the final pose if stopped or not yet played.</summary>
        [Button]
        public void Complete()
        {
            if (IsDone) { return; }

            // Seeking a sequence to its end also fires pending sound/SFX callbacks.
            // Each choreography already defines its final pose in CompleteImpl.
            _sequence.Stop();
            Finish();
        }

        private void Finish()
        {
            CompleteImpl();
            _state = JChoreographyState.Done;
        }

        private void EnsurePreviousPlaybackReleased()
        {
            if (_sequence.isAlive)
            {
                throw new
                    InvalidOperationException($"{name}: call Complete() or Stop() before replaying this choreography.");
            }
        }

        protected abstract void PrepareImpl();

        protected virtual void BeforePlayImpl() {}

        protected virtual void StopImpl() {}

        /// <summary>Apply the final visual pose without replaying sound or particle cues.</summary>
        protected abstract void CompleteImpl();

        /// <summary>Build the effect without its initial playback delay. Return default when already at the end.</summary>
        protected abstract Sequence BuildSequence(float duration);
    }
}
