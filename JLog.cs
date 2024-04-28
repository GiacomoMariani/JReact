using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace JReact
{
    /// <summary>
    /// display debug messages in the console or on the specific platform
    /// </summary>
    public static partial class JLog
    {
        // --------------- FORMAT --------------- //
        private static readonly StringBuilder _stringBuilder = new StringBuilder(2048);
        private static StringBuilder SBuilder
        {
            get
            {
                _stringBuilder.Clear();
                return _stringBuilder;
            }
        }

        private static string Format(string message, string tag) => SBuilder
                                                                   .AppendFormat("{0:HH:mm:ss}-[{1}] {2}", DateTime.Now, tag, message)
                                                                   .ToString();

        // --------------- MAIN LOGGERS --------------- //
        /// <summary>
        /// displays a message in the console
        /// </summary>
        /// <param name="tag">a tag useful for console pro</param>
        public static void Log(string message, string tag = "", Object context = null)
        {
#if UNITY_EDITOR
            Debug.Log(Format(message, tag), context);
#endif
#if !UNITY_EDITOR
			Debug.Log(Format(message, tag));
#endif
        }

        /// <summary>
        /// Displays a message in the console with colored text.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="tag">A tag useful for console pro.</param>
        /// <param name="context">The context object.</param>
        public static void LogColor(string message, Color color, string tag = "", Object context = null)
        {
#if UNITY_EDITOR
            Debug.Log(Format(message, tag).AppendWithColor(color), context);
#endif
#if !UNITY_EDITOR
			Debug.Log(Format(message, tag));
#endif
        }

        [Conditional("TRACE")]
        public static void Trace(string message, string tag = "", Object context = null)
        {
#if UNITY_EDITOR
            Debug.Log(Format(message, tag), context);
#endif
#if !UNITY_EDITOR
			Debug.Log(Format(message, tag));
#endif
        }

        public static void Warning(string message, string tag = "", Object context = null)
        {
#if UNITY_EDITOR
            Debug.LogWarning(Format(message, tag), context);
#endif
#if !UNITY_EDITOR
			Debug.LogWarning(Format(message, tag));
#endif
        }

        public static void Exception(Exception exception, Object context = null)
        {
#if UNITY_EDITOR
            Debug.LogException(exception, context);
#endif
#if !UNITY_EDITOR
			Debug.LogException(exception);
#endif
        }

        public static void Error(string message, string tag = "", Object context = null)
        {
#if UNITY_EDITOR
            Debug.LogError(Format(message, tag), context);
#endif
#if !UNITY_EDITOR
			Debug.LogError(Format(message, tag));
#endif
        }

        public static void Break(string message, string tag = "", Object context = null)
        {
#if UNITY_EDITOR
            Debug.LogError(Format(message, tag), context);
            Debug.Break();
#endif
#if !UNITY_EDITOR
			Debug.LogError(Format(message, tag));
            Debug.Break();
#endif
        }

        [Conditional("DEBUG")]
        public static void RememberToDo(string message, object workOnThis)
        {
#if UNITY_EDITOR
            Debug.Log($"#TO DO#\n{workOnThis.GetType()} needs to be completed.\n Task: {message}");
#endif
        }

        [Conditional("DEBUG")]
        public static void QuickLog(string message, Object context = null) { Log(message, JLogTags.QuickLog, context); }
    }
}
