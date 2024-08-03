using System;
using Cysharp.Threading.Tasks;
using JReact.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.SceneControl
{
    public abstract class J_EntryPoint : J_MonoSingleton<J_EntryPoint>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        public static event Action<IJScene> OnSceneStart;
        public static event Action<IJScene> OnSceneExit;

        [BoxGroup("Null = no loading", true, true, 0), SerializeField, AssetsOnly] private J_SO_Scene _directReference;
        [FoldoutGroup("State", true, 5), ReadOnly, ShowInInspector] private IJScene _scene;
        [FoldoutGroup("State", true, 5), ReadOnly, ShowInInspector] public bool InitCompleted { get; private set; }

        // --------------- QUERY --------------- //
        public static bool IsSceneReady(IJScene scene)
            => scene != default && GetInstanceSafe().InitCompleted && InstanceUnsafe._scene == scene;

        // --------------- INIT --------------- //
        protected internal async override void InitThis()
        {
            base.InitThis();
            if (_directReference != default) { await InitScene(_directReference); }
        }

        public async UniTask InitScene(IJScene scene)
        {
            Assert.IsFalse(gameObject.IsPermanent(), $"{name} scene entry points needs to be unloaded with the scene");
            await InitSceneImpl(scene);
            _scene        = scene;
            InitCompleted = true;
            OnSceneStart?.Invoke(_scene);
        }

        protected abstract UniTask InitSceneImpl(IJScene scene);

        // --------------- DE INIT --------------- //
        protected internal async override void TriggerDestroy()
        {
            base.TriggerDestroy();
            if (_directReference != default) { await DeInitScene(_directReference); }
        }

        public async UniTask DeInitScene(IJScene scene)
        {
            if (!InitCompleted) { return; }

            InitCompleted = false;
            await DeInitSceneImpl(scene);
            OnSceneExit?.Invoke(_scene);
        }

        protected abstract UniTask DeInitSceneImpl(IJScene scene);
    }
}
