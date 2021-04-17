using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl
{
    public class J_StateTracker<T> : J_Service
        where T : J_State
    {
        // --------------- VALUES AND PROPERTIES --------------- //
        private J_StateControl<T> _defaultControls;
        protected virtual J_StateControl<T> _stateControl => _defaultControls;
        [BoxGroup("Setup", true, true), SerializeField] private int _maxStatesToTrack = 5;

        //used to get the previous state
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public T PreviousState
        {
            get
            {
                if (_previousStates.Count == 0) return null;
                return _previousStates[_previousStates.Count - 1];
            }
        }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<T> _previousStates = new List<T>();

        // --------------- INITIALIZATION AND LISTENERS --------------- //
        public static J_StateTracker<T> CreateInstanceFor(J_StateControl<T> controls)
        {
            var tracker = ScriptableObject.CreateInstance<J_StateTracker<T>>();
            tracker._defaultControls = controls;
            return tracker;
        }

        public void Inject(J_StateControl<T> controls)
        {
            if (IsActive) { End(); }

            _defaultControls = controls;
        }

        protected override void ActivateThis()
        {
            base.ActivateThis();
            SanityChecks();
            _stateControl.Subscribe(ChangeState);
        }

        private void SanityChecks() { Assert.IsNotNull(_stateControl, $"{name} needs a state control."); }

        protected override void EndThis()
        {
            _previousStates.Clear();
            _stateControl.UnSubscribe(ChangeState);
            base.EndThis();
        }

        // --------------- STATE CHANGE PROCESSING --------------- //
        private void ChangeState((T previous, T current) transition) { AppendToPrevious(transition.previous); }

        private void AppendToPrevious(T oldState)
        {
            //if we reached the max states remove the first
            if (_previousStates.Count >= _maxStatesToTrack) { _previousStates.RemoveAt(0); }

            _previousStates.Add(oldState);
        }

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// moves the state control to the previous state
        /// </summary>
        public void GoToPreviousState()
        {
            if (NoPreviousStates()) return;

            JLog.Log($"{name} resets {_stateControl.name} to {PreviousState}", JLogTags.State, this);
            _stateControl.SetNewState(PreviousState);
            _previousStates.RemoveAt(_previousStates.Count - 1);
        }

        //a safecheck to avoid calling this without previous states
        private bool NoPreviousStates()
        {
            if (_previousStates.Count > 0) { return false; }

            JLog.Warning($"Currently there are no previous states on {name}. Aborting command.", JLogTags.State, this);
            return true;
        }
    }
}
