using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Pool
{
    public static class J_Pool_Extensions
    {
        public static J_Pool<T> CreatePool<T>(this T prefab,                             int population = J_Pool<T>.ExpectedItems,
                                              int    perFrame = J_Pool<T>.ExpectedItems, Transform parent = null)
            where T : Component
        {
            var pool = J_Pool<T>.GetPool(prefab, population, perFrame, parent);
            return pool;
        }

        public static T Spawn<T>(this T prefab, Transform parent = null, bool worldPositionStays = true) where T : Component
        {
            var pool = GetPool(prefab);
            return pool.Spawn(parent, worldPositionStays);
        }

        public static T SpawnInstantiate<T>(this T prefab, Transform parent, bool worldPositionStays = true) where T : Component
        {
            var pool = GetPool(prefab);
            return pool.SpawnInstantiate(parent, worldPositionStays);
        }

        public static void DeSpawn<T>(this T prefab, GameObject gameObject) where T : Component
        {
            var pool = GetPool(prefab);
            pool.DeSpawn(gameObject);
        }

        public static void DeSpawn<T>(this T prefab, T itemSpawned) where T : Component
        {
            var pool = GetPool(prefab);
            pool.DeSpawn(itemSpawned);
        }

        private static J_Pool<T> GetPool<T>(T prefab) where T : Component
        {
            int hasCode = prefab.GetHashCode();
            Assert.IsTrue(J_Pool<T>.PoolIsReady(hasCode), $"No pool ready for {prefab.name}");
            J_Pool<T> pool = J_Pool<T>.GetPoolFromHashCode(prefab.GetHashCode());
            return pool;
        }

        // --------------- PARTICLE EFFECTS --------------- //
        public static ParticleSystem PlayParticles(this ParticleSystem prefab, Vector3 position, Transform parent = null,
                                                   bool                worldPositionStays = true, bool autoDespawn = true)
        {
            var pool = GetPool(prefab);
            return pool.PlayParticles(position, parent, worldPositionStays, autoDespawn);
        }

        public static ParticleSystem PlayParticles(this J_Pool<ParticleSystem> pool, Vector3 position, Transform parent = null,
                                                   bool                        worldPositionStays = true, bool autoDespawn = true)
        {
            var particle = pool.Spawn(parent, worldPositionStays);
            particle.transform.position = position;
            if (!autoDespawn) { return particle; }

            Timing.RunCoroutine(PlayThanRemove(particle, pool).CancelWith(particle.gameObject), Segment.LateUpdate);
            return particle;
        }

        private static IEnumerator<float> PlayThanRemove(ParticleSystem particle, J_Pool<ParticleSystem> pool)
        {
            yield return Timing.WaitForSeconds(particle.main.duration);
            pool.DeSpawn(particle);
        }
    }
}
