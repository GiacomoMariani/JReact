#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;

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
    }
}
#endif
