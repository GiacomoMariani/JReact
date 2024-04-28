using System.Collections.Generic;
using DG.Tweening;
using JReact.J_Audio;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.StateControl.Weather
{
    /// <summary>
    /// darkens the sky on specicic states
    /// </summary>
    public sealed class J_Mono_DarkenSky : J_Mono_StateElement
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private const string COROUTINE_ThunderTag = "COROUTINE_ThunderTag";

        // --------------- SKY SETUP --------------- //
        [BoxGroup("Setup",       true, true), SerializeField, Required] private Image _darkSky;
        [BoxGroup("Setup - Sky", true, true), SerializeField, Range(0.00f, 0.25f)] private float _clearAlpha;
        [BoxGroup("Setup - Sky", true, true), SerializeField, Range(0.25f, 0.99f)] private float _darkAlpha = .5f;
        [BoxGroup("Setup - Sky", true, true), SerializeField] private float _secondsToGetDark = 1.0f;

        // --------------- THUNDER SETUP --------------- //
        [BoxGroup("Setup - Thunder", true, true), SerializeField] private Vector2 _thunderIntervals = new Vector2(5f, 30f);
        [BoxGroup("Setup - Thunder", true, true), SerializeField] private float _thunderChance = .5f;
        [BoxGroup("Setup - Thunder", true, true), SerializeField] private float _thunderLength = 4f;
        [BoxGroup("Setup - Thunder", true, true), SerializeField] private AnimationCurve _thunderCurve;
        [BoxGroup("Setup - Thunder", true, true), SerializeField] private AudioClip _soundControl;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Tween _thunderTween;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId;

        // --------------- INIT --------------- //
        protected override void InitThis()
        {
            base.InitThis();
            _instanceId = GetInstanceID();
        }

        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_darkSky,      $"{gameObject.name} requires a _darkSky");
            Assert.IsNotNull(_soundControl, $"{gameObject.name} requires a _soundControl");
            Assert.IsTrue(_darkSky.color.a <= 0.01,
                          $"{gameObject.name}- the alpha of the image {_darkSky.gameObject.name} should start at 0");
        }

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// darkens the sky
        /// </summary>
        private void DarkenSky()
        {
            _darkSky.DOFade(_darkAlpha, _secondsToGetDark);
            if (_thunderChance > 0f)
                Timing.RunCoroutine(ThunderLoop().CancelWith(gameObject), Segment.SlowUpdate, _instanceId, COROUTINE_ThunderTag);
        }

        /// <summary>
        /// clears the sky
        /// </summary>
        private void ClearSky()
        {
            Timing.KillCoroutines(_instanceId, COROUTINE_ThunderTag);
            _thunderTween?.Kill();
            _darkSky.DOFade(_clearAlpha, _secondsToGetDark);
        }

        // --------------- THUNDERS --------------- //
        //keep thundering until is raining
        private IEnumerator<float> ThunderLoop()
        {
            while (true)
            {
                TryThundering();
                yield return Timing.WaitForSeconds(_thunderIntervals.GetRandomValue());
            }
        }

        private void TryThundering()
        {
            if (_thunderChance.ChanceSuccess()) PlayThunder();
        }

        [BoxGroup("Test", true, true, 100), Button(ButtonSizes.Medium)]
        private void PlayThunder()
        {
            _thunderTween = null;
            _soundControl.PlaySound(J_AudioEnum.Ambience);
            _thunderTween = _darkSky.DOFade(_darkAlpha, _secondsToGetDark).SetEase<Tween>(_thunderCurve);
        }

        // --------------- ABSTRACT IMPLEMENTATION --------------- //
        protected override void EnterState() => DarkenSky();
        protected override void ExitState()  => ClearSky();
    }
}
