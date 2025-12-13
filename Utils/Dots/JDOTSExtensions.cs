#if UNITY_DOTS
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace JReact
{
    public static class JDOTSExtensions
    {
        public static void EnableSystemGroup(this ComponentSystemGroup systemGroup, bool enabled)
        {
            IReadOnlyList<ComponentSystemBase> managedSystems = systemGroup.ManagedSystems;
            foreach (var system in managedSystems) { system.Enabled = enabled; }

            NativeList<SystemHandle> unmanagedSystems = systemGroup.GetUnmanagedSystems();

            for (int i = 0; i < unmanagedSystems.Length; i++)
            {
                SystemHandle    handle    = unmanagedSystems[i];
                ref SystemState unmanaged = ref systemGroup.World.Unmanaged.ResolveSystemStateRef(handle);
                unmanaged.Enabled = enabled;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 AddOffsetWithRotation(this LocalTransform transform, float2 offset)
        {
            float3 r = math.mul(transform.Rotation, new float3(offset.x, offset.y, 0f)) * transform.Scale;
            return r.xy + transform.Position.xy;  
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool JContains<T>(this DynamicBuffer<T> buffer, T value)
            where T : unmanaged, IBufferElementData, System.IEquatable<T>
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].Equals(value))
                    return true;
            }
            return false;
        }
    }
}
#endif
