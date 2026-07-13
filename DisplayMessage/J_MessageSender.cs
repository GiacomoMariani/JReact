using System;
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
        /// sends a (sourceless) message on the screen
        /// </summary>
        /// <param name="message">the text to send</param>
        /// <param name="messageId">(optional) the message type, see JMessageType</param>
        public void Send(string message, int messageId = 0) => Send(message, messageId, 0, null);

        /// <summary>
        /// sends a message, optionally attributed to a source (e.g. a speaking character)
        /// </summary>
        /// <param name="message">the resolved text to send</param>
        /// <param name="messageId">the message type, see JMessageType</param>
        /// <param name="sourceId">stable id of the source, 0 for none</param>
        /// <param name="sourceName">cached display name of the source</param>
        /// <param name="timeStamp">Unix time in ms; pass 0 to stamp with the current time</param>
        public void Send(string message, int messageId, int sourceId, string sourceName, long timeStamp = 0)
        {
            JLog.Log($"{name} message = {message}", JLogTags.Message, this);

            if (timeStamp <= 0) { timeStamp = JMessage.NowUnixMs(); }

            _message = new JMessage(message, messageId, ++_currentId, sourceId, sourceName, timeStamp);

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
}
