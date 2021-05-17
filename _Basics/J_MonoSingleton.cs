using System;
using UnityEngine;

namespace JReact.Singleton
{
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
            if (_instance != null &&
                _instance != this)
            {
                JLog.Log($"{name} {typeof(T)} - replaces {_instance.gameObject}", JLogTags.Infrastructure, this);
                DestroyInstance();
            }

            if (_instance != this) { AssignInstance((T) (object) this); }
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

        public static void DestroyInstance()
        {
            if (_instance == null) { return; }

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
