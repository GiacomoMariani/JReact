using System.Collections.Generic;
using UnityEngine;

namespace JReact
{
    public static class JSO_Map<TScriptableObject>
        where TScriptableObject : ScriptableObject
    {
        private static Dictionary<int, TScriptableObject> _mapIdToScriptableObject = new Dictionary<int, TScriptableObject>();
        public static int Count => _mapIdToScriptableObject.Count;

        public static TScriptableObject FromId(int id) => _mapIdToScriptableObject[id];

        public static bool IsRegistered(int key) => _mapIdToScriptableObject.ContainsKey(key);
        
        public static void Add(TScriptableObject scriptableObject, int id)
            => _mapIdToScriptableObject.Add(id, scriptableObject);
    }
}
