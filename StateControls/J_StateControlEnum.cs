using System;
using Sirenix.OdinInspector;

namespace JReact.StateControl
{
    public abstract class J_StateControlEnum<TEnum>
        where TEnum : Enum
    {
        // --------------- EVENTS --------------- //
        public event Action<(TEnum previous, TEnum current)> OnStateChange;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private TEnum _current;

        /// <summary>
        /// used to get and check the current scene
        /// </summary>
        public TEnum Current
        {
            get => _current;
            private set
            {
                var previous = _current;
                _current = value;
                OnStateChange?.Invoke((previous, _current));
            }
        }

        /// <summary>
        /// sets the new state
        /// </summary>
        /// <param name="state"></param>
        public void SetNewState(TEnum state)
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
        protected virtual bool ValidState(TEnum state) { return true; }
    }
}
