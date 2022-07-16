using System;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using JReact.J_Audio;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.UiViewMono
{
    /// <summary>
    /// set this up to apply a multi pop up for a set of rect transforms
    /// </summary>
    public class J_MultiPopRect
    {
        public event Action OnPop;

        [FoldoutGroup("Setup", false, 0), ReadOnly, ShowInInspector] private readonly Vector3 _startScale;
        [FoldoutGroup("Setup", false, 0), ReadOnly, ShowInInspector] private readonly Vector3 _endScale;
        [FoldoutGroup("Setup", false, 0), ReadOnly, ShowInInspector] private readonly float _durationOfAnimation;
        [FoldoutGroup("Setup", false, 0), ReadOnly, ShowInInspector] private readonly float _pauseBetweenAnimations;
        [FoldoutGroup("Setup", false, 0), ReadOnly, ShowInInspector] private readonly Ease _easeType;
        [FoldoutGroup("Setup", false, 0), ReadOnly, ShowInInspector] private readonly AudioClip _sound;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public CoroutineHandle Handle { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool IsRunning => Handle.IsRunning;

        /// <summary>
        /// generates a struct to track the animation of a multi pop up ui 
        /// </summary>
        /// <param name="startScale">the start scale for the rect transform</param>
        /// <param name="endScale">the end scale for the the rect transform</param>
        /// <param name="durationOfAnimation">the duration of the animation</param>
        /// <param name="pauseBetweenAnimations">the pause between each animation</param>
        /// <param name="easeType">the ease type for the animation</param>
        public J_MultiPopRect(Vector3 startScale,                    Vector3 endScale, float durationOfAnimation = .15f,
                              float   pauseBetweenAnimations = 0.1f, Ease    easeType = Ease.OutBounce, AudioClip audioOnPop = null)
        {
            _startScale             = startScale;
            _endScale               = endScale;
            _durationOfAnimation    = durationOfAnimation;
            _pauseBetweenAnimations = pauseBetweenAnimations;
            _easeType               = easeType;
            _sound                  = audioOnPop;

            Handle = default;
        }

        public void Launch(RectTransform[] rectTransforms)
        {
            if (IsRunning) { Stop(); }

            Timing.RunCoroutine(PopRects(rectTransforms), Segment.LateUpdate);
        }

        private IEnumerator<float> PopRects(RectTransform[] rectTransforms)
        {
            for (int i = 0; i < rectTransforms.Length; i++) { rectTransforms[i].transform.localScale = _startScale; }

            for (int i = 0; i < rectTransforms.Length; i++)
            {
                rectTransforms[i].DOScale(_endScale, _durationOfAnimation).SetEase(_easeType);
                if (_sound != null) { _sound.PlaySound(J_AudioEnum.UI); }

                OnPop?.Invoke();
                yield return Timing.WaitForSeconds(_pauseBetweenAnimations);
            }
        }

        /// <summary>
        /// stops the current animation, he make sure that only the current tweener completes
        /// </summary>
        public void Stop()
        {
            if (Handle.IsRunning) { Timing.KillCoroutines(Handle); }
        }
    }
}
