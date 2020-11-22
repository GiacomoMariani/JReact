using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace JReact.J_Audio.View
{
    public class J_Mono_VolumeManager : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private string _mixerVolumeName;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private AudioMixer _mixer;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveFloat_Pref _volume;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        private void InitThis() { SetVolume(_volume.Current); }

        private void SanityChecks()
        {
            // --------------- REFERENCES --------------- //
            Assert.IsNotNull(_mixer,  $"{gameObject.name} requires a {nameof(_mixer)}");
            Assert.IsNotNull(_volume, $"{gameObject.name} requires a {nameof(_volume)}");
        }

        // --------------- VOLUME SET --------------- //
        private void SetVolume(float volume) => _mixer.SetFloat(_mixerVolumeName, volume);

        // --------------- LISTENER SETUP --------------- //
        private void OnEnable() { _volume.Subscribe(SetVolume); }

        private void OnDisable() { _volume.UnSubscribe(SetVolume); }
    }
}
