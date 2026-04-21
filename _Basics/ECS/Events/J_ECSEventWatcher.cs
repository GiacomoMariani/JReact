#if UNITY_DOTS
using System.Collections.Generic;
using JReact.JEntities;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace JECS
{
    public abstract class J_ECSEventWatcher<T> : MonoBehaviour
        where T : unmanaged, IBufferElementData
    {
        [BoxGroup("Setup", true, true, 0), SerializeField] private J_MonoEntity _monoEntity;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private EntityManager _em;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _version;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private List<J_ECSEventListener<T>> _listeners = new List<J_ECSEventListener<T>>();

        protected virtual void Awake()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            if (world == null ||
                !world.IsCreated) return;

            _em   = world.EntityManager;
        }

        protected virtual void Update()
        {
            if (!_monoEntity.IsReady) { return; }

            Entity targetEntity = _monoEntity.Entity;

            if (!_monoEntity.HasEcsBuffer<T>()) { return; }

            var buffer = _em.GetBuffer<T>(targetEntity);

            for (int i = 0; i < buffer.Length; i++)
            {
                T evt = buffer[i];
                OnEvent(evt, _version);
            }

            _version++;
            _em.CompleteDependencyBeforeRW<T>();
            buffer.Clear();
        }

        private void OnEvent(T evt, int version)
        {
            for (int i = 0; i < _listeners.Count; i++) { _listeners[i].OnEvent(evt, version); }
        }

        public void Subscribe(J_ECSEventListener<T> listener)
        {
            if (!_listeners.Contains(listener)) { _listeners.Add(listener); }
        }

        public void Unsubscribe(J_ECSEventListener<T> listener)
        {
            if (_listeners.Contains(listener)) { _listeners.Add(listener); }
        }
    }
}
#endif
