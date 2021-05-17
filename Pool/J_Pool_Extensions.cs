using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Pool
{
    public static class J_Pool_Extensions
    {
        public static J_Pool<T> CreatePool<T>(this T prefab, int population = J_Pool<T>.ExpectedItems,
                                              int    perFrame = J_Pool<T>.ExpectedItems, Transform parent = null)
            where T : MonoBehaviour
        {
            var pool = J_Pool<T>.GetPool(prefab, population, perFrame, parent);
            return pool;
        }

        public static T Spawn<T>(this T prefab, Transform parent = null, bool worldPositionStays = true) where T : MonoBehaviour
        {
            var pool = GetPool(prefab);
            return pool.Spawn(parent, worldPositionStays);
        }

        public static T SpawnInstantiate<T>(this T prefab, Transform parent, bool worldPositionStays = true) where T : MonoBehaviour
        {
            var pool = GetPool(prefab);
            return pool.SpawnInstantiate(parent, worldPositionStays);
        }

        public static void DeSpawn<T>(this T prefab, GameObject gameObject) where T : MonoBehaviour
        {
            var pool = GetPool(prefab);
            pool.DeSpawn(gameObject);
        }

        public static void DeSpawn<T>(this T prefab, T itemSpawned) where T : MonoBehaviour
        {
            var pool = GetPool(prefab);
            pool.DeSpawn(itemSpawned);
        }

        private static J_Pool<T> GetPool<T>(T prefab) where T : MonoBehaviour
        {
            int hasCode = prefab.GetHashCode();
            Assert.IsTrue(J_Pool<T>.PoolIsReady(hasCode), $"No pool ready for {prefab.name}");
            J_Pool<T> pool = J_Pool<T>.GetPoolFromHashCode(prefab.GetHashCode());
            return pool;
        }
    }
}
