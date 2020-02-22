using System;
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
        private const int DefaultPoolAmount = 10;

        // --------------- SETUP --------------- //
        //the prefabs are an array to differentiate them. Also an array of one can be used if we want always the same
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private T[] _prefabVariations;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly] private J_TransformGenerator _parentTransform;
        //set this to true if we want to disable items when they get back to pool
        [BoxGroup("Setup", true, true), SerializeField] private bool _disableItemInPool = true;
        [BoxGroup("Setup", true, true), SerializeField] private bool _instantPopulation;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 10), ReadOnly, ShowInInspector]
        private Dictionary<GameObject, T> _spawnedDict = new Dictionary<GameObject, T>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Stack<T> _poolStack;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId = -1;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Count => _poolStack.Count;

        // --------------- CREATION --------------- //
        public void SetupPool(ushort population = DefaultPoolAmount, bool instantPopulation = false)
        {
            if (_parentTransform == null) _parentTransform = J_TransformGenerator.CreateTransformGenerator($"{name}_Pool");
            _instantPopulation = instantPopulation;

            // --------------- SETUP --------------- //
            _instanceId = GetInstanceID();
            _poolStack  = new Stack<T>(population);
            // --------------- START RECURSION --------------- //
            Populate(population);
        }

        // --------------- INITIALIZATION --------------- //
        //checks that everything has been setup properly
        private void SanityChecks()
        {
            Assert.IsNotNull(_parentTransform,  $"{name} requires an element for {nameof(_parentTransform)}");
            Assert.IsNotNull(_prefabVariations, $"{name} requires an element for {nameof(_prefabVariations)}");
            Assert.IsNotNull(_poolStack,        $"{name} not initialized. {nameof(_poolStack)} is null. ");
        }

        private void Populate(int remainingObjects)
        {
            // --------------- ITEM CREATION --------------- //
            T itemToAdd = AddItemIntoPool();

            // --------------- CHECK --------------- //
            //check is set at the end to avoid a unnecessary coroutine
            remainingObjects--;
            if (remainingObjects <= 0) return;

            // --------------- MOVE NEXT --------------- //
            if (_instantPopulation) Populate(remainingObjects);
            else Timing.RunCoroutine(WaitAndPopulate(remainingObjects), Segment.SlowUpdate, _instanceId, PoolTag);
        }

        private IEnumerator<float> WaitAndPopulate(int remainingObjects)
        {
            yield return Timing.WaitForOneFrame;
            Populate(remainingObjects);
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
            if (_poolStack.Count == 0) AddItemIntoPool();

            //update the elements and return the next one 
            T element = _poolStack.Pop();
            _spawnedDict[element.gameObject] = element;
            return element;
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
        private void PlaceInPool(T itemToPool)
        {
            //disable the item if requested
            if (_disableItemInPool && itemToPool.gameObject.activeSelf) itemToPool.gameObject.SetActive(false);
            _poolStack.Push(itemToPool);
        }

        private T AddItemIntoPool()
        {
            T poolItem = Instantiate(_prefabVariations.GetRandomElement(), _parentTransform.ThisTransform);
            PlaceInPool(poolItem);
            return poolItem;
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

#if UNITY_EDITOR
        //used only for the testrunner
        public void SetupWithPrefabs(T[] prefabs, ushort population = DefaultPoolAmount, bool instantPopulation = false)
        {
            _prefabVariations = prefabs;
            SetupPool(population, instantPopulation);
        }
#endif
    }
}
