using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl.PopUp
{
    public abstract class J_GenericPopup<T> : ScriptableObject
        where T : J_State
    {
        // --------------- CONSTANTS --------------- //
        private const string DefaultConfirmText = "Confirm";
        private const string DefaultDenyText = "Cancel";

        // --------------- STATE - OPTIONAL --------------- //
        protected abstract J_StateControl<T> _stateControl { get; }
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private T _popUpState;

        // --------------- CONTENT --------------- //
        //J_Mono_ReactiveStringText might be used to display this
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveString _title;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveString _message;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveString _confirmButtonText;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveString _denyButtonText;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private T _previousState;

        // --------------- ACTIONS --------------- //        
        private JUnityEvent _confirm;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public JUnityEvent ConfirmAction
            => _confirm ??= new JUnityEvent();
        private JUnityEvent _deny;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public JUnityEvent CancelAction
            => _deny ??= new JUnityEvent();

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
            if (exitStateAfter) { ConfirmAction.AddListener(Close); }

            _confirmButtonText.Current = confirmText;
        }

        public void SetupDenyButton(Action denyAction, string confirmText = DefaultDenyText, bool exitStateAfter = true)
        {
            CancelAction.RemoveAllListeners();
            CancelAction.AddListener(denyAction.Invoke);
            if (exitStateAfter) { CancelAction.AddListener(Close); }

            _denyButtonText.Current = confirmText;
        }

        // --------------- OPEN AND CLOSE --------------- //
        public void Open()
        {
            if (_stateControl == null) { return; }

            Assert.IsNotNull(_popUpState, $"{name} requires a {nameof(_popUpState)}");
            _previousState = _stateControl.CurrentState;
            _stateControl.SetNewState(_popUpState);
        }

        public void Close()
        {
            if (_stateControl              != null        &&
                _stateControl.CurrentState == _popUpState &&
                _previousState             != null) { _stateControl.SetNewState(_previousState); }

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
        public void ResetThis()
        {
            _popUpState.ResetThis();
            ConfirmAction.RemoveAllListeners();
            CancelAction.RemoveAllListeners();
        }
    }
}
