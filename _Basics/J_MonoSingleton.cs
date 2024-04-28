using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace JReact.Singleton
{
    public abstract class J_MonoSingleton : MonoBehaviour
    {
        [BoxGroup("Permanency", true, true, -10), ReadOnly, ShowInInspector] internal bool _isPermanent => _permanency != default;
        [BoxGroup("Permanency", true, true, -10), ReadOnly, ShowInInspector] private J_PermanentGameObject _permanency;

#if UNITY_EDITOR
        [BoxGroup("Permanency", true, true, -10), ReadOnly, ShowInInspector, Button]
        private void TogglePermanent()
        {
            if (_isPermanent) { UnsetPermanent(); }
            else { SetPermanent(); }
        }

        private void SetPermanent()   { transform.root.gameObject.AddComponent<J_PermanentGameObject>(); }
        private void UnsetPermanent() { _permanency.AutoDestroy(); }

#endif

        protected internal virtual void InitThis() {}

        protected internal virtual void TriggerDestroy() {}

        private void OnValidate() { _permanency = transform.root.GetComponent<J_PermanentGameObject>(); }
    }

    public abstract class J_MonoSingleton<T> : J_MonoSingleton where T : J_MonoSingleton
    {
        // --------------- EVENTS --------------- //
        public static event Action<T> OnInitSingleton;
        public static event Action<T> OnDestroySingleton;

        // --------------- STATIC DATA --------------- //
        public static bool IsSingletonAlive { get; private set; } = false;
        private static T _Instance;
        public static T InstanceUnsafe => _Instance;
        public static bool IsReady() => _Instance != default;
        private static readonly string _WaitOperation = $"WaitForSingleton-{typeof(T)}";

        // --------------- INITIALIZATION --------------- //
        protected virtual void Awake()
        {
            if (_Instance == this) { return; }

            //destroy this instance if the other is permanent
            if (_Instance != null &&
                _Instance._isPermanent)
            {
                gameObject.AutoDestroy();
                return;
            }

            DestroyInstance();
            AssignInstance((T)(object)this);
        }

        /// <summary>
        /// waits the init of the singleton and returns it
        /// </summary>
        /// <returns>returns the single after we have an instance</returns>
        public static async UniTask<T> WaitForInit(Object caller = default)
        {
            if (await J_Async_Utils.WaitUntilReady(IsReady, _WaitOperation, caller: caller)) { return InstanceUnsafe; }
            else { return default; }
        }

        private static void AssignInstance(T instance)
        {
            Assert.IsTrue(FindObjectsByType<T>(FindObjectsSortMode.InstanceID).Length == 1,
                          $"Only one {nameof(T)} required. In scene: {FindObjectsByType<T>(FindObjectsSortMode.InstanceID).Length}");

            _Instance        = instance;
            IsSingletonAlive = true;
            instance.InitThis();
            OnInitSingleton?.Invoke(_Instance);
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
            return _Instance;
        }

        public static T GetInstanceSafe()
        {
            if (InstanceUnsafe != null) { return InstanceUnsafe; }

            DestroyInstance();
            var singletonObject = FindAnyObjectByType<T>(FindObjectsInactive.Include);
            Assert.IsNotNull(singletonObject, $"{nameof(singletonObject)} not found in the scene");
            AssignInstance(singletonObject);
            return InstanceUnsafe;
        }

        public static void DestroyInstance()
        {
            if (_Instance == null) { return; }

            JLog.Log($"{typeof(T)} - Removing {_Instance.gameObject}", JLogTags.Infrastructure, _Instance.gameObject);

            _Instance.TriggerDestroy();
            IsSingletonAlive = false;
            OnDestroySingleton?.Invoke(_Instance);
            _Instance.AutoDestroy();
            _Instance = default;
        }

        // --------------- UNITY EVENTS --------------- //
        protected virtual void OnApplicationQuit() { IsSingletonAlive = false; }

        protected virtual void OnDestroy()
        {
            if (_Instance != this) { return; }

            TriggerDestroy();
            IsSingletonAlive = false;
        }
    }
}
