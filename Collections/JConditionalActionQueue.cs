using System;
using System.Collections.Generic;
using System.Linq;
using MEC;

namespace JReact.Collections
{
    /// <summary>
    /// Represents a conditional action queue that executes actions based on a specified condition.
    /// </summary>
    public class JConditionalActionQueue
    {
        private const int DefaultCapacity = 4;

        private readonly Func<bool> _condition;
        private readonly List<Action> _actions;

        private CoroutineHandle _handle;
        public bool IsRunning => _handle.IsRunning;

        /// <summary>
        /// Represents a conditional action queue that executes actions based on a specified condition.
        /// </summary>
        public JConditionalActionQueue(Func<bool> condition, int capacity = DefaultCapacity)
        {
            _condition = condition;
            _actions   = new List<Action>(capacity);
        }

        public void RegisterAction(Action action)
        {
            if (_condition())
            {
                action?.Invoke();
                return;
            }

            _actions.Add(action);
            if (!IsRunning) { _handle = Timing.RunCoroutine(WaitThenRun(), Segment.Update); }
        }

        public void UnRegisterAction(Action action) { _actions?.RemoveSafe(action); }

        private IEnumerator<float> WaitThenRun()
        {
            yield return Timing.WaitUntilTrue(_condition);

            ExecuteQueue();

            ResetThis();
        }

        private void ExecuteQueue()
        {
            for (int i = _actions.Count - 1; i >= 0; i--) { _actions[i].Invoke(); }
        }

        private void ResetThis()
        {
            _actions.Clear();
            _handle = default;
        }

        public override string ToString()
        {
            string actionNames = string.Join(", ", _actions.Select(a => a.Method.Name));

            return $"Conditional Action Queue Status: \n"        +
                   $"- Number of Actions: {_actions.Count}\n"    +
                   $"- Actions: {actionNames}\n"                 +
                   $"- Is Running: {IsRunning}\n"                +
                   $"- Condition State: {_condition.Invoke()}\n" +
                   $"- Coroutine Handle: {_handle.ToString()}";
        }
    }
}
