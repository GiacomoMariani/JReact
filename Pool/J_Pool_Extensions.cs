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
            int hashCode = prefab.GetHashCode();
            Assert.IsTrue(J_Pool<T>.PoolIsReady(hashCode), $"No pool ready for {prefab.name}");
            J_Pool<T> pool = J_Pool<T>.GetPoolFromHashCode(prefab.GetHashCode());
            return pool;
        }

        public static void DestroyPoolFor<T>(this T prefab, bool alsoSpawned, bool unRegister) where T : Component
        {
            int hashCode = prefab.GetHashCode();
            Assert.IsTrue(J_Pool<T>.PoolIsReady(hashCode), $"No pool ready for {prefab.name}");
            J_Pool<T> poolToClear = J_Pool<T>.GetPoolFromHashCode(prefab.GetHashCode());
            poolToClear.DestroyPool(alsoSpawned, unRegister);
        }

        // --------------- PARTICLE EFFECTS --------------- //
        public static ParticleSystem PlayParticles(this ParticleSystem prefab, Vector3 position, Transform parent = null,
                                                   bool                worldPositionStays = true, bool autoDespawn = true)
        {
            J_Pool<ParticleSystem> pool = GetPool(prefab);
            return pool.PlayParticles(position, parent, worldPositionStays, autoDespawn);
        }

        public static ParticleSystem PlayParticles(this J_Pool<ParticleSystem> pool, Vector3 position, Transform parent = null,
                                                   bool                        worldPositionStays = true, bool autoDespawn = true)
        {
            ParticleSystem particle = pool.Spawn(parent, worldPositionStays);
            particle.transform.position = position;
            particle.Play();
            if (!autoDespawn) { return particle; }

            Timing.RunCoroutine(PlayThanRemove(particle, pool, particle.main.duration).CancelWith(particle.gameObject),
                                Segment.LateUpdate);

            return particle;
        }

        public static ParticleSystem PlayParticlesLookingAt(this J_Pool<ParticleSystem> pool, Vector3 position,
                                                            Vector2                     positionToLookAt, Direction forwardDirection,
                                                            Transform                   parent = null, bool worldPositionStays = true,
                                                            bool                        autoDespawn = true)
        {
            ParticleSystem particle = pool.Spawn(parent, worldPositionStays);
            particle.transform.LookAt2D(positionToLookAt, forwardDirection);
            particle.transform.position = position;
            particle.Play();
            if (!autoDespawn) { return particle; }

            Timing.RunCoroutine(PlayThanRemove(particle, pool, particle.main.duration).CancelWith(particle.gameObject),
                                Segment.LateUpdate);

            return particle;
        }

        // --------------- PLAY EFFECTS --------------- //
        public static T PlayEffect<T>(this T prefab, Vector3 position, float durationInSeconds, Transform parent = null,
                                      bool   worldPositionStays = true)
            where T : Component
        {
            J_Pool<T> pool = GetPool(prefab);
            return pool.PlayEffect(position, durationInSeconds, parent, worldPositionStays);
        }

        public static T PlayEffect<T>(this J_Pool<T> pool, Vector3 position, float durationInSeconds, Transform parent = null,
                                      bool           worldPositionStays = true)
            where T : Component
        {
            T item = pool.Spawn(parent, worldPositionStays);
            item.transform.position = position;

            Timing.RunCoroutine(PlayThanRemove(item, pool, durationInSeconds).CancelWith(item.gameObject), Segment.LateUpdate);

            return item;
        }

        // --------------- GENERIC HELPERS --------------- //
        private static IEnumerator<float> PlayThanRemove<T>(T item, J_Pool<T> pool, float durationInSeconds)
            where T : Component
        {
            yield return Timing.WaitForSeconds(durationInSeconds);
            pool.DeSpawn(item);
        }
    }
}
