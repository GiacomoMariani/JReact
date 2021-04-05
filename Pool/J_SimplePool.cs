using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Pool
{
    /// <summary>
    /// a simple pool that may contain only one prefab that might be generated on the spot
    /// </summary>
    /// <typeparam name="T">the prefab we want to pool</typeparam>
    public sealed class J_SimplePool<T> : IPool<T>
        where T : MonoBehaviour
    {
        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private T _prefab;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Transform _parentTransform;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Dictionary<GameObject, T> _spawnedDict;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Stack<T> _pool;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private CoroutineHandle _generationHandle;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Count => _pool.Count;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private static Dictionary<int, J_SimplePool<T>> _AllPools = new Dictionary<int, J_SimplePool<T>>(20);

        // --------------- CREATION --------------- //
        public static J_SimplePool<T> GetPool(T prefab, int population = 25, int spawnPerFrame = 25, Transform parentTransform = null)
        {
            var instanceId = prefab.GetHashCode();
            if (!_AllPools.ContainsKey(instanceId))
            {
                _AllPools[instanceId] = new J_SimplePool<T>(prefab, population, spawnPerFrame, parentTransform);
            }

            return _AllPools[instanceId];
        }

        private J_SimplePool(T prefab, int population = 25, int spawnPerFrame = 25, Transform parentTransform = null)
        {
            _prefab = prefab;
            _parentTransform = parentTransform == null
                                   ? new GameObject($"{prefab.name}_Pool").transform
                                   : parentTransform;

            _spawnedDict = new Dictionary<GameObject, T>(population);
            _pool        = new Stack<T>(population);

            //if we are re using a pool we do not need to have more population
            population -= _pool.Count;

            if (population > 0) { Populate(population, spawnPerFrame); }
        }

        //ad a given amount of items to the pool
        private void Populate(int remaining, int amountToAddPerFrame)
        {
            _generationHandle = Timing.RunCoroutine(WaitAndPopulate(remaining, amountToAddPerFrame), Segment.SlowUpdate);
        }

        //used to populate the list frame by frame
        private IEnumerator<float> WaitAndPopulate(int remaining, int amountToAddPerFrame)
        {
            while (remaining > 0)
            {
                if (remaining < amountToAddPerFrame) { amountToAddPerFrame = remaining; }

                // --------------- ITEM CREATION --------------- //
                for (int i = 0; i < amountToAddPerFrame; i++)
                {
                    T itemToAdd = AddItemIntoPool();
                }

                // --------------- CHECK --------------- //
                remaining -= amountToAddPerFrame;
                if (remaining <= 0) { yield break; }

                yield return Timing.WaitForOneFrame;
            }
        }

        // --------------- COMMANDS - SPAWN --------------- //
        /// <summary>
        /// gets an element from the pool
        /// creates a new element if there are no available
        /// </summary>
        /// <returns>an item taken the pool</returns>
        public T Spawn(Transform parent = null)
        {
            //check if the first element in the pool is missing, otherwise add one
            if (_pool.Count == 0) { AddItemIntoPool(); }

            //update the elements and return the next one 
            T item = _pool.Pop();
            _spawnedDict[item.gameObject] = item;
            if (parent != null) { item.transform.SetParent(parent); }

            item.gameObject.SetActive(true);
            return item;
        }

        /// <summary>
        /// a fast spawn system to instantiate the item on a parent with no change
        /// </summary>
        /// <param name="parent">the parent where to set the item,</param>
        /// <returns>returns the spawned item</returns>
        public T SpawnInstantiate(Transform parent)
        {
            T item = Object.Instantiate(_prefab, parent);
            if (item is IPoolableItem<T> poolableItem) { poolableItem.SetPool(this); }

            _spawnedDict[item.gameObject] = item;
            return item;
        }

        // --------------- COMMANDS - DESPAWN --------------- //
        /// <summary>
        /// place the item back in the pool
        /// </summary>
        /// <param name="itemGameObject">the item to set back, as gameobject</param>
        public void DeSpawn(GameObject itemGameObject)
        {
            if (!_spawnedDict.ContainsKey(itemGameObject))
            {
                JLog.Warning($"Does not contain the item {itemGameObject}", JLogTags.Pool, itemGameObject);
                return;
            }

            T item = _spawnedDict[itemGameObject];
            _spawnedDict.Remove(itemGameObject);
            if (_pool.Contains(item)) { JLog.Warning($"{itemGameObject} was already in the pool.", JLogTags.Pool, itemGameObject); }
            else PlaceInPool(item);
        }

        /// <summary>
        /// place the item back in the pool
        /// </summary>
        /// <param name="item">the item to despawn</param>
        public void DeSpawn(T item)
        {
            Assert.IsFalse(_pool.Contains(item), $"{item.gameObject} was already in the pool.");
            _spawnedDict.Remove(item.gameObject);
            PlaceInPool(item);
        }

        //sets the item at the end of the pool
        private void PlaceInPool(T item)
        {
            //disable the item if requested
            item.gameObject.SetActive(false);

            item.transform.SetParent(_parentTransform);
            _pool.Push(item);
        }

        private T AddItemIntoPool()
        {
            T item = Object.Instantiate(_prefab, _parentTransform);
            PlaceInPool(item);
            if (item is IPoolableItem<T> poolableItem) { poolableItem.SetPool(this); }

            return item;
        }

        // --------------- CLEAR --------------- //
        /// <summary>
        /// remove all the items still in the pool
        /// </summary>
        public static void DestroyPoolFor(T prefab)
        {
            var instanceId = prefab.GetHashCode();
            if (!_AllPools.ContainsKey(instanceId)) { return; }

            var poolToClear = _AllPools[instanceId];
            while (poolToClear.Count > 0) { poolToClear.DestroyPool(); }
        }

        public void DestroyPool()
        {
            while (_pool.Count > 0) { _pool.Pop().gameObject.AutoDestroy(); }
        }

        // --------------- QUERIES --------------- //
        public T Peek() => _pool.Peek();

        public bool IsSpawned(GameObject go)   => _spawnedDict.ContainsKey(go);
        public bool IsSpawned(T          item) => _spawnedDict.ContainsKey(item.gameObject);
        public bool IsInPool(T           item) => _pool.Contains(item);
    }
}
