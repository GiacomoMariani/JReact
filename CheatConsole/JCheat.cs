using System;

namespace JReact.CheatConsole
{
    /// <summary>
    /// the most generic base command with its data
    /// </summary>
    [Serializable]
    public class JCheat
    {
        private string _commandId;
        private string _commandDescription;
        private string _commandFormat;
        public string CommandId => _commandId;
        public string CommandDescription => _commandDescription;
        public string CommandFormat => _commandFormat;

        public JCheat(string commandId, string commandDescription, string commandFormat)
        {
            _commandId          = commandId;
            _commandDescription = commandDescription;
            _commandFormat      = commandFormat;
        }

        public virtual void Invoke(J_Mono_CheatConsole console)
        {
            JLog.Log($"{console.NameId}=> sends command {_commandId}", JLogTags.Cheats, console.gameObject);
        }

        public override string ToString() => $"{CommandId} - {CommandDescription} - {CommandFormat}";
    }

    /// <summary>
    /// a cheat command that sends an action
    /// </summary>
    public sealed class JCheatAction : JCheat
    {
        private Action _command;

        public JCheatAction(string commandId, string commandDescription, string commandFormat,
                            Action command) : base(commandId, commandDescription, commandFormat) => this._command = command;

        public override void Invoke(J_Mono_CheatConsole console)
        {
            base.Invoke(console);
            _command.Invoke();
        }
    }

    /// <summary>
    /// a cheat command that sends an action with a single generic input
    /// </summary>
    public abstract class JCheatAction<T> : JCheat
    {
        private Action<T> _command;

        public JCheatAction(string    commandId, string commandDescription, string commandFormat,
                            Action<T> command) : base(commandId, commandDescription, commandFormat) => this._command = command;

        public override void Invoke(J_Mono_CheatConsole console)
        {
            base.Invoke(console);
            if (TryConvert(console.Parameters[0], out T parameter)) { _command.Invoke(parameter); }
            else { HandleFailCommand(console); }
        }

        protected virtual  void HandleFailCommand(J_Mono_CheatConsole cheatConsole) {}
        protected abstract bool TryConvert(string                     consoleParameter, out T parameter);
    }

    public class JCheatActionFloat : JCheatAction<float>
    {
        public JCheatActionFloat(string commandId, string commandDescription, string commandFormat, Action<float> command) :
            base(commandId, commandDescription, commandFormat, command)
        {
        }

        protected override bool TryConvert(string consoleParameter, out float parameter)
            => float.TryParse(consoleParameter, out parameter);
    }

    /// <summary>
    /// a cheat command that sends an action with two generic input
    /// </summary>
    public abstract class JCheatAction<T, K> : JCheat
    {
        private Action<T, K> _command;

        public JCheatAction(string       commandId, string commandDescription, string commandFormat,
                            Action<T, K> command) : base(commandId, commandDescription, commandFormat) => this._command = command;

        public override void Invoke(J_Mono_CheatConsole console)
        {
            base.Invoke(console);
            if (console.Parameters        == null ||
                console.Parameters.Length < 2) { HandleFailCommand(console); }
            else if (TryConvert(console.Parameters[1], out T parameterA) &&
                     TryConvert(console.Parameters[2], out K parameterB)) { _command.Invoke(parameterA, parameterB); }
            else { HandleFailCommand(console); }
        }

        protected virtual void HandleFailCommand(J_Mono_CheatConsole console)
        {
            JLog.Warning($"{console.NameId}=> fails {CommandId}.\nParams: {console.Parameters.PrintAll()}", JLogTags.Cheats,
                         console.gameObject);
        }

        protected abstract bool TryConvert(string consoleParameter, out T parameter);
        protected abstract bool TryConvert(string consoleParameter, out K parameter);
    }
}
