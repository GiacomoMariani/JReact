using UnityEngine;

namespace JReact.Pool
{
    public class J_Mono_PoolItem : MonoBehaviour, IPoolableItem<J_Mono_PoolItem>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        public IPool<J_Mono_PoolItem> Pool { get; private set; }

        public void SetPool(IPool<J_Mono_PoolItem> pool) { Pool = pool; }
        public void DeSpawn()                                       => Pool.DeSpawn(this.gameObject);
    }
}
