#if JSPINE_SUPPORT
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

namespace JReact.JSpineSupport
{
    public sealed class J_ActorDoodle : J_Mono_Actor<J_ActorDoodle>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required, ChildGameObjectsOnly]
        private SkeletonAnimation _spineSkeleton;
        public SkeletonAnimation SpineSkeleton => _spineSkeleton;
        public Skeleton Skeleton => _spineSkeleton.Skeleton;
        public AnimationState AnimationState => _spineSkeleton.AnimationState;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required, ChildGameObjectsOnly]
        private SkeletonRenderer _skeletonRenderer;
        public SkeletonRenderer SkeletonRenderer => _skeletonRenderer;

        // --------------- COMPONENTS --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required, ChildGameObjectsOnly]
        private J_DoodleSkinSelector _skinSelector;
        public J_DoodleSkinSelector SkinSelector => _skinSelector;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required, ChildGameObjectsOnly]
        private J_DoodleAnimations _animations;
        public J_DoodleAnimations Animations => _animations;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required, ChildGameObjectsOnly]
        private J_DoodleEvents _events;
        public J_DoodleEvents Events => _events;
    }
}
#endif
