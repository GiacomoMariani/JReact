using JReact.TimeProgress;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Environment.SkyLight
{
    /// <summary>
    /// Drives a J_SkyLight through one full cycle. Progression 0..1 samples four inspector-authored
    /// curves (direction x/y, shadow multiplier, intensity) and feeds the light via SetLight, quantized
    /// into _cycleTurnAmount turns (at most one publish per turn). A static scene simply has no mover:
    /// author the start values on J_SkyLight instead.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class J_SkyLightMover : MonoBehaviour
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_SkyLight _skyLight;
        [BoxGroup("Setup", true, true, 0), SerializeField] private AnimationCurve _horizontalDirectionCycle;
        [BoxGroup("Setup", true, true, 0), SerializeField] private AnimationCurve _verticalDirectionCycle;
        [BoxGroup("Setup", true, true, 0), SerializeField] private AnimationCurve _shadowMultiplierCycle;
        [BoxGroup("Setup", true, true, 0), SerializeField] private AnimationCurve _intensityCycle;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0f)] private float _cyclesPerSecond = 0.01f;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(2)]
        [Tooltip("Number of published light changes per cycle. Use an even value to include progression 0.5 exactly.")]
        private int _cycleTurnAmount = 100;
        [BoxGroup("Setup", true, true, 0), SerializeField, Range(0f, 1f)] private float _startProgression;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _loop = true;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float Progression { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int CycleTurn { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int CompletedCycles { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsPlaying { get; private set; }

        /// <summary>Seconds between published turns; 0 when the cycle cannot advance.</summary>
        public float TurnInterval => _cyclesPerSecond > 0f ? 1f / (_cyclesPerSecond * _cycleTurnAmount) : 0f;

        private float _continuousProgression;

        // --------------- CYCLE --------------- //
        private void ApplyProgression(float progression)
        {
            Progression = Mathf.Clamp01(progression);

            Vector2 direction = new Vector2(_horizontalDirectionCycle.Evaluate(Progression),
                                            _verticalDirectionCycle.Evaluate(Progression));
            float brightness       = _intensityCycle.Evaluate(Progression);
            float shadowMultiplier = _shadowMultiplierCycle.Evaluate(Progression);

            _skyLight.SetTransitionInterval(TurnInterval);
            _skyLight.SetLight(direction, brightness, shadowMultiplier);
        }

        private int TurnFor(float progression)
        {
            if (progression >= 1f) { return _cycleTurnAmount; }

            return Mathf.Clamp(Mathf.FloorToInt(progression * _cycleTurnAmount), 0, _cycleTurnAmount - 1);
        }

        private void ResetProgression()
        {
            _cycleTurnAmount       = Mathf.Max(2, _cycleTurnAmount);
            _continuousProgression = Mathf.Clamp01(_startProgression);
            CycleTurn              = TurnFor(_continuousProgression);
            CompletedCycles        = 0;
            ApplyProgression(_continuousProgression);
        }

        private void Update()
        {
            if (!IsPlaying ||
                _cyclesPerSecond <= 0f) { return; }

            float progressionDelta = GetDeltaTime() * _cyclesPerSecond;
            if (progressionDelta <= 0f) { return; }

            float nextProgression = _continuousProgression + progressionDelta;
            bool  completedCycle  = false;

            if (_loop && nextProgression >= 1f)
            {
                int cyclesPassed = Mathf.FloorToInt(nextProgression);
                CompletedCycles += cyclesPassed;
                nextProgression =  Mathf.Repeat(nextProgression, 1f);
                completedCycle  =  true;
            }
            else if (!_loop &&
                     nextProgression >= 1f)
            {
                nextProgression = 1f;
                IsPlaying       = false;
            }

            _continuousProgression = nextProgression;
            int nextTurn = TurnFor(nextProgression);
            if (!completedCycle &&
                nextTurn == CycleTurn) { return; }

            CycleTurn = nextTurn;
            float turnProgression = nextTurn / (float)_cycleTurnAmount;
            ApplyProgression(turnProgression);
        }

        private static float GetDeltaTime() => JTime.DeltaTime;

        // --------------- LIFECYCLE --------------- //
        private void OnEnable()
        {
            ResetProgression();
            IsPlaying = true;
        }

        private void OnDisable() => IsPlaying = false;

        [ButtonGroup("Playback"), Button("Play")]  private void Play()  => IsPlaying = true;
        [ButtonGroup("Playback"), Button("Pause")] private void Pause() => IsPlaying = false;

        [ButtonGroup("Playback"), Button("Restart")]
        private void Restart()
        {
            ResetProgression();
            IsPlaying = true;
        }

        private void OnValidate()
        {
            if (_skyLight == null) { _skyLight = GetComponent<J_SkyLight>(); }
        }
    }
}
