#if JSPINE_SUPPORT
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Assertions;
using Bone = Spine.Bone;

namespace JReact.JSpineSupport
{
    public sealed class J_DoodleBonesWriter : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_ActorDoodle _doodle;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private SkeletonDataAsset _skeletonDataAsset;
        [BoxGroup("Setup", true, true, 0), SpineBone(dataField: nameof(_skeletonDataAsset)), SerializeField] private string _boneName;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required] private Transform _target;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _writeRotation;
        
        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] private Bone _bone;
        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] private SkeletonRenderer SkeletonRenderer
            => _doodle == null ? null : _doodle.SkeletonRenderer;

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
            _bone = _doodle.Skeleton.FindBone(_boneName);
            Assert.IsNotNull(_bone, $"{gameObject.name} could not resolve bone '{_boneName}'.");
        }
        
                
#if UNITY_EDITOR
        // --------------- EDITOR GIZMO --------------- //
        [UnityEditor.DrawGizmo(UnityEditor.GizmoType.InSelectionHierarchy)]
        private static void DrawIkTargetGizmo(J_DoodleBonesWriter boneWriter, UnityEditor.GizmoType gizmoType)
        {
            if (boneWriter._target == null) { return; }

            Vector3 worldPosition = boneWriter._target.position;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(worldPosition, 0.05f);
        }
#endif
    }
}
#endif
