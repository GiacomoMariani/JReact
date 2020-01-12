using System.Collections.Generic;
using DG.Tweening;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.TweenerEffects
{
    public sealed class J_UiView_TweenFadeActivator : J_Mono_ViewActivator
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private CanvasGroup _canvasGroup;
        [BoxGroup("Setup", true, true, 0), SerializeField] private FadeData _inData = new FadeData(0f,  1f, 0.5f, 0.5f);
        [BoxGroup("Setup", true, true, 0), SerializeField] private FadeData _outData = new FadeData(1f, 0f, 0.5f, 0f);

        //for safety reasons we do not want to run this at the first run
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Tweener _current;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId;

        protected override void InitThis()
        {
            base.InitThis();
            _instanceId = GetInstanceID();
        }

        public override void ActivateView(bool activateView)
        {
            //stop to avoid running multiple coroutine/tweeners
            ResetThis();
            //get the related animation and run it
            FadeData data = activateView ? _inData : _outData;
            Timing.RunCoroutine(DelayAndRun(data, activateView), Segment.Update, _instanceId);
        }

        private IEnumerator<float> DelayAndRun(FadeData data, bool activateView)
        {
            //if we have an in animation we require the GO to be active at start
            if (activateView) base.ActivateView(activateView);

            _canvasGroup.blocksRaycasts = false;
            _current                    = _canvasGroup.FadeCanvas(data);
            yield return Timing.WaitForSeconds(data.duration);
            _canvasGroup.blocksRaycasts = true;

            //if we have an out animation we require the GO to be active at the end
            if (!activateView) base.ActivateView(activateView);
        }

        private void ResetThis()
        {
            Timing.KillCoroutines(_instanceId);
            _current?.Complete(false);
        }
    }
}
