#if JSPINE_SUPPORT
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Assertions;
using Bone = Spine.Bone;


namespace JReact.JSpineSupport
{
    public sealed class J_DoodleBoneReader : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_ActorDoodle _doodle;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_DoodleBone _boneAsset;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required] private Transform _target;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _followRotation;

        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] private Bone _bone;
        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] private SkeletonRenderer SkeletonRenderer
            => _doodle.SkeletonRenderer;
        
        public Vector3 Position => _target.position;
        public Quaternion Rotation => _target.rotation;
        
        // --------------- COMMANDS --------------- //
        public void SetTarget(Transform target) { _target = target; }

        // --------------- IMPLEMENTATION --------------- //
        private void OnUpdateComplete(ISkeletonRenderer skeletonRenderer)
        {
            Transform skeletonTransform = skeletonRenderer.Component.transform;

            _target.position = _bone.GetWorldPosition(skeletonTransform);

            if (_followRotation) { _target.rotation = skeletonTransform.rotation * _bone.GetQuaternion(); }
        }

        private void ResolveBone()
        {
            Assert.IsTrue(_boneAsset.IsCompatible(_doodle.SpineSkeleton.skeletonDataAsset),
                          $"{gameObject.name} bone asset '{_boneAsset.name}' is not compatible with current skeleton.");

            _bone = _boneAsset.GetBone(_doodle.Skeleton);
        }
        
        private void OnEnable()
        {
            ResolveBone();
            SkeletonRenderer.UpdateComplete -= OnUpdateComplete;
            SkeletonRenderer.UpdateComplete += OnUpdateComplete;
        }

        private void OnDisable() { SkeletonRenderer.UpdateComplete -= OnUpdateComplete; }
    }
}
#endif