using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Pool.Roamer
{
    /// <summary>
    /// a wind with a 2d force that might change at intervals
    /// </summary>
    public sealed class J_Wind : MonoBehaviour, jObservable<Vector2>
    {
        // --------------- EVENT AND CONSTANT --------------- //
        private event Action<Vector2> OnWindChange;

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField] private bool _randomWind = true;
        private bool _notRandomWind => !_randomWind;
        // --------------- STATIC WIND --------------- //
        [ShowIf(nameof(_notRandomWind)), BoxGroup("Setup", true, true), SerializeField]
        private Vector2 _desiredSpeed = new Vector2(0.5f, 5f);

        // --------------- RANDOM WIND --------------- //
        //the min and max values for the velocity, the first value
        [ShowIf(nameof(_notRandomWind)), BoxGroup("Setup", true, true), SerializeField]
        private Vector2 _horizontalForceRange = new Vector2(-20f, 20f);
        [ShowIf(nameof(_notRandomWind)), BoxGroup("Setup", true, true), SerializeField]
        private Vector2 _verticalForceRange = new Vector2(-20f, 20f);

        // --------------- CHANGING WIND --------------- //
        [ShowIf(nameof(_randomWind)), BoxGroup(        "Setup", true, true), SerializeField] private bool _windChangeOverTime = true;
        [ShowIf(nameof(_windChangeOverTime)), BoxGroup("Setup", true, true), SerializeField]
        private Vector2 _secondsBeforeChange = new Vector2(0f, 15f);
        [ShowIf(nameof(_windChangeOverTime)), BoxGroup("Setup", true, true), SerializeField]
        private bool _additiveChange;

        // --------------- STATE AND BOOKKEEPING --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Vector2 _windSpeed = new Vector2(5f, 50f);
        public Vector2 WindSpeed => _windSpeed;

        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] private CoroutineHandle _coroutine;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public Direction CurrentDirection
            => _windSpeed.GetDirection();

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// winds start flowing
        /// </summary>
        public void StartWind()
        {
            if (_randomWind) { _coroutine = Timing.RunCoroutine(WindChanger(), Segment.LateUpdate); }
            else SetFixedSpeed();
        }

        /// <summary>
        /// wind stops flowing
        /// </summary>
        public void StopWind()
        {
            Timing.KillCoroutines(_coroutine);
            //remove the speed
            _windSpeed.x = 0;
            _windSpeed.y = 0;
            OnWindChange?.Invoke(WindSpeed);
        }

        // --------------- WIND CONTROLS --------------- //
        public void SetFixedSpeed()
        {
            _windSpeed = _desiredSpeed;
            OnWindChange?.Invoke(WindSpeed);
        }

        private void SetRandomSpeed()
        {
            // --------------- CALCULATING ALL DIRECTION SPEED --------------- //
            //horizontal (additive adds the value instead of just setting it)
            if (_additiveChange) _windSpeed.x += _horizontalForceRange.GetRandomValue();
            else _windSpeed.x                 =  _horizontalForceRange.GetRandomValue();

            //vertical (additive adds the value instead of just setting it)
            if (_additiveChange) _windSpeed.y += _verticalForceRange.GetRandomValue();
            else _windSpeed.y                 =  _verticalForceRange.GetRandomValue();

            //event 
            OnWindChange?.Invoke(WindSpeed);
        }

        //runs the wind
        private IEnumerator<float> WindChanger()
        {
            SetRandomSpeed();
            while (true)
            {
                yield return Timing.WaitForSeconds(_secondsBeforeChange.GetRandomValue());
                if (_windChangeOverTime) { SetRandomSpeed(); }
            }
        }

        // --------------- SUBSCRIBERS --------------- //
        public void Subscribe(Action<Vector2>   actionToAdd)    { OnWindChange += actionToAdd; }
        public void UnSubscribe(Action<Vector2> actionToRemove) { OnWindChange -= actionToRemove; }
    }
}
