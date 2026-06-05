#if UNITY_DOTS
using Unity.Entities;

namespace JReact
{
    public static class JECS_SystemGroupExtensions
    {
        public static void SetGroupEnabled<T>(this World world, bool enable)
            where T : ComponentSystemGroup
        {
            var group = world.GetExistingSystemManaged<T>();
            if (group != null)
                group.Enabled = enable;
        }

        public static bool IsGroupEnabled<T>(this World world)
            where T : ComponentSystemGroup
        {
            var group = world.GetExistingSystemManaged<T>();
            return group != null && group.Enabled;
        }
    }
}
#endif
