using Sirenix.OdinInspector;
using UnityEngine;

namespace Jreact._Basics.ECS.UiViewMono.UiAnimations
{
    /// <summary>
    /// A project's base choreography: one animation data per dictionary concept, authored to the project's theme.
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/UI/AnimationLibrary", fileName = "J_UiAnimationLibrary", order = 1)]
    public sealed class JUI_SO_AnimationLibrary : ScriptableObject
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JUI_SO_AnimationData _appearFade;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JUI_SO_AnimationData _appearMove;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JUI_SO_AnimationData _disappearFade;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JUI_SO_AnimationData _disappearMove;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JUI_SO_AnimationData _confirm;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JUI_SO_AnimationData _deny;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JUI_SO_AnimationData _received;

        public JUI_SO_AnimationData AppearFade => _appearFade;
        public JUI_SO_AnimationData AppearMove => _appearMove;
        public JUI_SO_AnimationData DisappearFade => _disappearFade;
        public JUI_SO_AnimationData DisappearMove => _disappearMove;
        public JUI_SO_AnimationData Confirm => _confirm;
        public JUI_SO_AnimationData Deny => _deny;
        public JUI_SO_AnimationData Received => _received;
    }
}
