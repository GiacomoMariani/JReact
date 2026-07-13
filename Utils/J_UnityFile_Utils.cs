#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    public static class J_UnityFile_Utils
    {
        public static List<T> GetObjectsAtPath<T>(string directoryPath, string keySearch = "*",
                                                  bool   searchInSubDirectories = true)
        {
            FileInfo[] fileInf = GetFileInfoAtPath(directoryPath, keySearch, searchInSubDirectories);
            var        found   = new List<T>();
            foreach (FileInfo fileInfo in fileInf)
            {
                string fullPath  = fileInfo.FullName.Replace(@"\", "/");
                string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
                if (AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) is T item) { found.Add(item); }
            }

            return found;
        }

        public static List<GameObject> GetGameObjectsAtPath(string directoryPath, string keySearch = "*",
                                                            bool   searchInSubDirectories = true)
        {
            FileInfo[] fileInf    = GetFileInfoAtPath(directoryPath, keySearch, searchInSubDirectories);
            var        itemsFound = new List<GameObject>();
            foreach (FileInfo fileInfo in fileInf)
            {
                string fullPath  = fileInfo.FullName.Replace(@"\", "/");
                string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
                if (AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) is GameObject prefab) { itemsFound.Add(prefab); }
            }

            return itemsFound;
        }

        public static List<Object> GetObjectsAtPath(string directoryPath, string keySearch = "*",
                                                    bool   searchInSubDirectories = true)
        {
            FileInfo[] fileInf    = GetFileInfoAtPath(directoryPath, keySearch, searchInSubDirectories);
            var        itemsFound = new List<Object>();
            foreach (FileInfo fileInfo in fileInf)
            {
                string fullPath  = fileInfo.FullName.Replace(@"\", "/");
                string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
                if (AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)) is Object prefab) { itemsFound.Add(prefab); }
            }

            return itemsFound;
        }

        public static List<ScriptableObject> GetScriptableObjectsAtPath(string directoryPath, string keySearch = "*",
                                                                        bool   searchInSubDirectories = true)
        {
            FileInfo[] fileInf    = GetFileInfoAtPath(directoryPath, keySearch, searchInSubDirectories);
            var        itemsFound = new List<ScriptableObject>();
            foreach (FileInfo fileInfo in fileInf)
            {
                string fullPath  = fileInfo.FullName.Replace(@"\", "/");
                string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
                if (AssetDatabase.LoadAssetAtPath(assetPath, typeof(ScriptableObject)) is ScriptableObject prefab)
                {
                    itemsFound.Add(prefab);
                }
            }

            return itemsFound;
        }

        public static FileInfo[] GetFileInfoAtPath(string directoryPath, string keySearch, bool searchInSubDirectories)
        {
            Assert.IsTrue(string.IsNullOrEmpty(directoryPath), $"Invalid Path: {directoryPath}");
            var dirInfo = new DirectoryInfo(directoryPath);
            FileInfo[] fileInf = searchInSubDirectories
                                     ? dirInfo.GetFiles(keySearch, SearchOption.AllDirectories)
                                     : dirInfo.GetFiles(keySearch);

            //loop through directory loading the game object and checking if it has the component you want
            return fileInf;
        }
        
        public static void CreateStepPrefab<T>(string name) where T : MonoBehaviour
        {
            // folder currently selected in the Project window (fallback: Assets)
            string folder = "Assets";
            if (UnityEditor.Selection.activeObject != null)
            {
                folder = AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
                if (!AssetDatabase.IsValidFolder(folder)) { folder = Path.GetDirectoryName(folder); }
            }

            string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{name}.prefab");

            var        go     = new GameObject(name, typeof(T));
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);

            UnityEditor.Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
        }
    }
}
#endif
