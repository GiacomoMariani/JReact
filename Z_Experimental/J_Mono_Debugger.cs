using UnityEngine;

namespace JReact
{
    public class J_Mono_Debugger : MonoBehaviour
    {
        private void Awake()     { JLog.Log($"{gameObject.FullName()} - {nameof(Awake)}"); }
        private void Start()     { JLog.Log($"{gameObject.FullName()} - {nameof(Start)}"); }
        private void OnEnable()  { JLog.Log($"{gameObject.FullName()} - {nameof(OnEnable)}"); }
        private void OnDisable() { JLog.Log($"{gameObject.FullName()} - {nameof(OnDisable)}"); }
        private void OnDestroy() { JLog.Log($"{gameObject.FullName()} - {nameof(OnDestroy)}"); }
    }
}
