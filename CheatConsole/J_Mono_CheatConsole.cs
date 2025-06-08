using System.Collections.Generic;
using JReact.Singleton;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.CheatConsole
{
    /// <summary>
    /// a simple cheat console to be displayed on the screen
    /// to add a cheat command:
    /// 1. create a cheat action with title, description and reference to a method like new JCheatAction("quit_game", "Quits the game", "quit_game", QuitGame);
    /// we can also add JCheatAction with different arguments such as JCheatAction<int>, in this case the method should be Action<int>
    /// 2. add the command with console.AddCommand(QuitGameCommand);
    /// </summary>
    public class J_Mono_CheatConsole : J_MonoSingleton<J_Mono_CheatConsole>
    {
        private const string CommandTitle = "AVAILABLE COMMANDS";

        // --------------- FIELD AND PROPERTIES --------------- //
        [BoxGroup("MAIN", true, true, -1), ReadOnly, ShowInInspector] public static bool CheatConsoleEnabledAndShown
            => CheatConsoleEnabled && InstanceUnsafe.IsConsoleShown;
        [BoxGroup("MAIN",  true, true, -1), ReadOnly, ShowInInspector] public static bool CheatConsoleEnabled = false;
        [BoxGroup("Setup", true, true, 0), SerializeField] private char _splitChar = ' ';
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _autoInit = false;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _desireGenericCommands = true;
        //auto commands are loaded using reflection
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _desireAutoCommands = true;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0)] private int _commandsToStore = 5;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0)] private int _stringToStore = 10;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private GameObject _consoleView;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private GameObject _helpTextView;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private TMP_InputField _input;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private Dictionary<string, JCheat> _validCommands = new Dictionary<string, JCheat>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public string[] Parameters;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Queue<string> _stringsReceived = new Queue<string>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Queue<JCheat> _commandsReceived = new Queue<JCheat>();
        [FoldoutGroup("State", false, 5), ShowInInspector] public bool IsConsoleShown
        {
            get => _consoleView.activeSelf;
            private set
            {
                _consoleView.SetActive(value && CheatConsoleEnabled);
                if (IsConsoleShown) { _input.ActivateInputField(); }
            }
        }
        [FoldoutGroup("State", false, 5), ShowInInspector] public string NameId => $"{gameObject.name}-{GetHashCode()}";

        // --------------- INITIALIZATION  AND SETUP --------------- //
        [Button]
        public static void EnableConsole()
        {
            GetInstanceSafe();
            InstanceUnsafe.ActivateConsole();
            CheatConsoleEnabled = true;
        }

        [Button]
        public static void DisableConsole()
        {
            if (!CheatConsoleEnabled) { return; }

            InstanceUnsafe.DeActivate();
            CheatConsoleEnabled = false;
        }

        private void ActivateConsole()
        {
            Assert.IsFalse(CheatConsoleEnabled, $"{NameId} already enabled. Current Instance: ({InstanceUnsafe.NameId})");

            if (_desireGenericCommands) { JGenericCheats.AddApplicationCommands(this); }

            if (_desireAutoCommands) { JAutoCheat.LoadReflectedCheats(this); }

            if (_helpTextView != null) { AddCommand(JCheatHelp.GetHelpCommand(_helpTextView)); }

            Assert.IsNotNull(_consoleView, $"{NameId} requires a {nameof(_consoleView)}");
            _input = _consoleView.GetComponentInChildren<TMP_InputField>(true);
            Assert.IsNotNull(_input, $"{gameObject.name} requires a {nameof(_input)}");
            _input.onSubmit.AddListener(TrySendCommand);

            if (_commandsToStore > 0)
            {
                _commandsReceived = new Queue<JCheat>(_commandsToStore);
                _stringsReceived  = new Queue<string>(_stringToStore);
            }

            JLog.Log($"{NameId} Initialized with {_validCommands.Count} commands", JLogTags.Cheats, gameObject);
        }

        private void DeActivate()
        {
            _input.onSubmit.RemoveListener(TrySendCommand);
            IsConsoleShown = false;
            JLog.Log($"{NameId} Disabled", "-CHEATS-", gameObject);
        }

        /// <summary>
        /// adds a command to the console
        /// </summary>
        /// <param name="command">the command we want to add</param>
        public void AddCommand(JCheat command)
        {
            Assert.IsFalse(_validCommands.ContainsKey(command.CommandId), $"{name} cheat already registered: {command.CommandId}");
            _validCommands.Add(command.CommandId, command);
        }

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// show and hides the console. Consider that the console won't be shown if it is not statically enabled with CheatConsoleEnabled
        /// </summary>
        /// <param name="consoleEnabled">true if we want to show the console</param>
        public void ShowConsole(bool consoleEnabled) => IsConsoleShown = consoleEnabled;

        /// <summary>
        /// toggles the console opens and close. CheatConsoleEnabled must be enabled to make it work
        /// </summary>
        public void ToggleConsole() => ShowConsole(!IsConsoleShown);

        private void TrySendCommand(string commandArguments)
        {
            if (!CheatConsoleEnabled) { return; }

            if (!commandArguments.HasAtLeastOneAlphanumeric()) { return; }

            JLog.Log($"{NameId} => received parameters: {commandArguments}", JLogTags.Cheats, gameObject);

            _stringsReceived.Enqueue(commandArguments);
            if (_stringsReceived.Count > _stringToStore) { _stringsReceived.Dequeue(); }

            Parameters = commandArguments.Split(_splitChar);

            var commandReceived = Parameters[0];
            if (!_validCommands.ContainsKey(commandReceived))
            {
                JLog.Warning($"{name} - No such command: {commandReceived}", JLogTags.Cheats, this);
                return;
            }

            var command = _validCommands[commandReceived];

            command.Invoke(this);
            _commandsReceived.Enqueue(command);
            if (_commandsReceived.Count > _commandsToStore) { _commandsReceived.Dequeue(); }

            _input.SetTextWithoutNotify(JConstants.EmptyString);
        }

        // --------------- QUERIES --------------- //
        /// <summary>
        /// gets the list of valid commands usable in the console
        /// </summary>
        /// <returns></returns>
        internal string GetCommandsList() => _validCommands.PrintAll(CommandTitle, JConstants.LineBreak);

        // --------------- AUTO INIT --------------- //
        protected internal override void InitThis()
        {
            base.InitThis();
            if (_autoInit) { EnableConsole(); }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DisableConsole();
        }
    }
}
