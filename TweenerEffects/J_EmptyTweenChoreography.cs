using PrimeTween;

namespace JReact
{
    /// <summary>Explicitly opts out of visual feedback while preserving the choreography lifecycle.</summary>
    public sealed class J_EmptyTweenChoreography : J_Abs_TweenChoreography
    {
        protected override void PrepareImpl() {}
        protected override void CompleteImpl() {}
        protected override Sequence BuildSequence(float duration) => default;
        private void OnDisable() => Stop();
    }
}
