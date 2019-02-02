﻿/*
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

using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControls
{
    /// <summary>
    /// This class implements a state machine sending event to be tracked by other scripts using 
    /// Dependency Injection (we may inject just the desired state or this entire state control)
    /// EXAMPLES OF USAGE: menu flow and transitions, weather system, simple artificial intelligence,
    /// but also friend invitation or any other element that may be tracked with a state machine
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Game States/State Control")]
    public class J_StateControl : J_AsbtractStateControl<J_State>
    {
    }

    public class J_AsbtractStateControl<T> : ScriptableObject, iActivable
        where T : J_State
    {
        #region FIELDS AND PROPERTIES
        // --------------- MAIN EVENTS AND DELEGATES --------------- //
        //this is the event that tracks the transition from one state to one other
        public delegate void StateTransition(T previousState, T nextState);
        private event StateTransition OnStateTransition;

        // --------------- VALID STATES --------------- //
        /* These are used just a sanity check, to make sure we are implementing the correct states */

        //a list of the valid states
        [Title("Setup", "The elements required to setup this control")]
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required]
        private T _firstState;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] protected T[] _validStates;

        // --------------- CURRENT SITUATION --------------- //
        /* The following items are used to track the current situation */

        [Title("State", "The current situation")]
        //the current state, with an available getter, to make sure anyone can check it
        //this property is also used to send the main events
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private T _currentState;
        public T CurrentState
        {
            get => _currentState;
            private set
            {
                // --------------- EXIT EVENT --------------- //
                //send exit event of the previous event
                Assert.IsNotNull(CurrentState, $"Trying to exit from a null state from {name}.");
                CurrentState.RaiseExitEvent();

                // --------------- VALUE SET --------------- //
                //set the value and raise the event of the next state
                Assert.IsNotNull(value, $"We're trying to set a null state on {name}.");
                _currentState = value;

                // --------------- ENTER EVENT--------------- //
                CurrentState.RaiseEvent();
            }
        }
        
        //check if this state control has been initialized
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsActive { get; private set; } = false;
        #endregion FIELDS AND PROPERTIES

        #region INITIALIZATION
        /// <summary>
        /// sets the first state of the game
        /// </summary>
        public virtual void Initialize()
        {
            Assert.IsNotNull(_firstState, $"Please set a first state to validate the controls on: {name}");
            SetStateSanityChecks(_firstState);
            _currentState = _firstState;
            _firstState.RaiseEvent();
            IsActive = true;
            JConsole.Log($"Initialization completed on {name} with {_validStates.Length} states.", JLogTags.State, this);
        }
        #endregion INITIALIZATION

        #region MAIN CONTROL
        /// <summary>
        /// the main command of this class. This is used to change the state into another most of the
        /// logic will be handled by the property CurrentState
        /// </summary>
        /// <param name="stateToSet">the state we want to set</param>
        public void SetNewState(T stateToSet)
        {
            // --------------- PRE COMMAND CHECKS --------------- //
            if (StateAlreadySet(stateToSet)) return;
            SetStateSanityChecks(stateToSet);

            JConsole.Log($"{name} from {(CurrentState != null ? CurrentState.name : "null")} to {stateToSet.name}.",
                         JLogTags.State, this);

            // --------------- COMMAND PROCESSING --------------- //
            CurrentState = stateToSet;
            OnStateTransition?.Invoke(CurrentState, stateToSet);
        }

        private void SetStateSanityChecks(T stateToSet)
        {
            Assert.IsTrue(Array.IndexOf(_validStates, stateToSet) > -1,
                          $"The state {stateToSet} is not in the of valid states of {name}. List{_validStates.PrintAll()}.");
        }

        //to avoid setting the same state again
        private bool StateAlreadySet(T stateToSet)
        {
            if (stateToSet == CurrentState)
            {
                JConsole.Warning($"{name} wants to set {stateToSet.name}, but it is already the current state",
                                 JLogTags.State, this);
                return true;
            }

            return false;
        }
        #endregion MAIN CONTROL

        #region SUBSCRIBE METHODS
        /// <summary>
        /// the following methods are used to subscribe/register to the transition event. they act like the observer pattern
        /// </summary>
        /// <param name="actionToSend">the action/handler to be sent at the event</param>
        public void Subscribe(StateTransition actionToSend)
        {
            if (!IsActive) Initialize();
            OnStateTransition += actionToSend;
        }

        public void UnSubscribe(StateTransition actionToSend) { OnStateTransition -= actionToSend; }
        #endregion SUBSCRIBE METHODS

        #region DISABLE AND RESET
        /// <summary>
        /// reset methods set OnDisable, as for Unity Scriptable objects
        /// </summary>
        protected virtual void OnDisable() { ResetThis(); }

        /// <summary>
        /// we reset the initialization and we remove the current state using the field instead of the property
        /// </summary>
        public virtual void ResetThis()
        {
            _currentState = null;
            IsActive      = false;
        }
        #endregion DISABLE AND RESET
    }
}
