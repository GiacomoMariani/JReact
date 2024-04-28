using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl.LevelSystem
{
    /// <summary>
    /// the system to control the player level progression
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Level System/Full Progression")]
    public sealed class J_LevelProgression : J_StateControl<J_LevelState>
    {
        //-----------------> SETUP CAPACITOR AND CAPACITOR VIEW
        private const string ExperienceSuffix = "{0}_Experience";

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_ExperienceReceiver _experience;
        public J_ExperienceReceiver Experience { get => _experience; private set => _experience = value; }

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _currentLevelIndex;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int PreviousExperience { get; private set; }

        // --------------- BOOK KEEPING --------------- //
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int TotalExperience
            => PreviousExperience + Experience.Current;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int CurrentLevel => _currentLevelIndex + 1;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int MaxLevel => _validStates.Length;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public J_LevelState CurrentLevelInfo => CurrentState;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool MaxLevelReached => CurrentLevel == MaxLevel;

        // --------------- INSTANTIATION --------------- //
        /// <summary>
        /// instantiate a level progression
        /// </summary>
        /// <param name="states">the valid states</param>
        /// <param name="init">if we want to initialize it, defaults at true</param>
        /// <param name="experience">an optional experience receiver</param>
        /// <returns>returns a state control system</returns>
        public static J_LevelProgression Create(J_LevelState[] states, bool init = true)
        {
            // --------------- SANITY CHECKS --------------- //
            Assert.IsNotNull(states, "Creation requires a states");
            for (int i = 0; i < states.Length; i++) Assert.IsNotNull(states[i], $"Null state at index {i}");
            var stateControl = CreateInstance<J_LevelProgression>();

            // --------------- SETUP --------------- //
            stateControl._validStates = states;
            stateControl._firstState  = states[0];

            // --------------- EXPERIENCE --------------- //
            J_ExperienceReceiver experience = J_ExperienceReceiver.Create(stateControl);
            experience.name         = string.Format(ExperienceSuffix, stateControl.name);
            stateControl.Experience = experience;

            if (init) stateControl.ActivateThis();
            return stateControl;
        }

        // --------------- COMMANDS --------------- //
        protected override void ActivateThis()
        {
            base.ActivateThis();
            Assert.IsNotNull(Experience, $"{name} requires Experience");
            SetupLevels();
            InitializeExperience();
        }

        private void InitializeExperience()
        {
            _currentLevelIndex = 0;
            PreviousExperience = 0;
            if (Experience.IsActive) Experience.ResetThis();
        }

        /// <summary>
        /// adds a number of levels to the current progression
        /// </summary>
        /// <param name="levelToAdd">the levels to be added</param>
        public void GainLevel(int levelToAdd = 1)
        {
            Assert.IsTrue(levelToAdd > 0, $"{name} level up requires positive numbers. Received {levelToAdd}");
            for (int i = 0; i < levelToAdd; i++)
            {
                if (MaxLevelReached)
                {
                    JLog.Warning($"{name} has reached max level of {MaxLevel}. Stop raising.", JLogTags.LevelSystem, this);
                    return;
                }

                GainOneLevel();
            }
        }

        /// <summary>
        /// sets a level directly
        /// </summary>
        /// <param name="level">the level we want to set</param>
        /// <param name="callAllEvents">mostly used for loading, we might not require the events</param>
        public void ForceSetLevel(int level, bool callAllEvents = true)
        {
            // --------------- SAFE AND SETUP --------------- //
            //we initialize it forcefully if not initialized, if so we start from level 1
            Assert.IsTrue(level > 0, $"{name} level to set requires to be positive. Received {level}");
            if (AboveMaxLevel(level)) level = MaxLevel;

            // --------------- SETUP --------------- //
            if (IsActive) InitializeExperience();
            Activate();

            // --------------- SET LEVEL --------------- //
            if (callAllEvents) GainLevel(level - 1);
            else _currentLevelIndex = level - 1;

            JLog.Log($"{name} forced to level {CurrentLevel}", JLogTags.LevelSystem, this);
        }

        // --------------- GETTERS --------------- //
        public J_LevelState GetLevelData(int level)
        {
            Assert.IsTrue(level <= MaxLevel, $"{name} has no such level: {level}. Max Level: {MaxLevel}");
            return _validStates[level - 1];
        }

        public int GetExperienceToReach(int level) => GetLevelData(level).PreviousExperience;

        // --------------- HELPERS AND IMPLEMENTORS --------------- //
        //used to set all the relevant info into the levels
        private void SetupLevels()
        {
            int previousLevelExperience = 0;
            for (int i = 0; i < MaxLevel; i++)
            {
                J_LevelState level = GetLevelData(i + 1);
                level.SetPreviousExperience(previousLevelExperience);
                previousLevelExperience += level.ExperienceNeeded;
                level.SetMaxLevel(i + 1 == MaxLevel);
            }

            GetLevelData(MaxLevel).SetMaxLevel(true);
        }

        //sends all the events related to a single level
        private void GainOneLevel()
        {
            // --------------- STORE PREVIOUS --------------- // 
            PreviousExperience += CurrentLevelInfo.ExperienceNeeded;

            // --------------- SET NEXT LEVEL --------------- //
            //updating the index we also update CurrentLevelInfo
            _currentLevelIndex++;
            //this command will also raise the enter and exit events of the other levels
            SetNewState(GetLevelData(CurrentLevel));

            JLog.Log($"{name} reached level {CurrentLevel}", JLogTags.LevelSystem, this);
        }

        private bool AboveMaxLevel(int level)
        {
            if (level > MaxLevel)
            {
                JLog.Warning($"{name} Current level {CurrentLevel} is above {level}. Setting Max", JLogTags.LevelSystem, this);
                return true;
            }

            return false;
        }
    }
}
