using System;
using JReact.StateControl;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Conditions
{
    /// <summary>
    /// a condition true when a state is active
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Conditions/State")]
    public class J_StateCondition : J_ReactiveCondition
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), SerializeField] private bool _passOnEnter = true;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_State[] _validStates;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_SimpleStateControl _stateControls;

        // --------------- INITIALIZE AND RESET --------------- //
        protected override void StartCheckingCondition()
        {
            StateChange((null, _stateControls.CurrentState));
            _stateControls.Subscribe(StateChange);
        }

        protected override void StopCheckingCondition() { _stateControls.UnSubscribe(StateChange); }

        // --------------- IMPLEMENTATION --------------- //
        private void StateChange((J_State previousState, J_State nextState) states)
        {
            bool stateValid = Array.IndexOf(_validStates, states.nextState) > -1;

            if (_passOnEnter) Current = stateValid;
            else Current              = !stateValid;
        }
    }
}
