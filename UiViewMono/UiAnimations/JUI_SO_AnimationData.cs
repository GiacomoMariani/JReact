using JReact;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace Jreact.UiViewMono.UiAnimations
{
    [CreateAssetMenu(menuName = "Reactive/UI/AnimationData", fileName = "J_UiAnimation", order = 0)]
    public class JUI_SO_AnimationData : ScriptableObject
    {
        public bool IsFloat => Type   == JEnum_Animation.AsFloat;
        public bool IsVector2 => Type == JEnum_Animation.AsVector2;
        public bool IsVector3 => Type == JEnum_Animation.AsVector3;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private AnimationCurve _animationCurve;
        public AnimationCurve AnimationCurve => _animationCurve;

        // --------------- FLOAT --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private float3 _onStartValue = new float3(0,    float.NaN, float.NaN);
        public float3 RawStartValue => _onStartValue;
        [BoxGroup("Setup", true, true, 0), SerializeField] private float3 _onCompleteValue = new float3(0, float.NaN, float.NaN);
        public float3 RawCompleteValue => _onCompleteValue;

        public float OnStartValue
        {
            get
            {
                Assert.IsTrue(IsFloat);
                return _onStartValue.x;
            }
        }
        public float OnCompleteValue
        {
            get
            {
                Assert.IsTrue(IsFloat);
                return _onCompleteValue.x;
            }
        }

        public Vector2 OnStartValueV2
        {
            get
            {
                Assert.IsTrue(IsVector2);
                return _onStartValue.xy;
            }
        }
        public Vector2 OnCompleteValueV2
        {
            get
            {
                Assert.IsTrue(IsVector2);
                return _onCompleteValue.xy;
            }
        }

        public Vector3 OnStartValueV3
        {
            get
            {
                Assert.IsTrue(IsVector3);
                return _onStartValue;
            }
        }
        public Vector3 OnCompleteValueV3
        {
            get
            {
                Assert.IsTrue(IsVector3);
                return _onCompleteValue;
            }
        }

        [BoxGroup("Setup", true, true, 0), ReadOnly, ShowInInspector] public JEnum_Animation Type
        {
            get
            {
                JEnum_Animation startType = InferType(_onStartValue);
                JEnum_Animation endType   = InferType(_onCompleteValue);
                Assert.IsTrue(startType == endType, $"Start({startType})/End({endType}) value types differ.");
                return startType;
            }
        }

        public float GetNormalizedProgress(float value)
        {
            Assert.IsTrue(IsFloat, $"{nameof(GetNormalizedProgress)} is only valid for float values");
            float denominator            = OnCompleteValue - OnStartValue;
            float currentNormalizedState = value           - OnStartValue;

            float normalizedProgress = math.abs(denominator) > math.EPSILON ? currentNormalizedState / denominator : 0;
            normalizedProgress = math.clamp(normalizedProgress, 0f, 1f);

            float evalProgress = AnimationCurve.FindNormalizedTimeMonotonic(normalizedProgress);
            return evalProgress;
        }

        [Button]
        private void ValidateFloat(float value)
        {
            float currentEval    = GetNormalizedProgress(value);
            float yNorm          = AnimationCurve.Evaluate(currentEval); // [0,1]
            float retrievedValue = math.lerp(OnStartValue, OnCompleteValue, yNorm);
            Assert.AreApproximatelyEqual(value, retrievedValue, 0.001f);
            JLog.Log($"Value: {value} - Eval: {currentEval} - Norm: {yNorm} - Retrieved: {retrievedValue}", JLogTags.Service, this);
        }

        private static JEnum_Animation InferType(float3 v)
        {
            if (float.IsNaN(v.y)) return JEnum_Animation.AsFloat;
            if (float.IsNaN(v.z)) return JEnum_Animation.AsVector2;
            return JEnum_Animation.AsVector3;
        }

        [Button] private void SetFloat() 
        { 
            _onStartValue = new float3(_onStartValue.x, float.NaN, float.NaN);
            _onCompleteValue = new float3(_onCompleteValue.x, float.NaN, float.NaN);
        }

        [Button]
        private void SetVector2() 
        {
            _onStartValue = new float3(_onStartValue.x, _onStartValue.y.IsNaN() ? 0 : _onStartValue.y, float.NaN);
            _onCompleteValue = new float3(_onCompleteValue.x, _onCompleteValue.y.IsNaN() ? 0 : _onCompleteValue.y, float.NaN);
        }

        [Button]
        private void SetVector3()
        {
            _onStartValue = new float3(_onStartValue.x, _onStartValue.y.IsNaN() ? 0 : _onStartValue.y,
                                      _onStartValue.z.IsNaN() ? 0 : _onStartValue.z);
            _onCompleteValue = new float3(_onCompleteValue.x, _onCompleteValue.y.IsNaN() ? 0 : _onCompleteValue.y,
                                      _onCompleteValue.z.IsNaN() ? 0 : _onCompleteValue.z);
        }
    }
}
