using UnityEditor;
using UnityEngine;

namespace JReact.CheatConsole
{
    public static class JGenericCheats
    {
        public static JCheatAction Invalid = default;
        private static JCheatAction QuitGameCommand => new JCheatAction("quit_game", "Quits the game", "quit_game", QuitGame);

        private static JCheatActionFloat SetTimeCommand
            => new JCheatActionFloat("set_time", "Set the time scales custom", "set_time <float time>", SetTime);

        private static void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        private static void SetTime(float timeScale) { Time.timeScale = 1f; }

        // --------------- HELPER METHOD --------------- //
        internal static void AddApplicationCommands(J_Mono_CheatConsole console)
        {
            console.AddCommand(QuitGameCommand);
            console.AddCommand(SetTimeCommand);
        }
    }
}
