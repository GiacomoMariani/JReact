using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.ScreenMessage
{
    /// <summary>
    /// shows a message to be printed on the screen
    /// </summary>
    public sealed class J_Mono_MessagePrinter : J_Mono_ActorItem<JMessage>, iResettable
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private const string COROUTINE_PrinterTag = "COROUTINE_MessagePrinterTag";
        internal event Action<bool> OnPrinting;

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveString _stringMessage;
        [BoxGroup("Setup", true, true), SerializeField, Range(0.01f, 0.5f)] private float _secondsForType = 0.1f;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isPrinting;
        public bool IsPrinting
        {
            get => _isPrinting;
            private set
            {
                _isPrinting = value;
                if (OnPrinting != null) OnPrinting(value);
            }
        }

        // --------------- INITIALIZATION --------------- // 
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        private void InitThis() { this.InjectToChildren(); }

        private void SanityChecks() { Assert.IsNotNull(_stringMessage, $"({gameObject.name}) needs an element for _currentMessage"); }

        // --------------- OVERRIDES --------------- //
        protected override void ActorUpdate(JMessage message)
        {
            //reset to make sure it's ready, then print
            ResetThis();
            Timing.RunCoroutine(PrintCurrent(message), Segment.FixedUpdate, _actor.MessageNumber, COROUTINE_PrinterTag);
        }

        // --------------- PRINT IMPLEMENTATION --------------- //
        //print all the chars of the message
        private IEnumerator<float> PrintCurrent(JMessage message)
        {
            //store and validate the message
            string messageToPrint = message.Content;
            if (string.IsNullOrEmpty(messageToPrint)) yield break;
            //reset the print string
            _stringMessage.ResetThis();
            //starts printing, then add a char and wait for the next 
            IsPrinting = true;
            for (int i = 0; i < messageToPrint.Length; i++)
            {
                _stringMessage.Current += messageToPrint[i];
                yield return Timing.WaitForSeconds(_secondsForType);
            }

            //finish printing
            IsPrinting = false;
        }

        // --------------- MESSAGE COMMANDS --------------- //
        /// <summary>
        /// used to fast finish to print the message
        /// </summary>
        public void FastComplete()
        {
            Assert.IsTrue(IsPrinting, $"{name} should not call CompletePrinting if it is not printing");
            //reset this and show the entire element
            ResetThis();
            _stringMessage.Current = _actor.Content;
        }

        // --------------- RESET & DISABLE --------------- //
        protected override void OnDisable()
        {
            base.OnDisable();
            ResetThis();
        }

        public void ResetThis()
        {
            if (!IsPrinting) return;
            Timing.KillCoroutines(_actor.MessageNumber, COROUTINE_PrinterTag);
            _stringMessage.ResetThis();
            IsPrinting = false;
        }
    }
}
