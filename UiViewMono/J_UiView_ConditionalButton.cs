using JReact.Conditions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.UiView
{
    /// <summary>
    /// a button that can have conditions to be used
    /// </summary>
    public class J_UiView_ConditionalButton : J_UiView_ButtonElement
    {
        // --------------- FIELDS AND PROPERTIEES --------------- //
        [BoxGroup("Setup", true, true), SerializeField] protected JUnityEvent _unityEventToSend;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ReactiveCondition[] _commandConditions;
        [BoxGroup("Setup", true, true), SerializeField] private bool _showDisabledButton = true;

        // --------------- COMMAND --------------- //
        protected override void ButtonCommand()
        {
            //STEP 1 - check if all save conditions are met
            if (!ConditionsMet()) return;
            //STEP 2 - sends the event
            _unityEventToSend.Invoke();
        }

        // --------------- CONDITION --------------- //
        private bool ConditionsMet()
        {
            for (int i = 0; i < _commandConditions.Length; i++)
            {
                Assert.IsNotNull(_commandConditions[i], $"The save condition on {name} at index {i} is null");
                if (_commandConditions[i].Current) continue;

                JLog.Log($"{name} - command stop. Condition not met: {_commandConditions[i].name}", JLogTags.UiView, this);
                return false;
            }

            return CheckFurtherConditions();
        }

        //used to implement further conditions if required
        protected virtual bool CheckFurtherConditions() => true;

        // --------------- LISTENERS & INTERACTION --------------- //
        //sent when any of the conditions changed
        private void UpdateInteractability(bool item) => CheckInteraction();
        protected void CheckInteraction()               => _button.interactable = ConditionsMet();

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!_showDisabledButton) return;
            CheckInteraction();
            _commandConditions.SubscribeToAll(UpdateInteractability);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _commandConditions.UnSubscribeToAll(UpdateInteractability);
        }
    }
}
