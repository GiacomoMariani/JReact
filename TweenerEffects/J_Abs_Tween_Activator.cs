using System.Collections.Generic;
using DG.Tweening;
using MEC;
using Sirenix.OdinInspector;

namespace JReact.TweenerEffects
{
    public abstract class J_Abs_Tween_Activator : J_Mono_ViewActivator
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Tweener _current;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId;

        public override void ActivateView(bool activateView)
        {
            _instanceId = GetInstanceID();
            ResetThis();
            Timing.RunCoroutine(TweenRunning(activateView).CancelWith(gameObject), Segment.Update, _instanceId);
        }

        protected virtual IEnumerator<float> TweenRunning(bool activateView)
        {
            //if we have an in animation we require the GO to be active at start
            if (activateView) base.ActivateView(activateView);

            _current = RunTween(activateView);
            yield return Timing.WaitForSeconds(_current.Duration());
            CompleteTween(activateView);

            //if we have an out animation we require the GO to be active at the end
            if (!activateView) base.ActivateView(activateView);
        }

        protected abstract Tweener RunTween(bool activateView);
        protected abstract void CompleteTween(bool activateView);
        

        private void ResetThis()
        {
            Timing.KillCoroutines(_instanceId);
            _current?.Complete(false);
        }

        private void OnDisable() => ResetThis();
    }
}
