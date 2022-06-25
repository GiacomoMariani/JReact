using System.Collections.Generic;
using JReact.Pool;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.J_Audio
{
    public abstract class J_SO_ABS_AudioBase : ScriptableObject
    {
        // --------------- CONSTANTS AND TANGS --------------- //
        private const string AudioPlayerTag = "AudioPlayerTag";
        private const float TimeToleranceAdjustment = 0.1f;
        private const int ExpectedPlayingSources = 2;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private AudioSource _sourcePrefab;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private List<AudioSource> _playingSources = new List<AudioSource>(ExpectedPlayingSources);

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsPlaying => _playingSources.Count > 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Pool<AudioSource> _sourcePool;

        // --------------- INITIALIZATION --------------- //
        public void InitThis(Transform poolParent)
        {
            Assert.IsNotNull(_sourcePrefab, $"{name} requires a {nameof(_sourcePrefab)}");
            _instanceId = GetHashCode();
            _sourcePool = J_Pool<AudioSource>.GetPool(_sourcePrefab, parent: poolParent);
        }

        // --------------- ABSTRACT ITEM --------------- //
        /// <summary>
        /// retrieves the desired sound
        /// </summary>
        /// <returns></returns>
        protected abstract AudioClip GetSound();

        // --------------- COMMANDS - PLAY--------------- //
        [Button]
        public void PlayAudio(bool loop = false)
        {
            Assert.IsNotNull(_sourcePool, $"{name} requires a {nameof(_sourcePool)}");
            var source = _sourcePool.Spawn();
            Assert.IsNotNull(source, $"{name} requires a {nameof(source)}");
            var sound = GetSound();
            Assert.IsNotNull(sound, $"{name} requires a {nameof(sound)}");
            Timing.RunCoroutine(PlayAndComplete(source, sound, loop), Segment.LateUpdate, _instanceId, AudioPlayerTag);
        }

        public void PlayAudioAtPosition(Vector3 position, bool loop = false)
        {
            Assert.IsNotNull(J_Mono_AudioInitiator.Instance, $"{name} audio pool not initiated");
            var sound = GetSound();
            Assert.IsNotNull(sound, $"{name} requires a {nameof(sound)}");
            var source = _sourcePool.SpawnAtPosition(position);
            Assert.IsNotNull(source, $"{name} requires a {nameof(source)}");
            Timing.RunCoroutine(PlayAndComplete(source, sound, loop), Segment.LateUpdate, _instanceId, AudioPlayerTag);
        }

        // --------------- COMMANDS - SAFE --------------- //
        /// <summary>
        /// play the sound in 2D making sure the audio source pool is initialized
        /// </summary>
        /// <param name="loop">true if we want to source to loop</param>
        public void PlaySafe(bool loop)
        {
            //try initializing it with the instance or init this ite directly
            J_Mono_AudioInitiator.AssureInstanceInitialization();
            if (_sourcePool == null) { InitThis(null); }

            PlayAudio(loop);
        }

        /// <summary>
        /// play the sound in 3D making sure the audio source pool is initialized
        /// </summary>
        /// <param name="position">the position where to play the sound</param>
        /// <param name="loop">true if we want to source to loop</param>
        public void PlaySafeAtPosition(Vector3 position, bool loop)
        {
            //try initializing it with the instance or init this ite directly
            J_Mono_AudioInitiator.AssureInstanceInitialization();
            if (_sourcePool == null) { InitThis(null); }

            PlayAudioAtPosition(position, loop);
        }

        // --------------- COMMANDS - STOP --------------- //
        [Button]
        public void Stop()
        {
            Timing.KillCoroutines(_instanceId, AudioPlayerTag);
            for (int i = 0; i < _playingSources.Count; i++)
            {
                AudioSource source = _playingSources[i];
                if (source == null) { return; }

                source.Stop();
                _sourcePool.DeSpawn(source);
            }

            _playingSources.Clear();
        }

        // --------------- HELPERS --------------- //
        private IEnumerator<float> PlayAndComplete(AudioSource source, AudioClip sound, bool loop)
        {
            _playingSources.Add(source);
            source.clip = sound;
            source.loop = loop;
            source.Play();
            if (loop) { yield break; }

            var duration = sound.length;
            yield return Timing.WaitForSeconds(duration + TimeToleranceAdjustment);
            Assert.IsFalse(source.isPlaying, $"{name} - {source.clip.name} still playing on {source.gameObject.name}");
            _playingSources.Remove(source);
            _sourcePool.DeSpawn(source);
        }
    }
}
