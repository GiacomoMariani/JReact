using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.ScreenMessage
{
    /// <summary>
    /// finish printing if we were printing
    /// </summary>
    public sealed class J_Mono_PermanentMessageControl : J_Mono_ActorItem<JMessage>
    {
        //the coroutine tag for the message display
        [BoxGroup("Setup", true, true), SerializeField, Range(0.05f, 1f)] private float _secondsOfPause = .1f;
        [BoxGroup("Setup", true, true), SerializeField, Required] private GameObject[] _views;
        [BoxGroup("Setup", true, true, 8), SerializeField, Required] private J_Mono_MessagePrinter _printer;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private CoroutineHandle _coroutine;
        
        // --------------- INITIALIZATION --------------- //
        private void Awake() { SanityChecks(); }

        private void SanityChecks() { Assert.IsNotNull(_printer, $"({gameObject.name}) requires a {nameof(_printer)}"); }

        // --------------- UPDATE --------------- //
        protected override void ActorUpdate(JMessage message)
        {
            //ignore null messages
            if (string.IsNullOrEmpty(message.Content))
            {
                ShowViews(false);
                return;
            }

            //show next message after a small blink
            _coroutine = Timing.RunCoroutine(BlinkThenNextMessage(), Segment.FixedUpdate);
        }

        // --------------- MAIN COMMANDS --------------- //
        //show or hide the views
        private void ShowViews(bool isActive) { _views.ActivateAll(isActive); }

        //finish printing or close this 
        private void TryCheckNext()
        {
            if (_printer.IsPrinting)
            {
                _printer.FastComplete();
                return;
            }

            ShowViews(false);
            TryCheckNext();
        }

        //a short blink before the next message
        private IEnumerator<float> BlinkThenNextMessage()
        {
            ShowViews(false);
            yield return Timing.WaitForSeconds(_secondsOfPause);
            ShowViews(true);
        }

        //hide all when the printer resets
        private void ResetPrinter()
        {
            ShowViews(false);
            _printer.ResetThis();
        }
    }
}
