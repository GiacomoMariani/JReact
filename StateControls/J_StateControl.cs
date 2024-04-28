/*
 * STRATEGY:
 * This scriptable object acts as a state machine, and offers an architecture that relies on Dependency
 * Injection and Observer pattern.
 *
 * COMMANDS AND REGISTERS
 * It receives state changes through the SetNewState method that offers the basics for the Command pattern.
 * It can be injected to scripts that require to follow state changes that may register/subscribe to OnStateTransition
 *
 * SANITY CHECKS
 * This script keeps checking if the commands are legit, using unity built in assertions.
 *
 * SIDE NOTE
 * This script is using Odin Inspector for better visualization on the unity editor.
 */

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl
{
    public class J_StateControl<T> : J_Service, jObservable<(T previous, T current)>
        where T : J_State
    {
        // --------------- MAIN EVENTS AND DELEGATES --------------- //
        private event Action<(T previous, T current)> OnStateTransition;

        // --------------- VALID STATES --------------- //
        /* These are used just a sanity check, to make sure we are implementing the correct states */
        [Title("Setup", "Starts with this state"), BoxGroup("Setup", true, true), SerializeField, AssetsOnly,
         Required]
        protected T _firstState;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] protected T[] _validStates;

        // --------------- CURRENT SITUATION --------------- //
        /* The following items are used to track the current situation */

        [Title("State", "The current situation"), FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        public T CurrentState { get; private set; }

        // --------------- INSTANTIATION --------------- //
        /// <summary>
        /// Creates a new instance of J_StateControl with the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of states.</typeparam>
        /// <param name="states">An array containing the valid states.</param>
        /// <param name="firstState">The first state to set.</param>
        /// <param name="initialize">Indicates if the state control should be initialized immediately.</param>
        /// <returns>A new instance of J_StateControl.</returns>
        public static J_StateControl<T> Create(T[] states, T firstState, bool initialize = true)
        {
            var stateControl = CreateInstance<J_StateControl<T>>();
            stateControl._validStates = states;
            stateControl._firstState  = firstState;
            if (initialize) { stateControl.ActivateThis(); }

            return stateControl;
        }

        /// <summary>
        /// Creates a new instance of <see cref="J_StateControl{T}"/> using a template <see cref="J_StateControl{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the states.</typeparam>
        /// <param name="template">The template <see cref="J_StateControl{T}"/> from which to create the new instance.</param>
        /// <param name="initialize">Specifies whether to initialize the new instance.</param>
        /// <returns>A new instance of <see cref="J_StateControl{T}"/> created from the template.</returns>
        public static J_StateControl<T> FromTemplate(J_StateControl<T> template, bool initialize = true)
        {
            int length                                 = template._validStates.Length;
            var states                                 = new T[length];
            for (int i = 0; i < length; i++) states[i] = J_State.Copy(template._validStates[i]);

            T firstState = J_State.Copy(template._firstState);

            return Create(states, firstState, initialize);
        }

        // --------------- ACTIVATION --------------- //
        // sets the first state of the game
        protected override void ActivateThis()
        {
            Assert.IsNotNull(_firstState, $"{name} Please set a first state.");
            CurrentState = _firstState;
            CurrentState.Activate();
            OnStateTransition?.Invoke((null, CurrentState));
            JLog.Log($"{name} sets first state: {CurrentState}. ({_validStates.Length} states) ", JLogTags.State, this);
            base.ActivateThis();
        }

        protected override void EndThis()
        {
            if (CurrentState != null) { CurrentState.End(); }

            CurrentState = null;
            base.EndThis();
        }

        // --------------- MAIN CONTROLS --------------- //
        /// <summary>
        /// the main command of this class. This is used to change the state into another most of the
        /// logic will be handled by the property CurrentState
        /// </summary>
        /// <param name="stateToSet">the state we want to set</param>
        public void SetNewState(T stateToSet)
        {
            // --------------- PRE COMMAND CHECKS --------------- //
            if (!ValidState(stateToSet)) return;
            if (StateAlreadySet(stateToSet)) return;

            JLog.Log($"{name} from {CurrentState.name} to {stateToSet.name}.", JLogTags.State, this);

            // --------------- EXIT EVENT --------------- //
            //send exit event of the previous event
            Assert.IsNotNull(CurrentState, $"{name} cannot exit from a null state.");
            CurrentState.End();

            // --------------- VALUE SET --------------- //
            T previous = CurrentState;
            CurrentState = stateToSet;

            // --------------- ENTER EVENTS --------------- //
            CurrentState.Activate();
            OnStateTransition?.Invoke((previous, stateToSet));
        }

        //to make sure the state is valid
        private bool ValidState(T stateToSet)
        {
            if (stateToSet == null)
            {
                JLog.Error($"{name} null state is not valid.", JLogTags.State, this);
                return false;
            }

            if (!_validStates.ArrayContains(stateToSet))
            {
                JLog.Error($"{name} - The state {stateToSet} is not a valid state.\nValid: {_validStates.PrintAll()}.",
                           JLogTags.State, this);

                return false;
            }

            return true;
        }

        //to avoid setting the same state again
        private bool StateAlreadySet(T stateToSet)
        {
            if (stateToSet != CurrentState) { return false; }

            JLog.Warning($"{name} - {stateToSet.name} is already the current state", JLogTags.State, this);

            return true;
        }

        // --------------- SUBSCRIBE METHODS --------------- //
        //the following methods are used to subscribe/register to the transition event. they act like the observer pattern
        public void Subscribe(Action<(T previous, T current)> action) { OnStateTransition += action; }

        public void UnSubscribe(Action<(T previous, T current)> action) { OnStateTransition -= action; }
    }
}
