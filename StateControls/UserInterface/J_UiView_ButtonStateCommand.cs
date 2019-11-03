using JReact.UiView;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.StateControl
{
    //this button will bring player to the desired state
    [RequireComponent(typeof(Button))]
    public sealed class J_UiView_ButtonStateCommand : J_UiView_ConditionalButton
    {
        [BoxGroup("State Control", true, true), SerializeField, AssetsOnly, Required]
        private J_SimpleStateControl _mainStateControl;
        [BoxGroup("State Control", true, true), SerializeField, AssetsOnly, Required]
        private J_State _desiredState;

        //caching components at initialization
        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_mainStateControl, $"This object ({gameObject}) needs an element for the value _mainStateControl");
        }

        //the command sent by this button
        protected override void ButtonCommand()
        {
            base.ButtonCommand();
            _mainStateControl.SetNewState(_desiredState);
        }

        //the button cannot be used if the player is alread in this state
        protected override bool CheckFurtherConditions() => _mainStateControl.CurrentState != _desiredState;
    }
}
