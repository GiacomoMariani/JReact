using System.Collections.Generic;
using DG.Tweening;
using JReact.UiView;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.ScreenMessage
{
    /// <summary>
    /// pops a message on the screen
    /// </summary>
    public sealed class J_Mono_PoppingMessages : J_Mono_ActorItem<JMessage>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private const string COROUTINE_PoppingMessagesTag = "COROUTINE_PoppingMessagesTag";

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_UiView_FloatingText _floatingPrefab;
        [BoxGroup("Setup", true, true), SerializeField] private Vector2 _direction;
        [BoxGroup("Setup", true, true), SerializeField] private Color _color;
        [BoxGroup("Setup", true, true), SerializeField, Range(0.5f, 10.0f)] private float _secondsToComplete = 1.0f;
        [BoxGroup("Setup", true, true), SerializeField] private Ease _messageEase;

        // --------------- STATE --------------- //
        [BoxGroup("State", true, true, 15), ReadOnly] private int _id;
        [BoxGroup("State", true, true, 15), ReadOnly]
        private List<(JMessage message, J_UiView_FloatingText instance)> _messageDictionary =
            new List<(JMessage message, J_UiView_FloatingText instance)>();

        [BoxGroup("State", true, true, 15), ReadOnly]
        private Queue<J_UiView_FloatingText> _messagePool = new Queue<J_UiView_FloatingText>(5);

        // --------------- INITIALIZATION AND SETUP --------------- //
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        private void SanityChecks() { Assert.IsNotNull(_floatingPrefab, $"{gameObject.name} requires a _simpleTextPrefab"); }
        private void InitThis()     { _id = GetHashCode(); }

        // --------------- SENDER --------------- //
        protected override void ActorUpdate(JMessage messageSent)
        {
            //instantiate a new message
            J_UiView_FloatingText messageObject = GetMessageInstance();
            //setup the message 
            messageObject.PublishThisMessage(messageSent.Content, _color, _direction, _secondsToComplete, _messageEase);
            //send it to the dictionary
            _messageDictionary.Add((messageSent, messageObject));
            //wait before removal if this is not a permanent message
            Timing.RunCoroutine(WaitThenRemove((messageSent, messageObject)), Segment.LateUpdate, _id, COROUTINE_PoppingMessagesTag);
        }

        private J_UiView_FloatingText GetMessageInstance()
        {
            J_UiView_FloatingText message = _messagePool.Count == 0 ? Instantiate(_floatingPrefab, transform) : _messagePool.Dequeue();
            message.gameObject.SetActive(true);
            return message;
        }

        //wait the amount of seconds, then remove the message
        private IEnumerator<float> WaitThenRemove((JMessage message, J_UiView_FloatingText instance) messageTuple)
        {
            //wait then remove
            yield return Timing.WaitForSeconds(_secondsToComplete);
            RemoveOneMessage(messageTuple);
        }

        //used to remove one object
        private void RemoveOneMessage((JMessage message, J_UiView_FloatingText instance) messageTuple)
        {
            J_UiView_FloatingText messageView = messageTuple.instance;
            _messageDictionary.Remove(messageTuple);
            Assert.IsNotNull(messageView, $"{gameObject.name} message was null {messageTuple.message.Content}");

            RemoveMessageInstance(messageView);
        }

        private void RemoveMessageInstance(J_UiView_FloatingText messageView)
        {
            messageView.gameObject.SetActive(false);
            _messagePool.Enqueue(messageView);
        }
    }
}
