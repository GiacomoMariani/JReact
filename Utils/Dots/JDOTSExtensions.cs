#if UNITY_DOTS
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

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
    }
}
#endif
