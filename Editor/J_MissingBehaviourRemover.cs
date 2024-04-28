#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace JReact.Editor
{
    public class J_MissingBehaviourRemover : MonoBehaviour
    {
        [Button]
        private void RemoveAll()
        {
            var allChild = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform g in allChild) {GameObjectUtility.RemoveMonoBehavioursWithMissingScript(g.gameObject);}
        }
        
        [Button]
        private void RemoveAllAndDestroyThis()
        {
            RemoveAll();
            DestroyImmediate(this);
        }
    }
}
#endif
