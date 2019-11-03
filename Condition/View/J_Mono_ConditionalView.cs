using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Conditions.View
{
    public sealed class J_Mono_ConditionalView : MonoBehaviour
    {
        // --------------- VALUES AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), SerializeField] private bool _activeWhenTrue;
        [BoxGroup("Setup", true, true), SerializeField, Required] private J_Mono_ViewActivator _view;
        private J_Mono_ViewActivator ThisView
        {
            get
            {
                if (_view == null) _view = GetComponent<J_Mono_ViewActivator>();
                return _view;
            }
        }

        //use a multi condition if it has multiple values
        [BoxGroup("Controls", true, true), SerializeField] private J_ReactiveCondition _condition;

        //to check the activation of this element
        private bool _isActive;
        private bool IsActive
        {
            get => _isActive;
            set
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

        private void SanityChecks() => Assert.IsNotNull(_condition, $"{gameObject.name} requires a {nameof(_condition)}");

        // --------------- CHECK --------------- //
        private void CheckCondition(bool active) => IsActive = active == _activeWhenTrue;

        // --------------- LISTENERS --------------- //
        private void OnEnable()
        {
            _isActive = ThisView.IsActive;
            _condition.Subscribe(CheckCondition);
            CheckCondition(_condition.Current);
        }

        private void OnDisable() => _condition.UnSubscribe(CheckCondition);
    }
}
