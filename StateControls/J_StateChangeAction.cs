using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControl
{
    /// <summary>
    /// a simple action used to change the state
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Game States/State Change Action")]
    public sealed class J_StateChangeAction : J_ProcessableAction
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("State Control", true, true), SerializeField, AssetsOnly, Required]
        private J_SimpleStateControl _stateControl;
        [BoxGroup("State Control", true, true), SerializeField, AssetsOnly, Required]
        private J_State _desiredState;

        /// <summary>
        /// sets the desired state
        /// </summary>
        public override void Process() => _stateControl.SetNewState(_desiredState);

        /// <summary>
        /// sets the desired state, with a given delay
        /// </summary>
        public void ProcessWithDelay(float delay)
        {
            if (delay <= 0f) _stateControl.SetNewState(_desiredState);
            else Timing.CallDelayed(delay, Process);
        }

        
    }
}
