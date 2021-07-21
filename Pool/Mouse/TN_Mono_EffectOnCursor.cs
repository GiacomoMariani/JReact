using System.Collections.Generic;
using JReact.Pool.SpecialEffect;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Pool.Mouse
{
    public class TN_Mono_EffectOnCursor : MonoBehaviour
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup - Cursor", true, true), SerializeField, AssetsOnly, Required]
        private J_Mono_FollowPointer _pointerTracker;

        [BoxGroup("Setup - Effect", true, true, 5), SerializeField, AssetsOnly] private J_Pool_SpecialEffects _effect;
        [BoxGroup("Setup - Effect", true, true, 5), SerializeField, Range(0.5f, 15f)]
        private float _interval = 1f;

        // --------------- STATE --------------- //
        [FoldoutGroup("State - Effects", false, 25), ReadOnly, ShowInInspector] private bool _activeEffect;

        // --------------- COMMANDS - EFFECT --------------- //
        /// <summary>
        /// applies an effect to spawn on mouse
        /// </summary>
        /// <param name="specialEffect">the effect to spawn</param>
        /// <param name="interval">the interval for the effect</param>
        public void SetEffect(J_Pool_SpecialEffects specialEffect, float interval)
        {
            _activeEffect = true;
            SetEffect(_effect, _interval);
            Timing.RunCoroutine(InstantiateEffect(), Segment.LateUpdate, JCoroutineTags.COROUTINE_MouseEffect);
        }
        
        private IEnumerator<float> InstantiateEffect()
        {
            while (true)
            {
                _effect.TriggerEffectOnPosition(_pointerTracker.CursorTracker.transform.position, Quaternion.identity);
                yield return Timing.WaitForSeconds(_interval);
            }
        }


        /// <summary>
        /// stops the effect
        /// </summary>
        public void DisableMouseEffect()
        {
            if (!_activeEffect) return;
            Timing.KillCoroutines(JCoroutineTags.COROUTINE_MouseEffect);
            _activeEffect = false;
        }
    }
}
