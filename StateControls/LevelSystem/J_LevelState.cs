﻿using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControls.LevelSystem
{
    /// <summary>
    /// a level is like a state, an entity may reach or move out of a given level
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Level System/Level")]
    public class J_LevelState : J_State, iResettable
    {
        public virtual string LevelName => name;

        [BoxGroup("Setup", true, true, 0), SerializeField] private int _experienceNeeded;
        public int ExperienceNeeded { get => _experienceNeeded; private set => _experienceNeeded = value; }

        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool IsMaxLevel { get; private set; } = false;

        private void SanityCheck() { Assert.IsTrue(ExperienceNeeded > 0, "One level experience is not positive. Level: " + name); }

        #region SETUP COMMANDS
        //used to set this as max level
        internal void SetMaxLevel() { IsMaxLevel = true; }

        //helper to set the max experience on this
        public void SetExperienceNeeded(int experience)
        {
            ExperienceNeeded = experience;
            SanityCheck();
        }
        #endregion
    }
}
