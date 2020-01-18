using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.TweenerEffects
{
    public sealed class J_UiView_TweenFadeActivator : J_Abs_Tween_Activator
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private CanvasGroup _canvasGroup;
        [BoxGroup("Setup", true, true, 0), SerializeField] private FadeData _inData = new FadeData(0f,  1f, 0.5f, 0.5f);
        [BoxGroup("Setup", true, true, 0), SerializeField] private FadeData _outData = new FadeData(1f, 0f, 0.5f, 0f);

        protected override Tweener RunTween(bool activateView)
        {
            FadeData data = activateView ? _inData : _outData;
            _canvasGroup.blocksRaycasts = false;
            return _canvasGroup.FadeCanvas(data);
        }

        protected override void CompleteTween(bool activateView) => _canvasGroup.blocksRaycasts = true;
    }
}
