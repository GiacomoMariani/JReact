using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.ScreenMessage
{
    /// <summary>
    /// Scene component that keeps a capped history of messages. It can listen to any number of
    /// <see cref="J_MessageSender"/>s (via <see cref="AddSender"/> / <see cref="RemoveSender"/>),
    /// and messages can also be pushed directly through the public <see cref="Record(JMessage)"/>
    /// from anywhere. Read <see cref="Messages"/> and subscribe via <see cref="SubscribeToLog"/> /
    /// <see cref="SubscribeToRemoved"/> to drive a scrolling log.
    /// </summary>
    public sealed class J_MessageStorage : MonoBehaviour
    {
        // --------------- EVENTS --------------- //
        private event Action<JMessage> OnLogged;  // a message was added to the history
        private event Action<JMessage> OnRemoved; // a message was dropped (over cap or cleared)

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0),
         Tooltip("Max messages kept. 0 = unlimited. Oldest are dropped first.")]
        private int _maxMessages = 100;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private readonly List<JMessage> _messages = new List<JMessage>(64);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private readonly List<J_MessageSender> _senders = new List<J_MessageSender>(4);
        private int _directCounter; // sequence number for messages built via the convenience Record overload

        public IReadOnlyList<JMessage>        Messages => _messages;
        public IReadOnlyList<J_MessageSender> Senders  => _senders;
        public int Count => _messages.Count;
        public JMessage this[int index] => _messages[index];

        // --------------- SENDERS --------------- //
        /// <summary>starts listening to the given sender; its messages are recorded from now on</summary>
        public void AddSender(J_MessageSender sender)
        {
            if (sender == null) { return; }
            if (_senders.Contains(sender))
            {
                JLog.Warning($"{name} is already listening to {sender.name}", JLogTags.Message, this);
                return;
            }

            sender.Subscribe(Record);
            _senders.Add(sender);
        }

        /// <summary>stops listening to the given sender</summary>
        public void RemoveSender(J_MessageSender sender)
        {
            if (sender == null) { return; }
            if (!_senders.Remove(sender)) { return; }

            sender.UnSubscribe(Record);
        }

        /// <summary>stops listening to every sender</summary>
        public void ClearSenders()
        {
            for (int i = 0; i < _senders.Count; i++) { _senders[i].UnSubscribe(Record); }
            _senders.Clear();
        }

        // --------------- RECORDING --------------- //
        /// <summary>records an already-built message (e.g. from a sender, or from anywhere)</summary>
        public void Record(JMessage message)
        {
            _messages.Add(message);
            OnLogged?.Invoke(message);
            TrimToCap();
        }

        /// <summary>builds and records a message directly, stamping a local number (and the current Unix time if none given)</summary>
        public void Record(string content, int messageId = JMessageType.System, int sourceId = 0,
                           string sourceName = null, long timeStamp = 0)
        {
            if (timeStamp <= 0) { timeStamp = JMessage.NowUnixMs(); }

            Record(new JMessage(content, messageId, _directCounter++, sourceId, sourceName, timeStamp));
        }

        private void TrimToCap()
        {
            if (_maxMessages <= 0) { return; }

            while (_messages.Count > _maxMessages)
            {
                JMessage removed = _messages[0];
                _messages.RemoveAt(0);
                OnRemoved?.Invoke(removed);
            }
        }

        // --------------- COMMANDS --------------- //
        [Button]
        public void ClearLog()
        {
            for (int i = _messages.Count - 1; i >= 0; i--)
            {
                JMessage removed = _messages[i];
                _messages.RemoveAt(i);
                OnRemoved?.Invoke(removed);
            }
        }

        // --------------- CLEANUP --------------- //
        //detach from every sender so the SO senders don't keep a dead delegate
        private void OnDestroy() { ClearSenders(); }

        // --------------- SUBSCRIBERS --------------- //
        public void SubscribeToLog(Action<JMessage>       action) { OnLogged  += action; }
        public void UnSubscribeToLog(Action<JMessage>     action) { OnLogged  -= action; }
        public void SubscribeToRemoved(Action<JMessage>   action) { OnRemoved += action; }
        public void UnSubscribeToRemoved(Action<JMessage> action) { OnRemoved -= action; }
    }
}
