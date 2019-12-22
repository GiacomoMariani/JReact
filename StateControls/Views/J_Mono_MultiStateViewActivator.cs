using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl
{
    /// <summary>
    /// used to change the menu based on the state
    /// </summary>
    public abstract class J_Mono_MultiStateViewActivator<T> : MonoBehaviour
    where T : J_State
    {
        // --------------- VALUES AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), SerializeField] private bool _activateWhenEnterState = true;
        //the views related to this element
        [BoxGroup("Setup", true, true), SerializeField, Required] private J_Mono_ViewActivator _view;
        private J_Mono_ViewActivator ThisView
        {
            get
            {
                if (_view == null) _view = GetComponent<J_Mono_ViewActivator>();
                return _view;
            }
        }

        //when we want to see this
        [BoxGroup("Controls", true, true), SerializeField] private T[] _validStates;

        protected abstract J_StateControl<T> _Controls { get; }

        //to check the activation of this element
        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            private set
            {
                //if we want to set the same value we ignore this
                if (_isActive == value) return;
                //otherwise we set the value
                _isActive = value;
                //then we call the desired method to open or close
                ThisView.ActivateView(_isActive);
            }
        }

        // --------------- INITIALIZATION --------------- //
        private void Awake() => SanityChecks();

        private void SanityChecks()
        {
            for (int i = 0; i < _validStates.Length; i++)
                Assert.IsNotNull(_validStates[i], $"{gameObject.name} has a missing state at index {i}");
        }

        // --------------- LISTENERS --------------- //
        private void StateChange((T previous, T current) transition)
        {
            IsActive = _validStates.ArrayContains(transition.current) == _activateWhenEnterState;
        }

        private void OnEnable()
        {
            _isActive = gameObject.activeSelf;
            if (_Controls.IsActive) StateChange((null, _Controls.CurrentState));
            else _Controls.Subscribe(CheckActivation);

            _Controls.Subscribe(StateChange);
        }

        private void CheckActivation()
        {
            _Controls.UnSubscribe(CheckActivation);
            StateChange((null, _Controls.CurrentState));
        }

        private void OnDisable() => _Controls.UnSubscribe(StateChange);
    }
}
