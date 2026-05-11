#if JSPINE_SUPPORT
using Spine;
using Spine.Unity;

namespace JReact.JSpineSupport
{
    public static class JSpine2dExtensions
    {
        public static bool IsAlreadyPlaying(this SkeletonAnimation skeletonAnimation, AnimationReferenceAsset animationReferenceAsset,
                                            int                    trackIndex)
        {
            TrackEntry currentTrackEntry = skeletonAnimation.AnimationState.GetCurrent(trackIndex);
            if (currentTrackEntry == null) { return false; }

            return currentTrackEntry.Animation == animationReferenceAsset.Animation;
        }

        public static TrackEntry GetTrackEntry(this SkeletonAnimation skeletonAnimation, int trackIndex)
            => skeletonAnimation.AnimationState.GetCurrent(trackIndex);

        public static TrackEntry SetData(this TrackEntry entry, JSpineData data)
        {
            entry.Loop        = data.Loop;
            entry.MixDuration = data.MixDuration;
            entry.TimeScale   = data.TimeScale;
            return entry;
        }

        public static bool IsLooping(this SkeletonAnimation skeletonAnimation, int trackIndex)
        {
            TrackEntry current = skeletonAnimation.AnimationState.GetCurrent(trackIndex);

            return current != null && current.Loop;
        }
    }
}
#endif
