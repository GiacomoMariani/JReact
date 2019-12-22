using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Conditions.Tasks
{
    /// <summary>
    /// contains multiple tasks related to a single goal, such as a mission
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Task/Task Chunk")]
    public class J_TaskChunk : J_Service, jObservable<J_TaskChunk>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private Action<J_TaskChunk> OnStateChange;
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_CompletableTask[] _tasks;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_Collection_DormantTasks _dormants;

        // --------------- STATE --------------- //
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] public string Title => name;
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly]
        private List<J_CompletableTask> _activeTasks = new List<J_CompletableTask>();
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] private ChunkState _state =
            ChunkState.NotStarted;
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] public ChunkState State
        {
            get => _state;
            private set
            {
                _state = value;
                OnStateChange?.Invoke(this);
                JLog.Log($"{name} enter state: {State}", JLogTags.Task, this);
            }
        }

        // --------------- GENERATORS AND CHECKS --------------- //
        public static J_TaskChunk CreateChunk(J_CompletableTask[] tasks, J_Collection_DormantTasks dormants, string nameSet = "Chunk")
        {
            var chunk = CreateInstance<J_TaskChunk>();
            chunk._tasks    = tasks;
            chunk._dormants = dormants;
            chunk.SanityChecks();
            chunk.name = nameSet;
            return chunk;
        }

        private void SanityChecks()
        {
            Assert.IsNotNull(_tasks, $"({name}) requires a _tutorialSteps");
            Assert.IsTrue(_tasks.Length > 0, $"({name}) needs at least one item for _tutorialSteps");
            Assert.IsNotNull(_dormants, $"{name} requires a _dormants");
        }

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// launch this task chunk
        /// </summary>
        protected override void ActivateThis()
        {
            base.ActivateThis();
            // --------------- START --------------- //
            JLog.Log($"{name} CHUNK START--------------", JLogTags.Task, this);
            SanityChecks();
            State = ChunkState.Active;

            // --------------- INITIALIZE --------------- //
            //first setup all the tasks
            for (int i = 0; i < _tasks.Length; i++)
            {
                J_CompletableTask nextTask = _tasks[i];
                _activeTasks.Add(nextTask);
                nextTask.SubscribeToTaskChange(StepCompleted);
                if (_dormants != null) _dormants.TrackTask(nextTask);
            }

            //then run them all
            for (int i = 0; i < _tasks.Length; i++)
                if (!_tasks[i].IsActive)
                    _tasks[i].Activate();
        }

        //remove the task when completed
        private void StepCompleted(J_CompletableTask completedTask)
        {
            if (completedTask.State != TaskState.Complete) return;
            // --------------- REMOVAL --------------- //
            completedTask.UnSubscribeToTaskChange(StepCompleted);
            completedTask.End();
            _activeTasks.Remove(completedTask);

            // --------------- CHUNK COMPLETE --------------- //
            //the chunk completes when there are no more active tasks
            CompleteChunk();
        }

        //sends all the logic for the completion
        private void CompleteChunk()
        {
            if (_activeTasks.Count > 0) return;
            JLog.Log($"{name} CHUNK COMPLETE--------------", JLogTags.Task, this);
            State = ChunkState.Completed;
            End();
        }

        /// <summary>
        /// used to complete all the missing tasks
        /// </summary>
        public void ForceCompleteChunk()
        {
            for (int i = 0; i < _tasks.Length; i++)
            {
                J_CompletableTask task = _tasks[i];
                if(task.RequireCompletion) task.ForceCompleteTask();
            }
        }

        /// <summary>
        /// sets a specific state
        /// </summary>
        public void SetState(ChunkState stateLoaded) { State = stateLoaded; }

        // --------------- SUBSCRIBERS --------------- //
        public void Subscribe(Action<J_TaskChunk>                action) { OnStateChange += action; }
        public void UnSubscribe(Action<J_TaskChunk>              action) { OnStateChange -= action; }
        public void SubscribeToStateChange(Action<J_TaskChunk>   action) { Subscribe(action); }
        public void UnSubscribeToStateChange(Action<J_TaskChunk> action) { UnSubscribe(action); }

        // --------------- DISABLE AND RESET --------------- //
        protected virtual void OnDisable() { ResetThis(); }

        public override void ResetThis()
        {
            base.ResetThis();
            if (State == ChunkState.Completed) State = ChunkState.NotStarted;
            if (State != ChunkState.Active) return;
            for (int i = 0; i < _activeTasks.Count; i++)
            {
                _activeTasks[i].ResetThis();
                _activeTasks[i].UnSubscribeToTaskChange(StepCompleted);    
            }

            _activeTasks.Clear();
            State = ChunkState.NotStarted;
        }
    }

    public enum ChunkState { NotStarted = 0, Active = 100, Completed = 200 }
}
