using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.TweenerEffects
{
    public static class J_TweenerExtensions
    {
        public static Tweener FadeCanvas(this CanvasGroup canvas, FadeData data)
        {
            canvas.alpha = data.start;
            var tweener = canvas.DOFade(data.end, data.duration).SetDelay(data.delay).SetEase(data.easeType);
            return tweener;
        }
        
        public static Tweener SlidePivot(this RectTransform canvas, SlidePivotData pivotData)
        {
            canvas.pivot = pivotData.start;
            var tweener = canvas.DOPivot(pivotData.end, pivotData.duration).SetDelay(pivotData.delay).SetEase(pivotData.easeType);
            return tweener;
        }
    }

    [Serializable]
    public struct FadeData
    {
        public FadeData(float start, float end, float duration, float delay = 0f, Ease easeType = Ease.Linear)
        {
            this.start    = start;
            this.end      = end;
            this.duration = duration;
            this.delay    = delay;
            this.easeType = easeType;
        }

        [BoxGroup("Setup", true, true, 0), MinValue(0f), MaxValue(1f)] public float start;
        [BoxGroup("Setup", true, true, 0), MinValue(0f), MaxValue(1f)] public float end;
        [BoxGroup("Setup", true, true, 0), MinValue(0)] public float duration;
        [BoxGroup("Setup", true, true, 0), MinValue(0)] public float delay;
        [BoxGroup("Setup", true, true, 0)] public Ease easeType;
    }
    
    [Serializable]
    public struct SlidePivotData
    {
        public SlidePivotData(Vector2 start, Vector2 end, float duration, float delay = 0f, Ease easeType = Ease.Linear)
        {
            this.start    = start;
            this.end      = end;
            this.duration = duration;
            this.delay    = delay;
            this.easeType = easeType;
        }

        [BoxGroup("Setup", true, true, 0), MinValue(0f), MaxValue(1f)] public Vector2 start;
        [BoxGroup("Setup", true, true, 0), MinValue(0f), MaxValue(1f)] public Vector2 end;
        [BoxGroup("Setup", true, true, 0), MinValue(0)] public float duration;
        [BoxGroup("Setup", true, true, 0), MinValue(0)] public float delay;
        [BoxGroup("Setup", true, true, 0)] public Ease easeType;
    }
}
