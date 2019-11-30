using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.JScreen
{
    public class J_Mono_CameraAssigner : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] protected J_ReactiveCamera _reactiveCamera;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Camera _camera;
        private Camera ThisCamera
        {
            get
            {
                if (_camera == null) _camera = GetComponent<Camera>();
                return _camera;
            }
        }

        private void Awake() => _reactiveCamera.Current = ThisCamera;
    }
}
