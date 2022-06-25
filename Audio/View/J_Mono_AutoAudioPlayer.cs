using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.J_Audio.View
{
    public class J_Mono_AutoAudioPlayer : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private J_AudioEnum _audioType = J_AudioEnum.Background;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _removePreviousSoundsOfType = true;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _loop = true;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private AudioClip _clip;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Mono_PlayingAudio _audioPlaying;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        private void SanityChecks() { Assert.IsNotNull(_clip, $"{gameObject.name} requires a {nameof(_clip)}"); }

        private void InitThis()
        {
            J_MonoS_Audio.AssureInstanceInitialization();
            var controls = J_MonoS_Audio.Instance.GetControls(_audioType);
            if (_removePreviousSoundsOfType) { controls.StopEverything(); }

            _audioPlaying = controls.PlayAudio(_clip, _loop);
        }

        [Button]
        private void StopSound()
        {
            if (_audioPlaying != null &&
                _audioPlaying.IsPlaying) { _audioPlaying.StopAndSendBack(); }
        }

        private void OnDestroy() { StopSound(); }
    }
}
