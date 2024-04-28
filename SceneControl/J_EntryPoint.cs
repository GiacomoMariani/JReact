using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.SceneControl
{
    public abstract class J_EntryPoint : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        public static event Action<J_SO_Scene> OnStartCompleted;
        public static event Action<J_SO_Scene> OnExitCompleted;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_Scene _scene;
        [FoldoutGroup("State", true, 5), ReadOnly, ShowInInspector] public bool IsReady { get; private set; }

        // --------------- INIT --------------- //
        private async void Awake()
        {
            Assert.IsFalse(gameObject.IsPermanent(), $"{name} scene entry points needs to be unloaded with the scene");
            await InitScene();
            IsReady = true;
            OnStartCompleted?.Invoke(_scene);
        }

        protected virtual async UniTask InitScene() {}

        // --------------- DE INIT --------------- //
        private async void OnDestroy()
        {
            IsReady = false;
            await DeInitScene();
            OnExitCompleted?.Invoke(_scene);
        }

        protected virtual async UniTask DeInitScene() {}
    }
}
