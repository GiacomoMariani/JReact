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

        // --------------- INITIALIZATION --------------- //
        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_sender, $"{gameObject.name} requires a {nameof(_sender)}");
        }

        // --------------- SENDER --------------- //
        private void TryPublish(JMessage messageSent) { ActorUpdate(messageSent); }

        // --------------- RESET AND LISTENERS --------------- //
        protected override void OnEnable()
        {
            base.OnEnable();
            _sender.Subscribe(TryPublish);
        }

        protected virtual void OnDisable() => _sender.UnSubscribe(TryPublish);
    }
}
