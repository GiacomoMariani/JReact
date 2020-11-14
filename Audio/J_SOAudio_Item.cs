using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.J_Audio
{
    [CreateAssetMenu(menuName = "Reactive/Audio/AudioItem", fileName = "SO_AudioItem", order = 0)]
    public sealed class J_SOAudio_Item : ScriptableObject
    {
        // --------------- EVENTS --------------- //
        private event Action<J_SOAudio_Item> OnAudioEnd;

        // --------------- FIELDS AND PROPERTIES --------------- //
        private const string AudioPlayerTag = "AudioPlayerTag";
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private AudioClip _sound;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private List<AudioSource> _playingSources = new List<AudioSource>(2);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Mono_AudioSourcePool _sourcesPool;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsReady => _sourcesPool            != null;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsPlaying => _playingSources.Count > 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float SecondsDuration
        {
            get
            {
                if (_sound == null) return 0;
                return _sound.length;
            }
        }

        // --------------- INIT --------------- //
        internal void InjectPool(J_Mono_AudioSourcePool pool)
        {
            _sourcesPool = pool;
            SanityChecks(pool.Scope);
        }

        private void SanityChecks(AudioSourceScope poolScope) {}

        // --------------- COMMANDS --------------- //
        public void PlayAudio()
        {
            Assert.IsNotNull(_sourcesPool, $"{name} requires a {nameof(_sourcesPool)}");
            AudioSource source = _sourcesPool.AllocateAudioSource();
            _playingSources.Add(source);
            Timing.RunCoroutine(PlayAudioAndComplete(source, _sound.length), Segment.FixedUpdate, _instanceId, AudioPlayerTag);
        }

        public void PlayAudio(Vector3 position)
        {
            Assert.IsNotNull(_sourcesPool, $"{name} requires a {nameof(_sourcesPool)}");
            AudioSource source = _sourcesPool.AllocateAudioSource();
            source.transform.position = position;
            _playingSources.Add(source);
            Timing.RunCoroutine(PlayAudioAndComplete(source, _sound.length), Segment.FixedUpdate, _instanceId, AudioPlayerTag);
        }

        public void PlayAudio_OneShot()
        {
            Assert.IsNotNull(_sourcesPool, $"{name} requires a {nameof(_sourcesPool)}");
            var source = _sourcesPool.GetDefaultAudioSource();
            source.PlayOneShot(_sound);
        }

        public void Stop()
        {
            Timing.KillCoroutines(_instanceId, AudioPlayerTag);
            for (int i = 0; i < _playingSources.Count; i++)
            {
                AudioSource source = _playingSources[i];
                source.Stop();
                _sourcesPool.ReleaseAudioSource(source);
            }

            _playingSources.Clear();
        }

        // --------------- HELPERS --------------- //
        private IEnumerator<float> PlayAudioAndComplete(AudioSource source, float duration)
        {
            source.clip = _sound;
            source.Play();
            yield return Timing.WaitForSeconds(duration);
            Assert.IsFalse(source.isPlaying, $"{name} the sound is still playing");
            _sourcesPool.ReleaseAudioSource(source);
            _playingSources.Remove(source);
            OnAudioEnd?.Invoke(this);
        }

        // --------------- SUBSCRIBERS --------------- //
        public void Subscribe(Action<J_SOAudio_Item>   action) => OnAudioEnd += action;
        public void UnSubscribe(Action<J_SOAudio_Item> action) => OnAudioEnd -= action;
    }
}
