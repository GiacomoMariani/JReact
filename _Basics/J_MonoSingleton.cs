using System;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace JReact.Singleton
{
    public sealed class J_SingletonInstance<T>
        where T : Object
    {
        private static T _instance;

        public static T GetUnsafe() => _instance;

        public static T Get()
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType<T>();
            }

            Assert.IsNotNull(_instance, $"Requires a {nameof(_instance)}");
            return _instance;
        }

        public static void SetInstance(T instance) => _instance = instance;
    }

    public abstract class J_MonoSingleton : MonoBehaviour
    {
        internal virtual void InitThis() {}

        internal virtual void TriggerDestroy() {}
    }

    public abstract class J_MonoSingleton<T> : J_MonoSingleton where T : J_MonoSingleton
    {
        // --------------- EVENTS --------------- //
        public static event Action<T> OnInitSingleton;
        public static event Action<T> OnDestroySingleton;

        // --------------- STATIC DATA --------------- //
        public static bool IsSingletonAlive { get; private set; } = false;
        private static T _instance;
        public static T Instance => _instance;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            if (_instance == this) { return; }

            DestroyInstance();
            AssignInstance((T) (object) this);
        }

        private static void AssignInstance(T instance)
        {
            _instance        = instance;
            IsSingletonAlive = true;
            instance.InitThis();
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

        public static T AssureInstanceInitialization()
        {
            if (Instance != null) { return Instance; }

            DestroyInstance();
            Assert.IsTrue(FindObjectsOfType<T>().Length == 1,
                          $"Only one {nameof(T)} required. In scene: {FindObjectsOfType<T>().Length}");

            var singletonObject = FindObjectOfType<T>();
            Assert.IsNotNull(singletonObject, $"{nameof(singletonObject)} not found in the scene");
            AssignInstance(singletonObject);
            return Instance;
        }

        public static void DestroyInstance()
        {
            if (_instance == null) { return; }

            JLog.Log($"{typeof(T)} - Removing {_instance.gameObject}", JLogTags.Infrastructure, _instance.gameObject);

            _instance.TriggerDestroy();
            IsSingletonAlive = false;
            OnDestroySingleton?.Invoke(_instance);
            _instance.gameObject.AutoDestroy();
            _instance = null;
        }

        // --------------- UNITY EVENTS --------------- //
        private void OnApplicationQuit() { IsSingletonAlive = false; }

        private void OnDestroy()
        {
            TriggerDestroy();
            IsSingletonAlive = false;
        }
    }
}
