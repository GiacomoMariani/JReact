using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace JReact
{
    public struct JData : IComponentData
    {
    }

    public struct TN_Archetype_Template
    {
        private static ComponentType[] Archetype = { ComponentType.ReadWrite<LocalTransform>(), ComponentType.ReadWrite<JData>(), };

        // --------------- BASE STRUCTURE --------------- //
        public static EntityArchetype AsArchetype(EntityManager em) => em.CreateArchetype(Archetype);

        public static EntityQuery AsQuery(EntityManager em) => em.CreateEntityQuery(Archetype);

        // --------------- GETTERS --------------- //
        public static NativeArray<Entity> GetEntitiesArray(EntityManager em, Allocator allocator)
            => em.CreateEntityQuery(Archetype).ToEntityArray(allocator);

        public static NativeArray<T> GetComponentArray<T>(EntityManager em, Allocator allocator)
            where T : unmanaged, IComponentData => em.CreateEntityQuery(Archetype).ToComponentDataArray<T>(allocator);

        public static Entity GetFirstEntity(EntityManager em) => GetEntityAtIndex(em, 0);

        public static Entity GetEntityAtIndex(EntityManager em, int index)
        {
            NativeArray<Entity> array  = GetEntitiesArray(em, Allocator.TempJob);
            Entity              entity = array[index];
            array.Dispose();
            return entity;
        }

        // --------------- COMMANDS --------------- //
        public static Entity AddEntity(EntityManager em) => em.CreateEntity(AsArchetype(em));
        public static void   DestryAll(EntityManager em) => em.DestroyEntity(AsQuery(em));
    }
}
