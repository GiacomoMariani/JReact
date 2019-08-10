﻿using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Pool.SpecialEffect
{
    /// <summary>
    /// animation effect implemented as pool item
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public sealed class J_PoolItem_Animation : J_PoolItem_SpecialEffect
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField] private float _animationLength;
        [BoxGroup("Setup", true, true), SerializeField] private string _animatorTrigger;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Animator _thisAnimator;
        private Animator _ThisAnimator
        {
            get
            {
                if (_thisAnimator == null) _thisAnimator = GetComponent<Animator>();
                return _thisAnimator;
            }
        }

        // --------------- IMPLEMENTATION --------------- //
        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_ThisAnimator, $"{gameObject.name} requires an animator ");
        }

        protected override void TriggerThisEffect()
        {
            _ThisAnimator.SetTrigger(_animatorTrigger);
            RemoveAfterSeconds(_animationLength);
        }
    }
}
