#if JSPINE_SUPPORT
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

namespace JReact.JSpineSupport
{
    [CreateAssetMenu(menuName = "Reactive/Spine2d/Spine2d Skin", fileName = "J_SO_DoodleSkins", order = 0)]
    public sealed class J_SO_DoodleSkin : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private SkeletonDataAsset _skeletonDataAsset;
        public string SkeletonDataName => _skeletonDataAsset.name;

        [SpineSkin(dataField: nameof(_skeletonDataAsset)), SerializeField] private string _thisSkin;
        public string ThisSkin => _thisSkin;

        public bool IsCompatible(SkeletonDataAsset skeletonAnimationSkeletonDataAsset)
            => skeletonAnimationSkeletonDataAsset == _skeletonDataAsset;
    }
}
#endif
