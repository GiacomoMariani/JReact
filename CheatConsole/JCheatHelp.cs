using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.CheatConsole
{
    public class JCheatHelp : JCheat
    {
        internal static JCheat GetHelpCommand(GameObject textView) => new JCheatHelp("help", "Toggles the help commands on and off", "help", textView);

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private TextMeshProUGUI _text;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private GameObject _helpTextView;

        private JCheatHelp(string commandId, string commandDescription, string commandFormat, GameObject view) :
            base(commandId, commandDescription, commandFormat)
        {
            _helpTextView = view;
            _text         = view.GetComponentInChildren<TextMeshProUGUI>(true);
            Assert.IsNotNull(_text, $"{view.name} requires a {nameof(_text)}");
        }

        public override void Invoke(J_Mono_CheatConsole console)
        {
            base.Invoke(console);
            _helpTextView.SetActive(!_helpTextView.activeSelf);
            if (_helpTextView.activeSelf) { _text.text = console.GetCommandsList(); }
        }
    }
}
