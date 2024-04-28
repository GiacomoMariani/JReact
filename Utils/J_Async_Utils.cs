using System;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace JReact
{
    public static class J_Async_Utils
    {
        /// <summary>
        /// wait until a function returning a bool is true, within a time limit
        /// </summary>
        /// <param name="func">the function logic to check</param>
        /// <param name="operationName">the operation name, for the report</param>
        /// <param name="msInterval">the interval to pass between each check</param>
        /// <param name="maxMs">the max amount of milliseconds before getting a timeout</param>
        /// <returns>returns true if the function passed, or false for the timeout</returns>
        public static async UniTask<bool> WaitUntilReady(this Func<bool> func,            string operationName, int maxMs = 5_000,
                                                         int             msInterval = 50, Object caller = null)
        {
            var masPassed = 0;
            while (!func.Invoke())
            {
                await UniTask.Delay(msInterval);
                masPassed += msInterval;
                if (masPassed <= maxMs) { continue; }

                JLog.Error($"{operationName} - Timeout after {masPassed} milliseconds", JLogTags.Task, caller);
                return false;
            }

            return true;
        }
    }
}
