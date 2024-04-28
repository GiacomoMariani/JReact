using System;
using JReact;
using JReact.Singleton;
using Sirenix.OdinInspector;

namespace MWC.StateControls
{
    public class J_MonoS_SimpleStateControls<T> : J_MonoSingleton<J_MonoS_SimpleStateControls<T>>
    where T : Enum, IEquatable<T>
    {
        // --------------- EVENTS --------------- //
        public event Action<(T previous, T current)> OnStateChange;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private T _current;
        
        /// <summary>
        /// used to get and check the current scene
        /// </summary>
        public T Current
        {
            get => _current;
            private set
            {
                var previous = _current;
                _current = value;
                JLog.Log($"{name} - state change: {previous} => {_current}", JLogTags.State, this);
                OnStateChange?.Invoke((previous, _current));
            }
        }

        /// <summary>
        /// sets the new state
        /// </summary>
        /// <param name="state"></param>
        public void SetNewState(T state)
        {
            if (!ValidState(state)) { return; }

            Current = state;
        }

        /// <summary>
        /// here we set the general validation logic to see if a state is valid
        /// specific validation logic will go to the specific implementation
        /// </summary>
        /// <param name="state">the state we want to set</param>
        /// <returns>returns true if the state is valid</returns>
        private bool ValidState(T state)
        {
            //avoid setting the same state
            if (state.Equals(_current))
            {
                JLog.Warning($"{name} cannot set {state}.", JLogTags.State, this);
                return false;
            }

            return true;
        }
    }
}
