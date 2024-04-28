using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.TweenerEffects
{
    public static class J_TweenerExtensions
    {
        private static readonly Vector2 Vector2Zero = new Vector2(0f, 0f);
        private static readonly Vector3 Vector3Zero = new Vector3(0f, 0f, 0f);
        private static readonly Vector3 Vector3One = new Vector3(1f,  1f, 1f);

        // --------------- SLIDE EFFECTS --------------- //
        /// <summary>
        /// let a rect transform slide in from any place
        /// </summary>
        /// <param name="rectTransform">the rect transform to slide</param>
        /// <param name="startPosition">the position where the slide start</param>
        /// <param name="duration">the duration of the tween</param>
        /// <param name="endValue">the final place for the rect transform</param>
        /// <param name="easeType">the type of ease animation</param>
        /// <returns>returns the tweener moving the transform</returns>
        public static Tweener Slide(this RectTransform rectTransform, Vector3 startPosition, Vector2 endValue, float duration,
                                    Ease               easeType = Ease.InOutQuint)
        {
            rectTransform.transform.localPosition = startPosition;
            return rectTransform.DOAnchorPos(endValue, duration, false).SetEase(easeType);
        }

        /// <summary>
        /// just like Slide, this is setup for slide in and defaults the endValue to 0
        /// </summary>
        public static Tweener SlideIn(this RectTransform rectTransform, Vector3 startPosition, float duration,
                                      Ease               easeType = Ease.InOutQuint)
            => Slide(rectTransform, startPosition, Vector2Zero, duration, easeType);

        /// <summary>
        /// just like Slide, this is setup for slide in and defaults the startValue to 0
        /// </summary>
        public static Tweener SlideOut(this RectTransform rectTransform, Vector2 endPosition, float duration,
                                       Ease               easeType = Ease.InOutQuint)
            => Slide(rectTransform, Vector3Zero, endPosition, duration, easeType);

        // --------------- FADE EFFECTS --------------- //
        /// <summary>
        /// fade in or out a given canvas group
        /// </summary>
        /// <param name="canvasGroup"the canvas group where to apply the effect></param>
        /// <param name="startAlpha">the starting alpha</param>
        /// <param name="endAlpha">the end alpha</param>
        /// <param name="duration">the duration of the animation</param>
        /// <param name="easeType">the ease type of the animation</param>
        /// <returns>returns the tweener related to this animation</returns>
        public static Tweener Fade(this CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration, Ease easeType)
        {
            canvasGroup.alpha = startAlpha;
            return canvasGroup.DOFade(endAlpha, duration).SetEase(easeType);
        }

        /// <summary>
        /// fades in the canvas group, it uses the same logic of Fade, but defaults the start as 0
        /// </summary>
        public static Tweener FadeIn(this CanvasGroup canvasGroup, float duration, Ease easeType, float endAlpha = 1f)
            => Fade(canvasGroup, 0f, endAlpha, duration, easeType);

        /// <summary>
        /// fades in the canvas group, it uses the same logic of Fade, but defaults the start as 1
        /// </summary>
        public static Tweener FadeOut(this CanvasGroup canvasGroup, float duration, Ease easeType, float endAlpha = 0f)
            => Fade(canvasGroup, 1f, endAlpha, duration, easeType);

        // --------------- SCALE --------------- //
        /// <summary>
        /// pops in or out a given rect transform using its scale
        /// </summary>
        /// <param name="rectTransform">the rect transform to pop in</param>
        /// <param name="startScale">the start scale for the transform</param>
        /// <param name="endScale">the end scale of the transform</param>
        /// <param name="duration">the duration of the animation</param>
        /// <param name="easeType">the ease type of the animation</param>
        /// <returns>return the tweener related to this animation</returns>
        public static Tweener PopRect(this RectTransform rectTransform, Vector3 startScale, Vector3 endScale,
                                      float              duration,      Ease    easeType = Ease.OutBounce)
        {
            rectTransform.transform.localScale = startScale;
            return rectTransform.DOScale(endScale, duration).SetEase(easeType);
        }

        /// <summary>
        /// let the transform pop in using PopRect and defaulting the start scale at 0
        /// </summary>
        public static Tweener PopIn(this RectTransform rectTransform, Vector3 endScale, float duration, Ease easeType = Ease.OutBounce)
            => PopRect(rectTransform, Vector3Zero, endScale, duration, easeType);

        /// <summary>
        /// let the transform pop in using PopRect and defaulting the start scale at 1
        /// </summary>
        public static Tweener PopOut(this RectTransform rectTransform, Vector3 endScale, float duration,
                                     Ease               easeType = Ease.OutBounce)
            => PopRect(rectTransform, Vector3One, endScale, duration, easeType);
    }
}
