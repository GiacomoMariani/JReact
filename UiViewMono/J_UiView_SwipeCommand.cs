using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace JReact.UiView
{
    public sealed class J_UiView_SwipeCommand : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // --------------- SETUP - EVENTS --------------- //
        [BoxGroup("Setup", true, true), SerializeField] private float _dragAmount = 2f;
        [BoxGroup("Setup", true, true), SerializeField] private JUnityEvent _upEvents = new JUnityEvent();
        [BoxGroup("Setup", true, true), SerializeField] private JUnityEvent _downEvents = new JUnityEvent();
        [BoxGroup("Setup", true, true), SerializeField] private JUnityEvent _rightEvents = new JUnityEvent();
        [BoxGroup("Setup", true, true), SerializeField] private JUnityEvent _leftEvents = new JUnityEvent();

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private float _currentDragX;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private float _currentDragY;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isDragging;

        // --------------- INJECTION --------------- //
        public void InjectCommands(UnityAction up = null, UnityAction down = null, UnityAction right = null, UnityAction left = null)
        {
            _upEvents.AddListener(up);
            _downEvents.AddListener(down);
            _leftEvents.AddListener(left);
            _rightEvents.AddListener(right);
        }

        // --------------- UNITY EVENTS --------------- //
        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging   = true;
            _currentDragX = 0;
            _currentDragY = 0;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            _currentDragX += eventData.delta.x;
            _currentDragY += eventData.delta.y;
            if (_currentDragX >= _dragAmount)
            {
                _rightEvents?.Invoke();
                ResetThis();
            }

            if (_currentDragX <= -_dragAmount)
            {
                _leftEvents?.Invoke();
                ResetThis();
            }

            if (_currentDragY >= _dragAmount)
            {
                _upEvents?.Invoke();
                ResetThis();
            }

            if (_currentDragY <= -_dragAmount)
            {
                _leftEvents?.Invoke();
                ResetThis();
            }
        }

        public void OnEndDrag(PointerEventData eventData) => ResetThis();

        // --------------- RESET AND DESTROY --------------- //
        private void ResetThis()
        {
            _isDragging   = false;
            _currentDragX = 0;
            _currentDragY = 0;
        }
    }
}
