using System;
using JReact.TimeProgress;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JReact.J_Input
{
    [Serializable]
    public struct JPointerClick
    {
        public const float DefaultClickDelay = 0.05f;

        [SerializeField, MinValue(0)] private float _secondsBeforeHold;
        [ReadOnly, ShowInInspector] private float _secondsPassedPressed;

        [ReadOnly, ShowInInspector] public bool IsStarted { get; private set; }
        [ReadOnly, ShowInInspector] public bool IsPerformed { get; private set; }
        [ReadOnly, ShowInInspector] public bool IsHold { get; private set; }
        [ReadOnly, ShowInInspector] public bool IsFastClick { get; private set; }

        public JPointerClick(float secondsBeforeHold = DefaultClickDelay) : this() => _secondsBeforeHold = secondsBeforeHold;

        /// <summary>
        /// Process the state of the pointer click.
        /// Note: This must be performed in the Update method.
        /// </summary>
        public JPointerClick ProcessPointer(InputAction pointerAction)
        {
            bool isPressed = pointerAction.IsPressed();
            IsStarted = pointerAction.WasPressedThisFrame();

            IsFastClick = false;
            switch (isPressed)
            {
                // --------------- HOLD: PRESSED NOW AND BEFORE --------------- //
                case true when IsPerformed:
                {
                    _secondsPassedPressed += JTime.DeltaTime;
                    if (_secondsPassedPressed >= _secondsBeforeHold) { IsHold = true; }

                    break;
                }
                // --------------- TAP: STOP PRESSING NOW --------------- //
                case false:
                {
                    //only case when fast click happens is when previously it was performed
                    if (IsPerformed && _secondsPassedPressed < _secondsBeforeHold) { IsFastClick = true; }

                    IsHold                = false;
                    _secondsPassedPressed = 0;
                    break;
                }
            }

            IsPerformed = isPressed;

            return this;
        }

        public override string ToString() => $"Click: {{ "                    +
                                             $"IsStarted = {IsStarted}, "     +
                                             $"IsPerformed = {IsPerformed}, " +
                                             $"IsHold = {IsHold}, "           +
                                             $"IsFastClick = {IsFastClick} }}";
    }
}
