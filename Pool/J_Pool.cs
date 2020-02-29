using System;
using System.Collections.Generic;
using MEC;
using NUnit.Framework.Internal.Commands;
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

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Count => _poolStack.Count;

        // --------------- CREATION --------------- //
        public void SetupPool(Transform parent = null, ushort population = DefaultAmount, bool instantPopulation = false)
        {
            _parentTransform = parent == null
                                   ? new GameObject($"{name}_Pool").transform
                                   : parent;

            // --------------- SETUP --------------- //
            _instanceId  = GetInstanceID();
            _poolStack   = new Stack<T>(population);
            _spawnedDict = new Dictionary<GameObject, T>(population);
            // --------------- START RECURSION --------------- //
            Populate(population, instantPopulation);
        }

        // --------------- INITIALIZATION --------------- //
        //checks that everything has been setup properly
        private void SanityChecks()
        {
            Assert.IsNotNull(_parentTransform,  $"{name} requires an element for {nameof(_parentTransform)}");
            Assert.IsNotNull(_prefabVariations, $"{name} requires an element for {nameof(_prefabVariations)}");
            Assert.IsNotNull(_poolStack,        $"{name} not initialized. {nameof(_poolStack)} is null. ");
        }

        private void Populate(int remaining, bool instantPopulation)
        {
            // --------------- ITEM CREATION --------------- //
            T itemToAdd = AddItemIntoPool();
            SetupItemAtAdd(itemToAdd);

            // --------------- CHECK --------------- //
            //check is set at the end to avoid a unnecessary coroutine
            remaining--;
            if (remaining <= 0) return;

            // --------------- MOVE NEXT --------------- //
            if (instantPopulation) Populate(remaining, true);
            else Timing.RunCoroutine(WaitAndPopulate(remaining), Segment.SlowUpdate, _instanceId, PoolTag);
        }
        
        private IEnumerator<float> WaitAndPopulate(int remaining)
        {
            yield return Timing.WaitForOneFrame;
            Populate(remaining, false);
        }

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// gets an element from the pool
        /// creates a new element if there are no available
        /// </summary>
        /// <returns>an item taken the pool</returns>
        public T Spawn()
        {
            if (_poolStack == null) SetupPool();

            //check if the first element in the pool is missing, otherwise add one
            if (Count == 0) AddItemIntoPool();

            //update the elements and return the next one 
            T item = _poolStack.Pop();
            _spawnedDict[item.gameObject] = item;
            SetupItemBeforeSpawn(item);
            return item;
        }


        public void DeSpawn(GameObject itemGameObject)
        {
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

        //sets the item at the end of the pool
        private void PlaceInPool(T item)
        {
            //disable the item if requested
            if (_disableItemInPool && item.gameObject.activeSelf) item.gameObject.SetActive(false);
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

        public bool IsInPool(T item) => _poolStack.Contains(item);

        // --------------- VIRTUAL IMPLEMENTATION --------------- //
        protected virtual void SetupItemAtAdd(T item) { }
        protected virtual void SetupItemBeforeSpawn(T item) {  }

#if UNITY_EDITOR
        //used only for the testrunner
        public void SetupWithPrefabs(T[] prefabs, ushort amount = DefaultAmount, bool instantPopulation = false)
        {
            _prefabVariations = prefabs;
            SetupPool(population: amount, instantPopulation: instantPopulation);
        }
#endif
    }
}
