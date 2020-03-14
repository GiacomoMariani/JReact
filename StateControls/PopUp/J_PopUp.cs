using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControl.PopUp
{
    /// <summary>
    /// a state that sends a pop up
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/PopUp/Reactive Pop Up")]
    public sealed class J_PopUp : J_State
    {
        // --------------- CONSTANTS --------------- //
        private const string DefaultConfirmText = "Confirm";
        private const string DefaultDenyText = "Cancel";

        // --------------- STATE - OPTIONAL --------------- //
        [InfoBox("Null => no connection with state"), BoxGroup("Setup", true, true), SerializeField, AssetsOnly]
        private J_SimpleStateControl _stateControl;

        // --------------- CONTENT --------------- //
        //J_Mono_ReactiveStringText might be used to display this
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveString _title;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveString _message;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveString _confirmButtonText;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveString _denyButtonText;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_State _previousState;

        // --------------- ACTIONS --------------- //        
        private JUnityEvent _confirm;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public JUnityEvent ConfirmAction
            => _confirm ?? (_confirm = new JUnityEvent());
        private JUnityEvent _deny;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public JUnityEvent CancelAction
            => _deny ?? (_deny = new JUnityEvent());

        // --------------- SETUP --------------- //
        public void SetupPopUpText(string message, string title = "")
        {
            _message.Current = message;
            _title.Current   = title;
        }

        public void SetupConfirmButton(Action confirmAction, string confirmText = DefaultConfirmText, bool exitStateAfter = true)
        {
            ConfirmAction.RemoveAllListeners();
            ConfirmAction.AddListener(confirmAction.Invoke);
            if (exitStateAfter) ConfirmAction.AddListener(Close);
            _confirmButtonText.Current = confirmText;
        }

        public void SetupDenyButton(Action denyAction, string confirmText = DefaultDenyText, bool exitStateAfter = true)
        {
            CancelAction.RemoveAllListeners();
            CancelAction.AddListener(denyAction.Invoke);
            if (exitStateAfter) CancelAction.AddListener(Close);
            _denyButtonText.Current = confirmText;
        }

        // --------------- OPEN AND CLOSE --------------- //
        public void Open()
        {
            if (_stateControl == null)
            {
                Activate();
            }
            else
            {
                _previousState = _stateControl.CurrentState;
                _stateControl.SetNewState(this);
            }
        }

        public void Close()
        {
            if (_stateControl == null) End();
            else if (_stateControl.CurrentState == this &&
                     _previousState             != null) _stateControl.SetNewState(_previousState);

            ResetThis();
        }

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// sends the confirm actions, can be attached to a button
        /// </summary>
        public void Confirm() => ConfirmAction.Invoke();

        /// <summary>
        /// sends the cancel actions, can be attached to a button
        /// </summary>
        public void Cancel() => CancelAction.Invoke();

        // --------------- RESET --------------- //
        public override void ResetThis()
        {
            base.ResetThis();
            ConfirmAction.RemoveAllListeners();
            CancelAction.RemoveAllListeners();
        }
    }
}
