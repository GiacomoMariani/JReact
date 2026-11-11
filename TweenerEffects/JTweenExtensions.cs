using System;
using Jreact._Basics.ECS.UiViewMono.UiAnimations;
using PrimeTween;
using UnityEngine;

namespace JReact.TweenEffects
{
    /// <summary>
    /// The shared tween helpers. Each plays one JUI_SO_AnimationData, which owns the curve, the values and the JPhiTime
    /// duration. Alpha and custom values are absolute, moves are offsets from rest, scales multiply rest. UI time is
    /// unscaled. Compose several beats with a Sequence.
    /// </summary>
    public static class J_TweenExtensions
    {
        public static Tween Fade(this CanvasGroup target, JUI_SO_AnimationData data, float delay = 0f)
            => Tween.Alpha(target, new TweenSettings<float>(data.OnStartValue, data.OnCompleteValue, Settings(data, delay)));

        public static Tween Move(this RectTransform target, JUI_SO_AnimationData data, Vector2 rest, float delay = 0f)
            => Tween.UIAnchoredPosition(target, new TweenSettings<Vector2>(rest + data.OnStartValueV2,
                                                                          rest + data.OnCompleteValueV2,
                                                                          Settings(data, delay)));

        public static Tween Scale(this Transform target, JUI_SO_AnimationData data, Vector3 rest, float delay = 0f)
            => Tween.Scale(target, new TweenSettings<Vector3>(Vector3.Scale(rest, data.OnStartValueV3),
                                                             Vector3.Scale(rest, data.OnCompleteValueV3),
                                                             Settings(data, delay)));

        public static Tween Play<T>(this JUI_SO_AnimationData data, T target, Action<T, float> onValueChange, float delay = 0f)
            where T : class
            => Tween.Custom(target, new TweenSettings<float>(data.OnStartValue, data.OnCompleteValue, Settings(data, delay)),
                            onValueChange);

        private static TweenSettings Settings(JUI_SO_AnimationData data, float delay)
            => new TweenSettings(data.Duration, data.AnimationCurve, startDelay: delay, useUnscaledTime: true);
    }
}
