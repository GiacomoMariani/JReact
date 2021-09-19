using UnityEditor;
using UnityEngine;

namespace JReact.CheatConsole
{
    public static class JGenericCheats
    {
        public static JCheatAction Invalid = default;
        // --------------- QUIT --------------- //
        private static JCheatAction QuitGameCommand => new JCheatAction("quit_game", "Quits the game", "quit_game", QuitGame);

        private static void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        // --------------- PERMANENT CLOSE --------------- //
        private static JCheatAction CloseCommand
            => new JCheatAction("close_permanent", "Permanently closes the game console", "close_permanent", CloseConsolePermanent);
        private static void CloseConsolePermanent() => J_Mono_CheatConsole.DisableConsole();

        // --------------- TOGGLE CONSOLE --------------- //
        private static JCheatAction ToggleConsoleCommand
            => new JCheatAction("toggle_console", "Toggles the console off", "toggle_console", ToggleConsole);
        private static void ToggleConsole() => J_Mono_CheatConsole.Instance.ToggleConsole();

        //these commands are more powerful and are allowed only out of the game
#if UNITY_EDITOR
        private static JCheatActionFloat SetTimeCommand
            => new JCheatActionFloat("set_time", "Set the time scales custom", "set_time <float time>", SetTime);

        private static void SetTime(float timeScale) { Time.timeScale = 1f; }
#endif

        // --------------- HELPER METHOD --------------- //
        internal static void AddApplicationCommands(J_Mono_CheatConsole console)
        {
            console.AddCommand(QuitGameCommand);
            console.AddCommand(CloseCommand);
            console.AddCommand(ToggleConsoleCommand);
#if UNITY_EDITOR
            console.AddCommand(SetTimeCommand);
#endif
        }
    }
}
