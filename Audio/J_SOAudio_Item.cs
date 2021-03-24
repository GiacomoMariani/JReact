using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.J_Audio
{
    [CreateAssetMenu(menuName = "Reactive/Audio/AudioItem", fileName = "SO_AudioItem", order = 0)]
    public sealed class J_SOAudio_Item : J_SO_ABS_SoundBase
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private const string AudioPlayerTag = "AudioPlayerTag";
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private AudioClip _sound;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float SecondsDuration
        {
            get
            {
                if (_sound == null) return 0;
                return _sound.length;
            }
        }

        protected override AudioClip GetSound() => _sound;
    }
}
