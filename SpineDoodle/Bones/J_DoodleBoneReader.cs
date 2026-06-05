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
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private SkeletonDataAsset _skeletonDataAsset;
        [BoxGroup("Setup", true, true, 0), SpineBone(dataField: nameof(_skeletonDataAsset)), SerializeField] private string _boneName;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required] private Transform _target;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _followRotation;

        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] private Bone _bone;
        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] private SkeletonRenderer SkeletonRenderer
            => _doodle.SkeletonRenderer;
        
        public Vector3 Position => _target.position;
        public Quaternion Rotation => _target.rotation;
        
        // --------------- UNITY --------------- //
        private void OnEnable()
        {
            ResolveBone();
            SkeletonRenderer.UpdateComplete -= OnUpdateComplete;
            SkeletonRenderer.UpdateComplete += OnUpdateComplete;
        }

        private void OnDisable() { SkeletonRenderer.UpdateComplete -= OnUpdateComplete; }
        
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
            _bone = _doodle.Skeleton.FindBone(_boneName);
            Assert.IsNotNull(_bone, $"{gameObject.name} could not resolve bone '{_boneName}'.");
        }
    }
}
#endif