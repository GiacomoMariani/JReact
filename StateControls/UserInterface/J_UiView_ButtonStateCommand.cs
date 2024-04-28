using JReact.UiView;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.StateControl
{
    //this button will bring player to the desired state
    [RequireComponent(typeof(Button))]
    public abstract class J_UiView_ButtonStateCommand<T> : J_UiView_ConditionalButton
        where T : J_State
    {
        [BoxGroup("State Control", true, true), SerializeField, AssetsOnly, Required]
        protected T _desiredState;

        protected abstract J_StateControl<T> _Controls { get; }

        //caching components at initialization
        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            InitThis();
            SanityChecks();
        }

        private void InitThis() {}

        private void SanityChecks()
        {
            Assert.IsNotNull(_Controls,     $"{gameObject.name} requires a {nameof(_Controls)}");
            Assert.IsNotNull(_desiredState, $"{gameObject.name} requires a {nameof(_desiredState)}");
        }

        //the command sent by this button
        protected override void ButtonCommand()
        {
            base.ButtonCommand();
            _Controls.SetNewState(_desiredState);
        }

        //the button cannot be used if the player is already in this state
        protected override bool CheckFurtherConditions() => _Controls.CurrentState != _desiredState;

        //this happens when the state controls changes state
        private void StateChanged((T previous, T current) transition) => CheckInteraction();

        protected override void OnEnable()
        {
            base.OnEnable();
            _Controls.Subscribe(StateChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _Controls.UnSubscribe(StateChanged);
        }
    }
}
