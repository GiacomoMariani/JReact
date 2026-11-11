using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JReact.JuiceMenuComposer;
using JReact.Singleton;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;

namespace JReact.SceneControl
{
    /// <summary>
    /// Runs one transition at a time. AddTask queues its steps and Display runs them while JUI_Screen_Transition covers the
    /// screen: its items play the cover as it shows and the uncover as it hides. Ordered steps run one after another,
    /// parallel steps run alongside them, and the cover hides once all finish. Listeners follow IsBusy through
    /// SubscribeOnBusyChanged.
    /// </summary>
    public sealed class J_St_SceneTransition : J_MonoSingleton<J_St_SceneTransition>
    {
        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private readonly List<Func<UniTask>> _steps = new List<Func<UniTask>>(4);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private readonly List<Func<UniTask>> _parallelSteps = new List<Func<UniTask>>(4);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private readonly List<UniTask> _running = new List<UniTask>(4);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsBusy { get; private set; }

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private readonly List<Action<bool>> _busyListeners = new List<Action<bool>>(2);

        // --------------- COMMANDS --------------- //
        public void AddTask(Func<UniTask> task, bool parallel = false)
        {
            Assert.IsNotNull(task, NullTask);
            Assert.IsFalse(IsBusy, AddWhileBusy);
            if (parallel) { _parallelSteps.Add(task); }
            else { _steps.Add(task); }
        }

        public async UniTask Display()
        {
            Assert.IsFalse(IsBusy, DisplayWhileBusy);
            Assert.IsTrue(JUI_St_Composer.TryGetRegisteredScreen<JUI_Screen_Transition>(out JUI_Screen _), MissingCover);
            SetBusy(true);
            await JUI_St_Composer.Show<JUI_Screen_Transition>();
            JLog.Log(CoverRaised, JLogTags.UiView, this);
            for (int i = 0; i < _parallelSteps.Count; i++) { _running.Add(_parallelSteps[i].Invoke()); }
            for (int i = 0; i < _steps.Count; i++) { await _steps[i].Invoke(); }
            // The parallel steps are already running, so awaiting them in turn waits for all of them.
            for (int i = 0; i < _running.Count; i++) { await _running[i]; }

            await JUI_St_Composer.Hide<JUI_Screen_Transition>();
            JLog.Log(CoverHidden, JLogTags.UiView, this);
            _steps.Clear();
            _parallelSteps.Clear();
            _running.Clear();
            SetBusy(false);
        }

        private void SetBusy(bool busy)
        {
            IsBusy = busy;
            for (int i = _busyListeners.Count - 1; i >= 0; i--) { _busyListeners[i].Invoke(busy); }
        }

        // --------------- SUBSCRIBERS --------------- //
        public void SubscribeOnBusyChanged(Action<bool> action)
        {
            Assert.IsFalse(_busyListeners.Contains(action), DuplicateListener);
            _busyListeners.Add(action);
        }

        public void UnSubscribeOnBusyChanged(Action<bool> action) => _busyListeners.Remove(action);

        private const string NullTask = nameof(J_St_SceneTransition) + ": queued transition step is null.";
        private const string MissingCover = nameof(J_St_SceneTransition) + ": " + nameof(JUI_Screen_Transition) + " is not registered on the JUI composer.";
        private const string CoverRaised = nameof(J_St_SceneTransition) + ": transition cover raised.";
        private const string CoverHidden = nameof(J_St_SceneTransition) + ": transition cover hidden.";
        private const string AddWhileBusy = nameof(J_St_SceneTransition) + ": a step was added while a transition runs.";
        private const string DisplayWhileBusy = nameof(J_St_SceneTransition) + ": Display was called while a transition runs.";
        private const string DuplicateListener = nameof(J_St_SceneTransition) + ": this busy listener is already subscribed.";
    }
}
