using JReact.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Tooltips
{
    public class J_TootlipRoot : J_MonoSingleton<J_TootlipRoot>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required] private Transform _root;
        public Transform Root => _root;
    }
}
