using JReact.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.J_Audio
{
    /// <summary>
    /// used to make sure all the audio items are initiated with a transform
    /// </summary>
    public sealed class J_Mono_AudioInitiator : J_MonoSingleton<J_Mono_AudioInitiator>
    {
        // --------------- AUDIO SOURCES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_SO_ABS_AudioBase[] _2dAudioItems;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_SO_ABS_AudioBase[] _3dAudioItems;

        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Transform _2dPoolParent;
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Transform _3dPoolParent;

        internal override void InitThis()
        {
            base.InitThis();
            for (int i = 0; i < _2dAudioItems.Length; i++) { _2dAudioItems[i].InitThis(_2dPoolParent); }

            for (int i = 0; i < _3dAudioItems.Length; i++) { _3dAudioItems[i].InitThis(_3dPoolParent); }
        }
    }
}
