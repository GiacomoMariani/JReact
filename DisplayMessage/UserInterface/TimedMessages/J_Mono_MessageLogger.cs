using JReact.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.ScreenMessage.MessageLogs
{
    public sealed class J_Mono_MessageLogger : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(1)] private int _expextedAmount = 5;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(1)] private int _messageLifeInSeconds = 5;

        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Transform _shown;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Transform _disabled;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_MessageSender _sender;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Mono_TimedMesssageView _prefab;

        private void DisplayMessage(JMessage message)
        {
            var messageView = _prefab.Spawn(_shown);
            messageView.Display(message.Content, _messageLifeInSeconds);
        }

        // --------------- LISTENER SETUP --------------- //
        private void Awake()
        {
            var pool = _prefab.CreatePool(_expextedAmount, parent: _disabled);
        }

        private void OnEnable() { _sender.Subscribe(DisplayMessage); }

        private void OnDisable() { _sender.UnSubscribe(DisplayMessage); }
    }
}
