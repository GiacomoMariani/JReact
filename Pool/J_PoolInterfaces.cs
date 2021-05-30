using UnityEngine;

namespace JReact.Pool
{
    public interface IPool<T>
        where T : Component
    {
        public T    Spawn(Transform            parent = null, bool worldPositionStays = true);
        public T    SpawnInstantiate(Transform parent,        bool worldPositionStays = true);
        public void DeSpawn(GameObject         itemGameObject);
        public void DeSpawn(T                  item);
        public T    Peek();
        public bool IsSpawned(GameObject go);
        public bool IsSpawned(T          item);
        public bool IsInPool(T           item);
    }

    public interface IPoolableItem<T>
        where T : Component
    {
        public void SetPool(IPool<T> pool);
    }
}
