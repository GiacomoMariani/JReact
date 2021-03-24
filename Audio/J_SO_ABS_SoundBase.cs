using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.J_Audio
{
    public abstract class J_SO_ABS_SoundBase : ScriptableObject
    {
        // --------------- EVENTS --------------- //
        private const float _timeToleranceAdjustment = 0.1f;
        private event Action<J_SO_ABS_SoundBase> OnAudioEnd;

        // --------------- FIELDS AND PROPERTIES --------------- //
        private const string AudioPlayerTag = "AudioPlayerTag";

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private List<AudioSource> _playingSources = new List<AudioSource>(2);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Mono_AudioSourcePool _sourcesPool;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsReady => _sourcesPool            != null;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsPlaying => _playingSources.Count > 0;

        // --------------- ABSTRACT ITEM --------------- //
        protected abstract AudioClip GetSound();

        // --------------- INIT --------------- //
        internal void InjectPool(J_Mono_AudioSourcePool pool)
        {
            _sourcesPool = pool;
            Stop();
            SanityChecks(pool.Scope);
        }

        private void SanityChecks(AudioSourceScope poolScope) {}

        // --------------- COMMANDS --------------- //
        [Button]
        public void PlayAudio(bool loop = false)
        {
            Assert.IsNotNull(_sourcesPool, $"{name} requires a {nameof(_sourcesPool)}");
            var sound = GetSound();
            Assert.IsNotNull(sound, $"{name} requires a {nameof(sound)}");
            AudioSource source = _sourcesPool.AllocateAudioSource();
            _playingSources.Add(source);
            Timing.RunCoroutine(PlayAudioAndComplete(sound, source, loop), Segment.FixedUpdate, _instanceId, AudioPlayerTag);
        }

        public void PlayAudio(Vector3 position, bool loop = false)
        {
            Assert.IsNotNull(_sourcesPool, $"{name} requires a {nameof(_sourcesPool)}");
            var sound = GetSound();
            Assert.IsNotNull(sound, $"{name} requires a {nameof(sound)}");
            AudioSource source = _sourcesPool.AllocateAudioSource();
            source.transform.position = position;
            _playingSources.Add(source);
            Timing.RunCoroutine(PlayAudioAndComplete(sound, source, loop), Segment.FixedUpdate, _instanceId, AudioPlayerTag);
        }

        [Button]
        public void PlayAudio_OneShot()
        {
            Assert.IsNotNull(_sourcesPool, $"{name} requires a {nameof(_sourcesPool)}");
            var sound = GetSound();
            Assert.IsNotNull(sound, $"{name} requires a {nameof(sound)}");
            var source = _sourcesPool.GetDefaultAudioSource();
            source.PlayOneShot(sound);
        }

        [Button]
        public void Stop()
        {
            Timing.KillCoroutines(_instanceId, AudioPlayerTag);
            for (int i = 0; i < _playingSources.Count; i++)
            {
                AudioSource source = _playingSources[i];
                if (source == null) return;
                source.Stop();
                _sourcesPool.ReleaseAudioSource(source);
            }

            _playingSources.Clear();
        }

        public void DirectPlay(AudioSource source)
        {
            var sound = GetSound();
            Assert.IsNotNull(sound, $"{name} requires a {nameof(sound)}");
            source.clip = sound;
            source.Play();
        }

        // --------------- HELPERS --------------- //
        private IEnumerator<float> PlayAudioAndComplete(AudioClip sound, AudioSource source, bool loop)
        {
            source.clip = sound;
            source.loop = loop;
            var duration = sound.length;
            source.Play();
            if (loop) { yield break; }

            yield return Timing.WaitForSeconds(duration + _timeToleranceAdjustment);
            Assert.IsFalse(source.isPlaying, $"{name} the sound is still playing");
            _sourcesPool.ReleaseAudioSource(source);
            _playingSources.Remove(source);
            OnAudioEnd?.Invoke(this);
        }

        // --------------- SUBSCRIBERS --------------- //
        public void Subscribe(Action<J_SO_ABS_SoundBase>   action) => OnAudioEnd += action;
        public void UnSubscribe(Action<J_SO_ABS_SoundBase> action) => OnAudioEnd -= action;
    }
}
