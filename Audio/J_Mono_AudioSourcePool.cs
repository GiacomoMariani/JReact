using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

namespace JReact.J_Audio
{
    internal enum AudioSourceScope { None = 0, Music = 50, UserInterface = 100, Sound2D = 150, Sound3D = 200 }

    public sealed class J_Mono_AudioSourcePool : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private AudioMixerGroup _mixer;

        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private AudioSource[] _audioSources;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_SOAudio_Item[] _audioItems;
        [BoxGroup("Setup", true, true, 0), SerializeField] private AudioSourceScope _scope;
        internal AudioSourceScope Scope => _scope;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private Queue<AudioSource> _availableSources = new Queue<AudioSource>(10);

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            for (int i = 0; i < _audioSources.Length; i++) { SanityChecks(_audioSources[i]); }

            InitThis();
        }

        private void SanityChecks(AudioSource source)
        {
            Assert.IsNotNull(source, $"{gameObject.name} requires a {nameof(source)}");
            Assert.IsFalse(source.playOnAwake, $"{name} source should not play on awake");

            if (_scope == AudioSourceScope.Music   ||
                _scope == AudioSourceScope.Sound2D ||
                _scope == AudioSourceScope.UserInterface)
            {
                Assert.IsTrue(source.spatialBlend == 0f, $"{name} musics should play as 2d");
            }

            if (_scope == AudioSourceScope.Music)
            {
                Assert.IsTrue(source.priority == 0, $"{name} requires a source with priority 0 for a playlist");
            }
        }

        private void InitThis()
        {
            _availableSources.Clear();
            for (int i = 0; i < _audioSources.Length; i++) { AddToPool(_audioSources[i]); }

            for (int i = 0; i < _audioItems.Length; i++) { _audioItems[i].InjectPool(this); }
        }

        private void AddToPool(AudioSource source)
        {
            _availableSources.Enqueue(source);
            source.outputAudioMixerGroup = _mixer;
        }

        // --------------- GETTERS --------------- //
        internal AudioSource GetDefaultAudioSource() { return _audioSources[0]; }

        internal AudioSource AllocateAudioSource()
        {
            if (_availableSources.Count == 0) return null;
            return _availableSources.Dequeue();
        }

        internal void ReleaseAudioSource(AudioSource source)
        {
            Assert.IsTrue(_audioSources.ArrayContains(source), $"{name} does not control source {source.gameObject.name}");
            _availableSources.Enqueue(source);
        }
    }
}
