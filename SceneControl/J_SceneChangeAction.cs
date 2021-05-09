using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.SceneControls
{
    [CreateAssetMenu(menuName = "Reactive/Scenes/Scene Change Action")]
    public class J_SceneChangeAction : J_ProcessableAction
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("State Control", true, true), SerializeField, AssetsOnly, Required]
        private J_SceneChanger _sceneChanger;
        [BoxGroup("State Control", true, true), SerializeField, AssetsOnly, Required]
        private string _desiredScene;

        [BoxGroup("Debug", true, true, 100), Button(ButtonSizes.Medium)]
        public override void Process() => _sceneChanger.LoadSceneDirect(_desiredScene);
    }
}
