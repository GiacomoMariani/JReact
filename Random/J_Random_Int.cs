using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.JRandom
{
    /// <summary>
    /// a random float interval
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Random/Int")]
    public sealed class J_Random_Int : ScriptableObject
    {
        [BoxGroup("Range", true, true), SerializeField] private int _min;
        [BoxGroup("Range", true, true), SerializeField] private int _max = 1;

        public static J_Random_Int CreateRandomFloat(int min, int max)
        {
            var randomCreator = CreateInstance<J_Random_Int>();
            randomCreator._min = min;
            randomCreator._max = max;
            return randomCreator;
        }

        public int GetRandomValue()
        {
            Assert.IsTrue(_max >= _min, $"{name} the minimum range ({_min}) should be lower or equal maximum {_max}");
            return UnityEngine.Random.Range(_min, _max);
        }
    }
}
