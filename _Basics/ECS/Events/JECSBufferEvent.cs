#if UNITY_DOTS
using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;

namespace JECS
{
    public static class JECSEvent
    {
        /// <summary>
        /// Structural/setup path. Safe to keep on EntityManager.
        /// Ensures the entity has the buffer and state, and resizes the buffer
        /// so indexed writes are always valid.
        /// </summary>
        public static void Initialize<T>(EntityManager em, Entity entity)
            where T : unmanaged, IBufferElementData
        {
            if (!em.HasBuffer<T>(entity)) { em.AddBuffer<T>(entity); }
        }

        public static T Get<T>(EntityManager em, Entity entity, int index) where T : unmanaged, IBufferElementData
        {
            DynamicBuffer<T> buffer = em.GetBuffer<T>(entity);
            return buffer[index];
        }

        /// <summary>
        /// Overwrites oldest when full.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), BurstCompile]
        public static void Push<T>(DynamicBuffer<T> buffer, in T value)
            where T : unmanaged, IBufferElementData { buffer.Add(value); }

        /// <summary>
        /// Overwrites oldest when full.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining), BurstCompile]
        public static void Push<T>(EntityCommandBuffer.ParallelWriter ecb, Entity entity, int sortKey, in T value)
            where T : unmanaged, IBufferElementData
        {
            ecb.AppendToBuffer(sortKey, entity, value);
        }
    }
}
#endif
