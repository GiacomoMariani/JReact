using System;
using DG.Tweening;
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
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _vibrato = 10;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _randomness = 50;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0.01f)] private float _duration = .5f;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Vector3 _strength = Vector3.one;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Tweener _tweener;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Vector3 _startScale;

        // --------------- INITIALIZATION --------------- //
        private void Awake() => InitThis();

        private void InitThis()
        {
            DOTween.Init(true, true, LogBehaviour.ErrorsOnly);
            _startScale = transform.localScale;
        }

        // --------------- COMMANDS --------------- //
        [BoxGroup("Test", true, true, 0), Button(ButtonSizes.Medium)]
        private void ApplyEffect() => _tweener = transform.DOShakeScale(_duration, _strength, _vibrato, _randomness, _fadeOut)
                                                          .SetEase(_easeType)
                                                          .OnComplete(ResetThis)
                                                          .Play();

        [BoxGroup("Test", true, true, 0), Button(ButtonSizes.Medium)] private void ResetThis() => transform.localScale = _startScale;

        // --------------- UNITY EVENTS --------------- //
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_tweener == null ||
                !_tweener.active) ApplyEffect();
        }

        private void OnDisable() => _tweener.Complete(true);
    }
}
