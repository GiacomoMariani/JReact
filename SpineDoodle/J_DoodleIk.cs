#if JSPINE_SUPPORT
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.JSpineSupport
{
    public sealed class J_DoodleIk : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_ActorDoodle _doodle;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required, ChildGameObjectsOnly] private J_DoodleBonesWriter _targetWriter;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private SkeletonDataAsset _skeletonDataAsset;
        [BoxGroup("Setup"), SpineIkConstraint(dataField: nameof(_skeletonDataAsset)), SerializeField] private string _ikName;

        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] private IkConstraint _ik;

        public J_DoodleBonesWriter TargetWriter => _targetWriter;
        public IkConstraint Ik => _ik;

        // --------------- UNITY --------------- //
        private void OnEnable() => ResolveIk();

        // --------------- COMMANDS --------------- //
        public void SetTargetPosition(Vector2 worldPosition) => _targetWriter.SetPosition(worldPosition);

        public void SetMix(float mix) => _ik.Pose.Mix = Mathf.Clamp01(mix);

        public void SetBendDirection(int bendDirection) => _ik.Pose.BendDirection = bendDirection;

        // --------------- IMPLEMENTATION --------------- //
        private void ResolveIk()
        {
            _ik = _doodle.Skeleton.FindConstraint<IkConstraint>(_ikName);
            Assert.IsNotNull(_ik, $"{gameObject.name} could not resolve IK constraint '{_ikName}'.");
        }
    }
}
#endif
