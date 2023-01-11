using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Singleton
{
    public abstract class J_MonoSingleton : MonoBehaviour
    {
        [BoxGroup("Setup", true, true, 0), SerializeField] internal bool _permanentInstance;

        protected internal virtual void InitThis() {}

        protected internal virtual void TriggerDestroy() {}
    }

    public abstract class J_MonoSingleton<T> : J_MonoSingleton where T : J_MonoSingleton
    {
        // --------------- EVENTS --------------- //
        public static event Action<T> OnInitSingleton;
        public static event Action<T> OnDestroySingleton;

        // --------------- STATIC DATA --------------- //
        public static bool IsSingletonAlive { get; private set; } = false;
        private static T _instance;
        public static T InstanceUnsafe => _instance;

        // --------------- INITIALIZATION --------------- //
        protected virtual void Awake()
        {
            if (_instance == this) { return; }

            //destroy this instance if the other is permanent
            if (_instance != null &&
                _instance._permanentInstance)
            {
                gameObject.AutoDestroy(); 
                return;
            }

            DestroyInstance();
            AssignInstance((T)(object)this);
        }

        private static void AssignInstance(T instance)
        {
            _instance        = instance;
            IsSingletonAlive = true;
            instance.InitThis();
            if (InstanceUnsafe._permanentInstance)
            {
                Assert.IsTrue(InstanceUnsafe.transform.root == InstanceUnsafe.transform,
                              $"{InstanceUnsafe.name} should be a root transform for a permanent instance");

                DontDestroyOnLoad(InstanceUnsafe);
            }

            OnInitSingleton?.Invoke(_instance);
            JLog.Log($"{instance.name} - Singleton init: {typeof(T)}", JLogTags.Infrastructure, instance);
        }

        // --------------- DIRECT COMMANDS --------------- //
        public static T ForceCreateInstance()
        {
            DestroyInstance();
            var singletonObject = new GameObject();
#if UNITY_EDITOR
            singletonObject.name = $"{typeof(T)}_Singleton";
#endif
            AssignInstance(singletonObject.AddComponent<T>());
            return _instance;
        }

        public static T GetInstanceSafe()
        {
            if (InstanceUnsafe != null) { return InstanceUnsafe; }

            DestroyInstance();
            Assert.IsTrue(FindObjectsOfType<T>().Length == 1,
                          $"Only one {nameof(T)} required. In scene: {FindObjectsOfType<T>().Length}");

            var singletonObject = FindObjectOfType<T>();
            Assert.IsNotNull(singletonObject, $"{nameof(singletonObject)} not found in the scene");
            AssignInstance(singletonObject);
            return InstanceUnsafe;
        }

        public static void DestroyInstance()
        {
            if (_instance == null ||
                InstanceUnsafe._permanentInstance) { return; }

            JLog.Log($"{typeof(T)} - Removing {_instance.gameObject}", JLogTags.Infrastructure, _instance.gameObject);

            _instance.TriggerDestroy();
            IsSingletonAlive = false;
            OnDestroySingleton?.Invoke(_instance);
            _instance.gameObject.AutoDestroy();
            _instance = null;
        }

        // --------------- UNITY EVENTS --------------- //
        protected virtual void OnApplicationQuit() { IsSingletonAlive = false; }

        protected virtual void OnDestroy()
        {
            TriggerDestroy();
            IsSingletonAlive = false;
        }
    }
}
