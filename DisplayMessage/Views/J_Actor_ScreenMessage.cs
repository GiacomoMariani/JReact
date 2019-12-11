using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.ScreenMessage
{
    /// <summary>
    /// the actor to display a message
    /// </summary>
    public class J_Actor_ScreenMessage : J_Mono_Actor<JMessage>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] protected J_MessageSender _sender;
        [BoxGroup("Setup", true, true), SerializeField] private J_MessageId[] _desiredTypes;

        // --------------- INITIALIZATION --------------- //
        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_sender, $"{gameObject.name} requires a _messageControl");
        }

        // --------------- SENDER --------------- //
        private void TrySettingThisElement(JMessage messageSent)
        {
            //publish only desired messages
            if (_desiredTypes.ArrayContains(messageSent.MessageId)) ActorUpdate(messageSent);
        }

        // --------------- RESET AND LISTENERS --------------- //
        protected override void OnEnable()
        {
            base.OnEnable();
            _sender.Subscribe(TrySettingThisElement);
        }

        protected virtual void OnDisable() => _sender.UnSubscribe(TrySettingThisElement);
    }
}
