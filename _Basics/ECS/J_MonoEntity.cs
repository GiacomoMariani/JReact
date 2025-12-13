using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace JReact.JEntities
{
    public sealed class J_MonoEntity : MonoBehaviour
    {
        // --------------- ECS DATA --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Entity Entity { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private World _world;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private EntityManager _entityManager;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsReady => Entity != Entity.Null;

        // --------------- SPAWN --------------- //
        public void InjectEntity(Entity entity, World world = default)
        {
            // --------------- ECS SETUP AND CONNECTION --------------- //
            _world         = world ?? World.DefaultGameObjectInjectionWorld;
            _entityManager = _world.EntityManager;

            if (Entity != Entity.Null) { ResetEntity(); }

            Entity = entity;
        }

        public void ResetEntity()
        {
            
            _entityManager.DestroyEntity(Entity);
        }

        // --------------- ECS CONNECTION --------------- //
        public bool HasEcsComponent<TComponentData>() where TComponentData : unmanaged, IComponentData
            => _entityManager.HasComponent<TComponentData>(Entity);
        
        public bool HasEcsBuffer<TBuffer>() where TBuffer : unmanaged, IBufferElementData
            => _entityManager.HasComponent<TBuffer>(Entity);

        public TComponentData GetEcsComponent<TComponentData>() where TComponentData : unmanaged, IComponentData
            => _entityManager.GetComponentData<TComponentData>(Entity);

        public void SetEcsComponent<TComponentData>(TComponentData value) where TComponentData : unmanaged, IComponentData
            => _entityManager.SetComponentData<TComponentData>(Entity, value);

        public void SetEcsComponentEnabled<TComponentData>(bool isComponentEnabled)
            where TComponentData : unmanaged, IComponentData, IEnableableComponent
            => _entityManager.SetComponentEnabled<TComponentData>(Entity, isComponentEnabled);

        public void AssureBuffer<TBufferData>() where TBufferData : unmanaged, IBufferElementData
        {
            if (_entityManager.HasComponent<TBufferData>(Entity)) { return; }

            _entityManager.AddBuffer<TBufferData>(Entity);
        }

        public DynamicBuffer<TBufferData> GetBuffer<TBufferData>() where TBufferData : unmanaged, IBufferElementData
            => _entityManager.GetBuffer<TBufferData>(Entity);

        public void AddToBuffer<TBufferData>(TBufferData value) where TBufferData : unmanaged, IBufferElementData
        {
            DynamicBuffer<TBufferData> buffer = GetBuffer<TBufferData>();
            buffer.Add(value);
        }

        public void RemoveFromBuffer<TBufferData>(TBufferData value) where TBufferData : unmanaged, IBufferElementData
        {
            int                        index  = -1;
            DynamicBuffer<TBufferData> buffer = GetBuffer<TBufferData>();
            for (int i = 0; i < buffer.Length; i++)
            {
                if (!buffer[i].DefaultEqual(value)) { continue; }

                index = i;
                break;
            }

            if (index >= 0) { buffer.RemoveAtSwapBack(index); }
        }

        // --------------- ECS SYSTEMS --------------- //
        public ref SystemState GetSystemHandle<T>() where T : ISystem => ref _world.Unmanaged.GetExistingSystemState<T>();

        // --------------- ARCHETYPE CHANGER --------------- //
        public void AddEcsComponent<TComponentData>(TComponentData value) where TComponentData : unmanaged, IComponentData
            => _entityManager.AddComponentData<TComponentData>(Entity, value);

        public void RemoveEcsComponent<TComponentData>() where TComponentData : unmanaged, IComponentData
            => _entityManager.RemoveComponent<TComponentData>(Entity);
        
        public void RemoveEcsBuffer<TBuffer>() where TBuffer : unmanaged, IBufferElementData
            => _entityManager.RemoveComponent<TBuffer>(Entity);
    }
}
