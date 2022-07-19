using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.ScreenMessage
{
    /// <summary>
    /// sends the messages
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Screen Message/Sender")]
    public sealed class J_MessageSender : ScriptableObject, jObservable<JMessage>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private event Action<JMessage> OnPublish;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private JMessage _message;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _currentId;

        // --------------- MAIN COMMAND - SEND --------------- //
        /// <summary>
        /// sends a message on the screen
        /// </summary>
        /// <param name="message">the text to send</param>
        /// <param name="messageId">the type of message</param>
        public void Send(string message)
        {
            JLog.Log($"{name} message = {message}", JLogTags.Message, this);

            _message.Content       = message;
            _message.MessageNumber = _currentId++;

            OnPublish?.Invoke(_message);
        }

        // --------------- SUBSCRIBERS --------------- //
        public void Subscribe(Action<JMessage>   actionToAdd)    { OnPublish += actionToAdd; }
        public void UnSubscribe(Action<JMessage> actionToRemove) { OnPublish -= actionToRemove; }

        private void OnDisable() { _currentId = 0; }
#if UNITY_EDITOR
        // --------------- TEST --------------- //
        [BoxGroup("Debug", true, true, 50), Button(ButtonSizes.Medium)] private void SendTestMessage() { Send("This is just a test"); }
#endif
    }

    //the message type
    public struct JMessage
    {
        public int MessageNumber;
        public string Content;
    }
}
