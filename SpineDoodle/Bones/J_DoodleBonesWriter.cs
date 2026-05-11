#if JSPINE_SUPPORT
using JReact;
using Sirenix.OdinInspector;
using UnityEngine;
using Spine.Unity;
using UnityEngine.Assertions;
using Bone = Spine.Bone;

namespace JReact.JSpineSupport
{
    public sealed class J_DoodleBonesWriter : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_ActorDoodle _doodle;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_DoodleBone _boneAsset;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required] private Transform _target;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _writeRotation;
        
        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] private Bone _bone;
        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] private SkeletonRenderer SkeletonRenderer
            => _doodle.SkeletonRenderer;

        // --------------- UNITY --------------- //
        private void OnEnable()
        {
            ResolveBone();
            SkeletonRenderer.UpdateWorld -= OnUpdateWorld;
            SkeletonRenderer.UpdateWorld += OnUpdateWorld;
        }

        private void OnDisable() { SkeletonRenderer.UpdateWorld -= OnUpdateWorld; }

        // --------------- COMMANDS --------------- //
        public void SetTarget(Transform target) { _target = target; }

        public void Place(J2DPlacement     data)     { _target.PlaceFromJ2D(data); }
        public void SetPosition(Vector2    position) { _target.position = new Vector3(position.x, position.y, _target.position.z); }
        public void SetRotation(Quaternion rotation) { _target.rotation = rotation; }

        // --------------- IMPLEMENTATION --------------- //

        private void OnUpdateWorld(ISkeletonRenderer skeletonRenderer)
        {
            Vector3 skeletonSpacePosition =
                skeletonRenderer.Component.transform.InverseTransformPoint(_target.position);

            _bone.SetPositionSkeletonSpace(skeletonSpacePosition);
            
            if (_writeRotation) { _bone.Pose.Rotation = _target.eulerAngles.z; }
        }

        private void ResolveBone()
        {
            Assert.IsTrue(_boneAsset.IsCompatible(_doodle.SpineSkeleton.skeletonDataAsset),
                          $"{gameObject.name} bone asset '{_boneAsset.name}' is not compatible with current skeleton.");

            _bone = _boneAsset.GetBone(_doodle.Skeleton);
        }
    }
}
#endif
