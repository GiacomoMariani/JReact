using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Collections
{
    /// <summary>
    /// handles tasks in a given order
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Collection/Task Queue")]
    public class J_TaskQueue : J_Service, IEnumerable<iTask>
    {
        // --------------- FIELDS AND PROPERTIS --------------- //
        public const string NoTask = "QueueFree_NoTask";
        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private iTask _currentTask;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Queue<iTask> _taskQueue { get; } = new Queue<iTask>(50);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int TotalTasks => _taskQueue.Count;

        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public string CurrentTaskName => _currentTask != default
                                                                                                                  ? _currentTask.Name
                                                                                                                  : NoTask;

        // --------------- COMMANDS --------------- //
        public static J_TaskQueue CreateInstance() => CreateInstance<J_TaskQueue>();

        public void ProcessTask(iTask taskToProcess)
        {
            JLog.Log($"{name} task added. Current tasks: {TotalTasks}", JLogTags.Task, this);
            //send this task if no task is running, otherwise enqueue it
            if (!IsActive) StartQueueWith(taskToProcess);
            else EnqueueTask(taskToProcess);
        }

        // --------------- START AND END QUEUE --------------- //
        //the first task activate the queue        
        private void StartQueueWith(iTask taskToProcess)
        {
            Activate();
            JLog.Log($"{name} Task Queue START with {taskToProcess.Name}", JLogTags.Task, this);
            RunTask(taskToProcess);
        }

        private void StopQueue(iTask currentTask)
        {
            JLog.Log($"{name} Task Queue STOP with {_currentTask.Name}", JLogTags.Task, this);
            End();
        }

        // --------------- TASKS PROCESSING --------------- // 
        private void RunTask(iTask taskToProcess)
        {
            //send the task and wait for the next
            JLog.Log($"{name} running task {taskToProcess.Name}", JLogTags.Task, this);
            _currentTask = taskToProcess;
            taskToProcess.Activate();
            //if the task is not active it is already completed, otherwise wait for completion
            if (!taskToProcess.IsActive) CheckNext();
            else taskToProcess.SubscribeToEnd(CheckNext);
        }

        private void EnqueueTask(iTask taskToProcess)
        {
            JLog.Log($"{name} enqueued {taskToProcess.Name}", JLogTags.Task, this);
            _taskQueue.Enqueue(taskToProcess);
        }

        //this is called when the processed task is completed
        private void CheckNext()
        {
            StopTrackingTask();
            if (TotalTasks > 0) RunTask(_taskQueue.Dequeue());
        }

        // --------------- DISABLE AND RESET --------------- //
        public override void ResetThis()
        {
            base.ResetThis();
            if (_currentTask != null) StopTrackingTask();
            _taskQueue.Clear();
        }

        private void StopTrackingTask()
        {
            _currentTask.UnSubscribeToEnd(CheckNext);
            if (_taskQueue.Count == 0) StopQueue(_currentTask);
            _currentTask = null;
        }

        // --------------- QUEUE IMPLEMENTATION --------------- //
        public IEnumerator<iTask> GetEnumerator() => _taskQueue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => _taskQueue.Count;
        public bool Contains(iTask state) => _taskQueue.Contains(state);
    }
}
