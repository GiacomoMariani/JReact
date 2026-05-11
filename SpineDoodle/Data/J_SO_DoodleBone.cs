#if JSPINE_SUPPORT
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.JSpineSupport
{
    [CreateAssetMenu(menuName = "Reactive/Spine2d/Spine2d Skin", fileName = "J_SO_DoodleSkins", order = 3)]
    public sealed class J_SO_DoodleBone : ScriptableObject
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private SkeletonDataAsset _skeletonDataAsset;

        [BoxGroup("Setup"), SpineBone(dataField: nameof(_skeletonDataAsset)), SerializeField]
        private string _boneName;
        public string BoneName => _boneName;

        public bool IsCompatible(SkeletonDataAsset skeletonDataAsset) => skeletonDataAsset == _skeletonDataAsset;

        public Bone GetBone(Skeleton skeleton)
        {
            Assert.IsNotNull(skeleton, "Skeleton is required.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(_boneName), "Bone name is required.");

            Bone bone = skeleton.FindBone(_boneName);

            Assert.IsNotNull(bone, $"Could not resolve bone '{_boneName}' on current skeleton.");
            return bone;
        }
    }
}
#endif
