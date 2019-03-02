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
    public class J_Mono_PermanentMessageControl : J_Mono_ActorElement<JMessage>
    {
        //the coroutine tag for the message display
        private const string COROUTINE_MessageChangerTag = "COROUTINE_MessageChangerTag";

        [BoxGroup("Setup", true, true, 0), SerializeField, Range(0.05f, 1f)] private float _secondsOfPause = .1f;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private GameObject[] _views;
        [BoxGroup("Setup", true, true, 8), SerializeField, Required] private J_Mono_MessagePrinter _printer;

        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Event _checkNextEvent;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Event _resetPrinterEvent;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Event _onMessageComplete;

        #region INITIALIZATION
        private void Awake() { SanityChecks(); }
        private void SanityChecks() { Assert.IsNotNull(_printer, $"({gameObject.name}) requires a _printer"); }
        #endregion

        #region UPDATE
        protected override void ActorUpdate(JMessage message)
        {
            //ignore null messages
            if (string.IsNullOrEmpty(message.MessageContent))
            {
                ShowViews(false);
                return;
            }

            //show next message after a small blink
            Timing.RunCoroutine(BlinkThenNextMessage(), Segment.FixedUpdate, GetInstanceID(), COROUTINE_MessageChangerTag);
        }
        #endregion

        #region MAIN COMMANDS
        //show or hide the views
        private void ShowViews(bool isActive)
        {
            for (int i = 0; i < _views.Length; i++)
                _views[i].SetActive(isActive);
        }

        //finish printing or close this 
        private void TryCheckNext()
        {
            if (_printer.IsPrinting)
            {
                _printer.FastComplete();
                return;
            }

            ShowViews(false);
            _onMessageComplete.RaiseEvent();
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
        #endregion

        #region LISTENERS
        protected override void OnEnable()
        {
            base.OnEnable();
            _checkNextEvent.Subscribe(TryCheckNext);
            _resetPrinterEvent.Subscribe(ResetPrinter);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _checkNextEvent.UnSubscribe(TryCheckNext);
            _resetPrinterEvent.UnSubscribe(ResetPrinter);
        }
        #endregion
    }
}
