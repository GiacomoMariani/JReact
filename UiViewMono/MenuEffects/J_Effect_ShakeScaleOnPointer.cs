using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JReact.UiView
{
    public sealed class J_Effect_ShakeScaleOnPointer : MonoBehaviour, IPointerEnterHandler
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private Ease _easeType;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _fadeOut = true;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _frequency = 10;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _randomness = 50;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0.01f)] private float _duration = .5f;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Vector3 _strength = Vector3.one;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Transform _thisTransform => transform;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Tween _tween;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Vector3 _startScale;

        // --------------- INITIALIZATION --------------- //
        private void Awake() => InitThis();

        private void InitThis() { _startScale = transform.localScale; }

        // --------------- COMMANDS --------------- //
        [BoxGroup("Test", true, true, 0), Button(ButtonSizes.Medium)]
        private void ApplyEffect() => _tween = Tween.ShakeScale(_thisTransform, _strength, _duration, _frequency, _fadeOut).OnComplete(ResetThis); 

        [BoxGroup("Test", true, true, 0), Button(ButtonSizes.Medium)] private void ResetThis() => transform.localScale = _startScale;

        // --------------- UNITY EVENTS --------------- //
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_tween.isAlive ) { ApplyEffect(); }
        }

        private void OnDisable() => _tween.Complete();
    }
}
