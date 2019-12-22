using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Credits
{
    public class J_UiView_RectMover : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private RectTransform _rect;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private Vector2 _velocityDirection = Vector2.up;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private float _endPosition;

        [FoldoutGroup("State", false, 5), ShowInInspector] private Vector2 _startPosition;
        [FoldoutGroup("State", false, 5), ShowInInspector] private Vector2 _currentVelocity;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        private void SanityChecks() => Assert.IsNotNull(_rect, $"{gameObject.name} requires a {nameof(_rect)}");

        private void InitThis() => _startPosition = _rect.anchoredPosition;

        // --------------- COMMANDS --------------- //
        public void AddVelocity(Vector2 velocity) => _currentVelocity += velocity;

        // --------------- RESET COMMANDS --------------- //
        public void ResetVelocity() => _currentVelocity = _velocityDirection;
        public void ResetPosition() => _rect.anchoredPosition = _startPosition;

        // --------------- UNITY FUNCTIONS --------------- //
        private void Update()
        {
            if (_rect.offsetMax.y < -_endPosition) _rect.Translate(_currentVelocity * Time.deltaTime, Space.Self);
        }

        private void OnEnable()
        {
            ResetPosition();
            ResetVelocity();
        }

        // --------------- EDITOR SETUP --------------- //
#if UNITY_EDITOR
        internal void SetFinalPosition(float y) { _endPosition = y + 1080f; }
#endif
    }
}
