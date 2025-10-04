using Sirenix.OdinInspector;
using UnityEngine;

namespace Jreact.UiViewMono.UiAnimations
{
    [CreateAssetMenu(menuName = "Reactive/UI/AnimationTime", fileName = "J_AnimationTime", order = 0)]
    public sealed class J_SO_AnimationTime : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _animationName;
        
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0)] private int _milliseconds;
        public int Milliseconds => _milliseconds;
        public float Seconds => _milliseconds * 0.001f;
        public bool WantsAnimation => _milliseconds > 0;
        
        [TextArea(4, 10), SerializeField]
        private string _description = "";
    }
}
