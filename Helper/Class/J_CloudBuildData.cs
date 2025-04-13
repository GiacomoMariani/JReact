using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.CloudBuild
{
    public static class J_CloudBuildData
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        //the file manifest
        private static TextAsset _manifest;
        //the dictionary of the manifest
        private static Dictionary<string, object> _manifestDictionary;

        // --------------- CONSTANTS --------------- //
        private const string ManifestNameTxt = "UnityCloudBuildManifest.json";

        private const string VersionNumber = "buildNumber";

        // --------------- SETUP --------------- //
        //setup and check manifest and dictionary
        private static void GetManifest()
        {
            _manifest = (TextAsset) Resources.Load(ManifestNameTxt);
            if (_manifest == null) JLog.Error("No manifest found.");
            Assert.IsNotNull(_manifest, $"Retrieved no manifest file from resource file: {ManifestNameTxt}");
            _manifestDictionary = JsonUtility.FromJson<Dictionary<string, object>>(_manifest.text);
            if (_manifestDictionary == null) JLog.Error("No Manifest dictionary found");
        }

        //gets a key
        public static string GetKeyFromManifest(string key)
        {
#pragma warning disable 0162
#if UNITY_EDITOR
            return "";
#endif
            //key without a value sends a warning
            if (_manifestDictionary.ContainsKey(key))
            {
                JLog.Warning($"No key -{key}- found on manifest", JLogTags.Build);
                return "";
            }

            //return the given key
            return _manifestDictionary[key].ToString();
        }

        // --------------- PRINT ALL COMMAND --------------- //
        //print all the values of the manifest
        public static void PrintAllManifestValues()
        {
#if UNITY_EDITOR
            return;
#endif
            if (_manifest == null) GetManifest();
            if (_manifest == null) return;

            JLog.Log($"Build Manifest with {_manifestDictionary.Count} kvp", JLogTags.Build);
            foreach (KeyValuePair<string, object> kvp in _manifestDictionary)
            {
                string value = kvp.Value != null
                                   ? kvp.Value.ToString()
                                   : "";

                JLog.Log($"Key: {kvp.Key}, Value: {value}", JLogTags.Build);
            }
        }

        // --------------- DATA GETTERS --------------- //
        /// <summary>
        /// gets the cloud build data
        /// </summary>
        /// <returns>returns the version of the cloud build</returns>
        public static string GetCloudBuildVersion()
        {
#if UNITY_EDITOR
            return "";
#endif

            if (_manifest == null) GetManifest();
            if (_manifest == null) return "";

            return GetKeyFromManifest(VersionNumber);
        }
    }
}
