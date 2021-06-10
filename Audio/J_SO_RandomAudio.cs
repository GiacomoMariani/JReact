using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.J_Audio
{
    [CreateAssetMenu(menuName = "Reactive/Audio/AudioItem", fileName = "SO_AudioItem", order = 0)]
    public sealed class J_SO_RandomAudio : J_SO_ABS_AudioBase
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private AudioClip[] _allSounds;

        protected override AudioClip GetSound() => _allSounds.GetRandomElement();
    }
}
