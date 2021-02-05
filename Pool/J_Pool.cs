using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Pool
{
    /// <summary>
    /// implements a pool of monobehaviours
    /// like explained http://www.gameprogrammingpatterns.com/object-pool.html
    /// </summary>
    public abstract class J_Pool<T> : ScriptableObject
        where T : MonoBehaviour
    {
        // --------------- CONST --------------- //
        public const string PoolTag = "J_PoolTag";
        private const int DefaultAmount = 10;

        // --------------- SETUP --------------- //
        //the prefabs are an array to differentiate them. Also an array of one can be used if we want always the same
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private T[] _prefabVariations;
        //set this to true if we want to disable items when they get back to pool
        [BoxGroup("Setup", true, true), SerializeField] private bool _disableItemInPool = true;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Transform _parentTransform;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Dictionary<GameObject, T> _spawnedDict;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Stack<T> _poolStack;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId = -1;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Count => _poolStack?.Count ?? 0;

        // --------------- CREATION --------------- //
        /// <summary>
        /// sets up the pool with a given amount of population. If required the instantiation might be placed in different frames
        /// </summary>
        /// <param name="parent">the parent transform where we want to place the items</param>
        /// <param name="population">the amount of items we want to start with in the pool. Set to 0 if you want just to instantiate the pool</param>
        /// <param name="maxPerFrame">the amount we want to instantiate per frame. If set to 0 we automatically set them equal to population to ushort.MaxValue max to populate. must be higher than 0</param>
        public void SetupPoolFor(Transform parent = null, ushort population = DefaultAmount, ushort maxPerFrame = ushort.MaxValue)
        {
            PrePoolSetup();
            _parentTransform = parent == null
                                   ? new GameObject($"{name}_Pool").transform
                                   : parent;

            // --------------- SETUP --------------- //
            _instanceId  = GetInstanceID();
            _poolStack   = new Stack<T>(population);
            _spawnedDict = new Dictionary<GameObject, T>(population);
            SanityChecks(maxPerFrame);

            // --------------- START RECURSION --------------- //
            if (population > 0) { Populate(population, maxPerFrame); }
        }

        /// <summary>
        /// setup the pool, used for backward compatibility
        /// </summary>
        public void SetupPool(Transform parent = null, ushort population = DefaultAmount, bool instantPopulation = false)
        {
            ushort maxPerFrame = population;
            if (!instantPopulation) { maxPerFrame = 1; }

            SetupPoolFor(parent, population, maxPerFrame);
        }

        // --------------- INITIALIZATION --------------- //
        //checks that everything has been setup properly
        private void SanityChecks(ushort maxPerFrame)
        {
            Assert.IsTrue(maxPerFrame > 0, $"{name} - {maxPerFrame} needs to be above 0");
            Assert.IsNotNull(_parentTransform,  $"{name} requires an element for {nameof(_parentTransform)}");
            Assert.IsNotNull(_prefabVariations, $"{name} requires an element for {nameof(_prefabVariations)}");
            Assert.IsNotNull(_poolStack,        $"{name} not initialized. {nameof(_poolStack)} is null. ");
        }

        //ad a given amount of items to the pool
        private void Populate(ushort remaining, ushort amountToAddPerFrame)
        {
            if (remaining < amountToAddPerFrame) { amountToAddPerFrame = remaining; }

            // --------------- ITEM CREATION --------------- //
            for (int i = 0; i < amountToAddPerFrame; i++)
            {
                T itemToAdd = AddItemIntoPool();
                SetupItemAtAdd(itemToAdd);
            }

            // --------------- CHECK --------------- //
            //check is set at the end to avoid a unnecessary coroutine
            remaining -= amountToAddPerFrame;
            if (remaining <= 0) return;

            // --------------- MOVE NEXT --------------- //
            Timing.RunCoroutine(WaitAndPopulate(remaining, amountToAddPerFrame), Segment.SlowUpdate, _instanceId, PoolTag);
        }

        //used to populate the list frame by frame
        private IEnumerator<float> WaitAndPopulate(ushort remaining, ushort amountToAddPerFrame)
        {
            yield return Timing.WaitForOneFrame;
            Populate(remaining, amountToAddPerFrame);
        }

        // --------------- COMMANDS - SPAWN --------------- //
        /// <summary>
        /// gets an element from the pool
        /// creates a new element if there are no available
        /// </summary>
        /// <returns>an item taken the pool</returns>
        public T Spawn(Transform parent = null)
        {
            Assert.IsNotNull(_poolStack, $"{name} requires a {nameof(_poolStack)} - pool not initialized");

            //check if the first element in the pool is missing, otherwise add one
            if (Count == 0) { AddItemIntoPool(); }

            //update the elements and return the next one 
            T item = _poolStack.Pop();
            _spawnedDict[item.gameObject] = item;
            if (parent != null) { item.transform.SetParent(parent); }

            SetupItemBeforeSpawn(item);
            return item;
        }

        /// <summary>
        /// a fast spawn system to instantiate the item on a parent with no change
        /// </summary>
        /// <param name="parent">the parent where to set the item,</param>
        /// <returns>returns the spawned item</returns>
        public T SpawnInstantiate(Transform parent)
        {
            Assert.IsNotNull(_poolStack, $"{name} requires a {nameof(_poolStack)} - pool not initialized");
            Assert.IsNotNull(parent,     $"{name} requires a {nameof(parent)}, there's no default in this case, to avoid the if");
            T item = Instantiate(_prefabVariations.GetRandomElement(), parent);
            _spawnedDict[item.gameObject] = item;
            SetupItemBeforeSpawn(item);
            return item;
        }

        // --------------- COMMANDS - DESPAWN --------------- //
        /// <summary>
        /// place the item back in the pool
        /// </summary>
        /// <param name="itemGameObject">the item to set back, as gameobject</param>
        public void DeSpawn(GameObject itemGameObject)
        {
            Assert.IsNotNull(_poolStack, $"{name} requires a {nameof(_poolStack)} - pool not initialized");
            if (!_spawnedDict.ContainsKey(itemGameObject))
            {
                JLog.Warning($"{name} does not contain the item {itemGameObject}", JLogTags.Pool, this);
                return;
            }

            T item = _spawnedDict[itemGameObject];
            _spawnedDict.Remove(itemGameObject);
            if (_poolStack.Contains(item))
            {
                JLog.Warning($"{name} - {itemGameObject} was already in the pool.", JLogTags.Pool, this);
            }
            else PlaceInPool(item);
        }

        /// <summary>
        /// place the item back in the pool
        /// </summary>
        /// <param name="item">the item to despawn</param>
        public void DeSpawn(T item)
        {
            Assert.IsFalse(_poolStack.Contains(item), $"{name} - {item.gameObject} was already in the pool.");
            _spawnedDict.Remove(item.gameObject);
            PlaceInPool(item);
        }

        //sets the item at the end of the pool
        private void PlaceInPool(T item)
        {
            //disable the item if requested
            if (_disableItemInPool && item.gameObject.activeSelf) { item.gameObject.SetActive(false); }

            item.transform.SetParent(_parentTransform);
            _poolStack.Push(item);
        }

        private T AddItemIntoPool()
        {
            T item = Instantiate(_prefabVariations.GetRandomElement(), _parentTransform);
            PlaceInPool(item);
            return item;
        }

        // --------------- QUERIES --------------- //
        public T Peek()
        {
            if (_poolStack == null) SetupPool();
            return _poolStack.Peek();
        }

        public bool IsSpawned(GameObject go)   => _spawnedDict.ContainsKey(go);
        public bool IsSpawned(T          item) => _spawnedDict.ContainsKey(item.gameObject);
        public bool IsInPool(T           item) => _poolStack.Contains(item);

        // --------------- VIRTUAL IMPLEMENTATION --------------- //
        protected virtual void PrePoolSetup()               {}
        protected virtual void SetupItemAtAdd(T       item) {}
        protected virtual void SetupItemBeforeSpawn(T item) {}
#if UNITY_EDITOR
        //used only for the testrunner
        public void SetupWithPrefabs(T[] prefabs, ushort amount = DefaultAmount, ushort amountPerFrame = DefaultAmount)
        {
            _prefabVariations = prefabs;
            SetupPoolFor(population: amount, maxPerFrame: amountPerFrame);
        }
#endif
    }
}
