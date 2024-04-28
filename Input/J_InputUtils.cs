using System.Collections.Generic;
using System.Text;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR;

namespace JReact.J_Input
{
    public static class J_InputUtils
    {
        private static readonly StringBuilder _stringBuilder = new StringBuilder(1024 * 10);

        private static readonly string Format = "({0}) === {1}/n";

        /// <summary>
        /// gets a list of all valid inputs and their state
        /// </summary>
        /// <param name="log">true to log the result also on the console</param>
        /// <returns>returns the string of all inputs</returns>
        public static string GetAvailableXRInputs(bool log)
        {
            var inputDevices = new List<InputDevice>();
            InputDevices.GetDevices(inputDevices);
            _stringBuilder.Clear();
            _stringBuilder.Append("(Inputs) === value ");
            foreach (var device in inputDevices)
            {
                var inputFeatures = new List<UnityEngine.XR.InputFeatureUsage>();
                if (!device.TryGetFeatureUsages(inputFeatures)) { continue; }

                foreach (InputFeatureUsage feature in inputFeatures)
                {
                    if (feature.type != typeof(bool)) { continue; }

                    if (device.TryGetFeatureValue(feature.As<bool>(), out bool featureValue) && featureValue)
                    {
                        _stringBuilder.AppendFormat(Format, feature.name, featureValue.ToString());
                    }
                }
            }

            string result = _stringBuilder.ToString();
            if (log) { JLog.Log(result, JLogTags.Input); }

            return result;
        }
    }
}
