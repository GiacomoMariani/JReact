using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace JReact.Pool
{
    /// <summary>
    /// a simple pool that may contain only one prefab that might be generated on the spot
    /// </summary>
    /// <typeparam name="T">the prefab we want to pool</typeparam>
    public sealed class J_Pool<T> : IPool<T>
        where T : Component
    {
        // --------------- CONSTS --------------- //
        private const int ExpectedPools = 20;
        internal const int ExpectedItems = 25;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private T _prefab;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Transform _parentTransform;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Dictionary<GameObject, T> _spawnedDict;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Stack<T> _pool;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private CoroutineHandle _generationHandle;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int AmountInPool => _pool.Count;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int AmountSpawned => _spawnedDict.Count;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private static Dictionary<int, J_Pool<T>> _AllPools = new Dictionary<int, J_Pool<T>>(ExpectedPools);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private static Action<T> _actionOnGenerate;

        // --------------- CREATION --------------- //
        public static J_Pool<T> GetPool(T         prefab,        int       population = ExpectedItems, int perFrame = ExpectedItems,
                                        Transform parent = null, Action<T> action     = null)
        {
            Assert.IsNotNull(prefab, $"{nameof(T)} pool requires a {nameof(prefab)}");
            if (action != null) { _actionOnGenerate = action; }

            var instanceId = prefab.GetHashCode();

            if (!PoolIsReady(instanceId)) { _AllPools[instanceId] = new J_Pool<T>(prefab, parent, population, perFrame); }

            //if we are re using a pool we do not need to have more population
            var pool = GetPoolFromHashCode(instanceId);
            //in some cases, if we changed scene, the pool might become not valid
            if (!pool.IsValid()) { pool.Regenerate(population, perFrame, parent, action); }

            population -= pool.AmountInPool;
            if (population > 0) { pool.Populate(population, perFrame); }

            return _AllPools[instanceId];
        }

        public static bool PoolIsReady(int instanceId) => _AllPools.ContainsKey(instanceId);
        public static bool PoolIsReady(T   instance)   => _AllPools.ContainsKey(instance.GetHashCode());

        internal static J_Pool<T> GetPoolFromHashCode(int instanceId) => _AllPools[instanceId];

        private J_Pool(T prefab, Transform parentTransform, int population = ExpectedItems, int spawnPerFrame = ExpectedItems)
        {
            _prefab = prefab;
#if UNITY_EDITOR
            if (parentTransform == null) { parentTransform = new GameObject($"{prefab.gameObject.name}_Pool").transform; }
#endif

            _parentTransform = parentTransform;
            _spawnedDict     = new Dictionary<GameObject, T>(population);
            _pool            = new Stack<T>(population);

            if (population > 0) { Populate(population, spawnPerFrame); }
        }

        //ad a given amount of items to the pool
        private void Populate(int remaining, int amountToAddPerFrame)
        {
            _generationHandle = Timing.RunCoroutine(WaitAndPopulate(remaining, amountToAddPerFrame), Segment.SlowUpdate);
        }

        //used to populate the list frame by frame
        private IEnumerator<float> WaitAndPopulate(int remaining, int amountPerFrame)
        {
            while (remaining > 0)
            {
                if (remaining < amountPerFrame) { amountPerFrame = remaining; }

                // --------------- ITEM CREATION --------------- //
                for (int i = 0; i < amountPerFrame; i++) { AddItemIntoPool(); }

                // --------------- CHECK --------------- //
                remaining -= amountPerFrame;
                if (remaining <= 0) { yield break; }

                yield return Timing.WaitForOneFrame;
            }
        }

        // --------------- REGENERATES THIS POOL --------------- //
        /// <summary>
        /// regenerates the pool and make sure the previous elements are removed
        /// </summary>
        public void Regenerate(int       population = ExpectedItems, int perFrame = ExpectedItems, Transform parent = null,
                               Action<T> action     = null)
        {
            DestroyPool(true, false);
#if UNITY_EDITOR
            if (parent == null) { parent = new GameObject($"{_prefab.gameObject.name}_Pool").transform; }
#endif
            if (action != null) { _actionOnGenerate = action; }

            _parentTransform = parent;

            Assert.IsTrue(AmountInPool  == 0, $"Remaining items {AmountInPool}, pool was not destroyed correctly");
            Assert.IsTrue(AmountSpawned == 0, $"Remaining spawned items {AmountSpawned}");

            if (population > 0) { Populate(population, perFrame); }
        }

        // --------------- COMMANDS - SPAWN --------------- //
        /// <summary>
        /// gets an element from the pool
        /// creates a new element if there are no available
        /// </summary>
        /// <param name="parent">the parent where to set the item,</param>
        /// <param name="worldPositionStays">decide if we want to keep the world position, defaults at true</param>
        /// <returns>an item taken the pool</returns>
        public T Spawn(Transform parent = null, bool worldPositionStays = true)
        {
            //check if the first element in the pool is missing, otherwise add one
            if (_pool.Count == 0) { AddItemIntoPool(); }

            //update the elements and return the next one 
            T item = _pool.Pop();
            
            //safety check, some items might get null after scene change
            if (item.IsNull()) { item = SpawnInstantiate(parent, worldPositionStays); }

            _spawnedDict[item.gameObject] = item;
            if (parent != null) { item.transform.SetParent(parent, worldPositionStays); }

            item.gameObject.SetActive(true);
            return item;
        }

        /// <summary>
        /// spawn an item directly at position
        /// </summary>
        /// <param name="position">the position where to spawn the item</param>
        /// <param name="parent">the parent where to set the item,</param>
        /// <param name="worldPositionStays">decide if we want to keep the world position, defaults at true</param>
        /// <returns>an item taken the pool</returns>
        public T SpawnAtPosition(Vector3 position, Transform parent = null, bool worldPositionStays = true)
        {
            var item = Spawn(parent, worldPositionStays);
            item.transform.position = position;
            return item;
        }

        /// <summary>
        /// a fast spawn system to instantiate the item on a parent with no change
        /// </summary>
        /// <param name="parent">the parent where to set the item,</param>
        /// <param name="worldPositionStays">decide if we want to keep the world position, defaults at true</param>
        /// <returns>returns the spawned item</returns>
        public T SpawnInstantiate(Transform parent, bool worldPositionStays = true)
        {
            T item = Object.Instantiate(_prefab, parent, worldPositionStays);
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
            else { PlaceInPool(item); }
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
            item.gameObject.SetActive(false);
            item.transform.SetParent(_parentTransform, false);
            _pool.Push(item);
        }

        //adds an item to the pool
        private T AddItemIntoPool()
        {
            T item = Object.Instantiate(_prefab, _parentTransform, false);
            if (_actionOnGenerate != null) { _actionOnGenerate.Invoke(item); }

            PlaceInPool(item);
            if (item is IPoolableItem<T> poolableItem) { poolableItem.SetPool(this); }

            return item;
        }

        // --------------- CLEAR --------------- //
        /// <summary>
        /// remove all the items still in the pool
        /// </summary>
        /// <param name="alsoSpawned">true if we want to remover also spawned items</param>
        /// <param name="unRegister">remove the pool from the registered pool, this is a very edge case if we want to create an independent pool</param>
        [ButtonGroup("Commands"), Button(ButtonSizes.Medium)]
        public void DestroyPool(bool alsoSpawned, bool unRegister = false)
        {
            if (alsoSpawned) { DespawnAll(); }

            while (AmountInPool > 0)
            {
                var item = _pool.Pop();
                if (!item.IsNull()) { item.gameObject.AutoDestroy(); }
            }

            _pool.Clear();

            if (_parentTransform != null) { _parentTransform.gameObject.AutoDestroy(); }

            if (unRegister) { UnregisterPool(); }
        }

        /// <summary>
        /// removes the pool from the register, and makes it independent from the main pool
        /// </summary>
        public void UnregisterPool() { _AllPools.Remove(_prefab.GetHashCode()); }

        /// <summary>
        /// despawn all items spawned from this pool
        /// </summary>
        [ButtonGroup("Commands"), Button(ButtonSizes.Medium)]
        public void DespawnAll()
        {
            int                                        currentTotal = AmountSpawned;
            using Dictionary<GameObject, T>.Enumerator enumerator   = _spawnedDict.GetEnumerator();
            T[]                                        spawned      = new T[currentTotal];
            int                                        index        = 0;
            while (enumerator.MoveNext())
            {
                KeyValuePair<GameObject, T> element = enumerator.Current;
                spawned[index] = element.Value;
                index++;
            }

            for (int i = 0; i < currentTotal; i++)
            {
                if (!spawned[i].IsNull()) { DeSpawn(spawned[i]); }
            }

            _spawnedDict.Clear();
        }

        // --------------- QUERIES --------------- //
        public T Peek() => _pool.Peek();

        public bool IsSpawned(GameObject go)   => _spawnedDict.ContainsKey(go);
        public bool IsSpawned(T          item) => _spawnedDict.ContainsKey(item.gameObject);
        public bool IsInPool(T           item) => _pool.Contains(item);

        public T   GetPrefab() => _prefab;
        public int GetId()     => _prefab.GetHashCode();

        /// <summary>
        /// the pool is no more alive when we sent the DestroyPool command
        /// </summary>
        /// <returns>true is the pool is still ready</returns>
        public bool IsRegistered() => PoolIsReady(_prefab);

        /// <summary>
        /// the pool is valid if it's been initiated and, has no null items
        /// </summary>
        /// <returns></returns>
        public bool IsValid() => _pool != null && (_pool.Count == 0 || !_pool.Peek().IsNull());
    }
}
