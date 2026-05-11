#if JSPINE_SUPPORT
using System;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Assertions;
using Animation = Spine.Animation;

namespace JReact.JSpineSupport
{
    [Serializable]
    public class JSpineData
    {
        public bool Loop = true;
        public bool AllowRestart = true;
        public float MixDuration = 0.15f;
        public float TimeScale = 1f;
    }

    public sealed class J_DoodleAnimations : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_ActorDoodle _doodle;

        // --------------- COMMANDS - ENTRY POINT --------------- //
        [Button]
        public void SetAnimation(AnimationReferenceAsset animationReferenceAsset, JSpineData data, int trackIndex = 0)
        {
            SkeletonAnimation skeletonAnimation = _doodle.SpineSkeleton;
            if (!data.AllowRestart &&
                skeletonAnimation.IsAlreadyPlaying(animationReferenceAsset, trackIndex)) { return; }

            Animation spineAnimation = GetAnimation(animationReferenceAsset);

            TrackEntry trackEntry = skeletonAnimation.AnimationState.SetAnimation(trackIndex, spineAnimation, data.Loop);
            trackEntry.SetData(data);
        }

        public void QueueAnimation(AnimationReferenceAsset animationReferenceAsset, int trackIndex, JSpineData data, float delay = 0f)
        {
            SkeletonAnimation skeletonAnimation = _doodle.SpineSkeleton;
            if (delay <= 0)
            {
                Assert.IsTrue(!skeletonAnimation.IsLooping(trackIndex),
                              $"Cannot queue animation {animationReferenceAsset.Animation.Name} on track {trackIndex} with delay {delay} because it is already looping");
            }

            Animation  spineAnimation = GetAnimation(animationReferenceAsset);
            TrackEntry trackEntry     = skeletonAnimation.AnimationState.AddAnimation(trackIndex, spineAnimation, data.Loop, delay);

            trackEntry.SetData(data);
        }

        [Button]
        public void ClearTrackStill(int trackIndex)
        {
            SkeletonAnimation skeletonAnimation = _doodle.SpineSkeleton;
            skeletonAnimation.AnimationState.ClearTrack(trackIndex);
        }

        public void SetEmptyAnimation(int trackIndex, float mixDuration = 0.15f)
        {
            SkeletonAnimation skeletonAnimation = _doodle.SpineSkeleton;
            skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndex, mixDuration);
        }

        // --------------- SAFECHECKS --------------- //
        private Animation GetAnimation(AnimationReferenceAsset animationReferenceAsset)
        {
            Assert.IsNotNull(animationReferenceAsset, $"{gameObject.name} requires a {nameof(animationReferenceAsset)}");
            Assert.IsNotNull(animationReferenceAsset.Animation,
                             $"{gameObject.name} requires a valid {nameof(animationReferenceAsset.Animation)}");

            Skeleton  skeleton          = _doodle.Skeleton;
            Animation resolvedAnimation = skeleton.Data.FindAnimation(animationReferenceAsset.Animation.Name);

            Assert.IsNotNull(resolvedAnimation,
                             $"{gameObject.name} could not resolve animation '{animationReferenceAsset.Animation.Name}' on current skeleton");

            return resolvedAnimation;
        }

        [Button]
        public void SeeAllAnimations()
        {
            SkeletonAnimation skeletonAnimation = _doodle.SpineSkeleton;
            Skeleton          skeleton          = skeletonAnimation.Skeleton;

            if (skeleton      == null ||
                skeleton.Data == null)
            {
                JLog.Warning("Cannot list animations because skeleton is not initialized.", JLogTags.Avatar, this);
                return;
            }

            foreach (Animation animation in skeleton.Data.Animations) JLog.Log(animation.Name, JLogTags.Avatar, this);
        }
    }
}
#endif
