using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.JRandom
{
    /// <summary>
    /// a random float interval
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Random/Float")]
    public sealed class J_Random_Float : ScriptableObject
    {
        [BoxGroup("Range", true, true), SerializeField] private float _min;
        [BoxGroup("Range", true, true), SerializeField] private float _max = 1f;

        public static J_Random_Float CreateRandomFloat(float min, float max)
        {
            var randomCreator = CreateInstance<J_Random_Float>();
            randomCreator._min = min;
            randomCreator._max = max;
            return randomCreator;
        }

        public float GetRandomValue()
        {
            Assert.IsTrue(_max >= _min, $"{name} the minimum range ({_min}) should be lower or equal maximum {_max}");
            return UnityEngine.Random.Range(_min, _max);
        }
    }
}
