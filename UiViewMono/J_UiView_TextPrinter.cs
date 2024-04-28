using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.UiView
{
    public sealed class J_UiView_TextPrinter : J_UiView_Text
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private const string EmptyString = "";

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField, Range(0.01f, 0.5f)] private float _secondsPerCharacter = 0.1f;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private string _currentText;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool IsPrinting => _coroutine.IsRunning;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private CoroutineHandle _coroutine;

        // --------------- MESSAGE COMMANDS --------------- //
        /// <summary>
        /// sets a new speed for the print effect
        /// </summary>
        public void UpdateSpeed(float secondsPerCharacter) => _secondsPerCharacter = secondsPerCharacter;

        /// <summary>
        /// print one message on char at a time, on the ui
        /// </summary>
        /// <param name="message"></param>
        public void PrintMessage(string message)
        {
            if (IsPrinting) StopThis();
            //store the current message in case we want to fast finish
            _currentText = message;
            _coroutine   = Timing.RunCoroutine(PrintStepByStep(message), Segment.FixedUpdate);
        }

        /// <summary>
        /// used to fast finish to print the message
        /// </summary>
        public void FastFinish()
        {
            if (!IsPrinting)
            {
                JLog.Warning($"{gameObject.name} is not printing. Cancel fast finish.");
                return;
            }

            StopThis();
            SetFinalText();
        }

        private void SetFinalText()
        {
            SetText(_currentText);
            _currentText = EmptyString;
        }

        // --------------- PRINT IMPLEMENTATION --------------- //
        private IEnumerator<float> PrintStepByStep(string messageToPrint)
        {
            for (int i = 0; i < messageToPrint.Length; i++)
            {
                SetText(messageToPrint.Substring(0, i));
                yield return Timing.WaitForSeconds(_secondsPerCharacter);
            }

            SetFinalText();
        }

        // --------------- RESET & DISABLE --------------- //
        private void OnDisable() => StopThis();

        private void StopThis() { Timing.KillCoroutines(_coroutine); }
    }
}
