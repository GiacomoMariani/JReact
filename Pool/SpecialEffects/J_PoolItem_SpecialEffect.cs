using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Pool.SpecialEffect
{
    /// <summary>
    /// the structure to create a special effect
    /// </summary>
    public abstract class J_PoolItem_SpecialEffect : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        public event Action<J_PoolItem_SpecialEffect> OnActivation;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Pool_SpecialEffects _pool;
        
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId = -1;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            InitThis();
            SanityChecks();
        }

        protected virtual void InitThis()     { _instanceId = GetHashCode(); }
        protected virtual void SanityChecks() {}

        // --------------- ACTIVATION AND DEACTIVATION --------------- //
        /// <summary>
        /// triggers all the effect of this item
        /// </summary>
        internal void Activate()
        {
            //activate, play and event
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            TriggerThisEffect();
            OnActivation?.Invoke(this);
        }

        /// <summary>
        /// an helper method if we want to time this effect
        /// </summary>
        /// <param name="secondsToWait">seconds to wait before disable</param>
        protected virtual void RemoveAfterSeconds(float secondsToWait)
        {
            Timing.RunCoroutine(PlayThanRemove(secondsToWait), Segment.LateUpdate, _instanceId,
                                JCoroutineTags.COROUTINE_SpecialEffectTag);
        }

        private IEnumerator<float> PlayThanRemove(float particleDuration)
        {
            yield return Timing.WaitForSeconds(particleDuration);
            EndEffect();
        }

        // --------------- ABSTRACT IMPLEMENTATION --------------- //
        /// <summary>
        /// this is the specific implementation of the effect
        /// </summary>
        protected abstract void TriggerThisEffect();

        /// <summary>
        /// this is mostly a method to remind to despawn this at the end of the effect
        /// </summary>
        protected virtual void EndEffect() { _pool.DeSpawn(gameObject); }
    }
}
