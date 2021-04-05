using UnityEngine;

namespace JReact.Pool
{
    public interface IPool<T>
        where T : MonoBehaviour
    {
        public T    Spawn(Transform            parent = null);
        public T    SpawnInstantiate(Transform parent);
        public void DeSpawn(GameObject         itemGameObject);
        public void DeSpawn(T                  item);
        public T    Peek();
        public bool IsSpawned(GameObject go);
        public bool IsSpawned(T          item);
        public bool IsInPool(T           item);
    }

    public interface IPoolableItem<T>
        where T : MonoBehaviour
    {
        public void SetPool(IPool<T> pool);
    }
}
