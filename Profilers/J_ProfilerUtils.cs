using System.Collections.Generic;
using Unity.Profiling.LowLevel.Unsafe;

namespace JReact.Profilers
{
    public static class J_ProfilerUtils
    {
        /// <summary>
        /// gets a list of all valid profilers with (Category) and description
        /// </summary>
        /// <param name="log">true to log the result also on the console</param>
        /// <returns>returns the string of all profilers</returns>
        public static string GetAvailableInputs(bool log)
        {
            var list = new List<ProfilerRecorderHandle>();
            ProfilerRecorderHandle.GetAvailable(list);
            string result = "(Category) Description";
            for (int i = 0; i < list.Count; i++)
            {
                ProfilerRecorderDescription descr = ProfilerRecorderHandle.GetDescription(list[i]);
                result += $"({descr.Category}) {descr.Name}\n";
            }

            if (log) { JLog.Log(result, JLogTags.Input); }

            return result;
        }
    }
}
