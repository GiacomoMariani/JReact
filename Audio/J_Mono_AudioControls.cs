using System.Collections.Generic;
using JReact.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.J_Audio
{
    public sealed class J_Mono_AudioControls : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private Transform _poolParent;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Mono_PlayingAudio _source;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _maxSounds = 8;
        [BoxGroup("Setup", true, true, 0), SerializeField] private J_AudioEnum _audioType;
        public J_AudioEnum AudioType => _audioType;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Pool<J_Mono_PlayingAudio> _sourcePool;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<J_Mono_PlayingAudio> _soundsPlaying;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int TotalSoundsPlaying => _soundsPlaying?.Count ?? 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsPlaying => TotalSoundsPlaying > 0;

        // --------------- INITIALIZATION --------------- //
        internal void InitThis()
        {
            Assert.IsNotNull(_source, $"{name} requires a {nameof(_source)}");
            _soundsPlaying = new List<J_Mono_PlayingAudio>(_maxSounds);
            _sourcePool    = J_Pool<J_Mono_PlayingAudio>.GetPool(_source, parent: _poolParent);
        }

        // --------------- COMMANDS - PLAY--------------- //
        /// <summary>
        /// play a sound directly, without moving the audio source
        /// do not call this at awake, wait at least for start
        /// </summary>
        /// <param name="sound">the sound to play</param>
        /// <param name="loop">if we want the sound looping</param>
        [Button]
        public J_Mono_PlayingAudio PlayAudio(AudioClip sound, bool loop = false)
        {
            var playingSound = _sourcePool.Spawn();
            PlayTheSound(sound, loop, playingSound);
            return playingSound;
        }

        /// <summary>
        /// play the sound at a given position.
        /// do not call this at awake, wait at least for start
        /// </summary>
        /// <param name="sound">the sound to play</param>
        /// <param name="position">the position where the sound will start</param>
        /// <param name="loop">if we want the sound looping</param>
        public J_Mono_PlayingAudio PlayAudioAtPosition(AudioClip sound, Vector3 position, bool loop = false)
        {
            var playingSound = _sourcePool.SpawnAtPosition(position);
            PlayTheSound(sound, loop, playingSound);
            return playingSound;
        }

        private void PlayTheSound(AudioClip sound, bool loop, J_Mono_PlayingAudio playingSound)
        {
            Assert.IsNotNull(sound,        $"{name} requires a {nameof(sound)}");
            Assert.IsNotNull(playingSound, $"{name} requires a {nameof(playingSound)}");

            _soundsPlaying.Add(playingSound);
            if (loop) { playingSound.PlayLoop(this, sound); }
            else { playingSound.PlayAndGetBack(this, sound); }
        }

        // --------------- COMMANDS - STOP --------------- //
        /// <summary>
        /// stops all the current sound
        /// do not call this at awake, wait at least for start
        /// </summary>
        [Button]
        public void StopEverything()
        {
            for (int i = 0; i < _soundsPlaying.Count; i++) { _soundsPlaying[i].StopAndSendBack(); }

            _soundsPlaying.Clear();
        }

        internal void SoundComplete(J_Mono_PlayingAudio playingSound)
        {
            _soundsPlaying.Remove(playingSound);
            _sourcePool.DeSpawn(playingSound);
        }
    }
}
