using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.J_Audio
{
    /// <summary>
    /// Represents a MonoBehaviour class used for playing audio.
    /// </summary>
    public class J_Mono_PlayingAudio : MonoBehaviour
    {
        // --------------- EVENTS --------------- //
        public event Action<J_Mono_PlayingAudio> OnComplete;

        // --------------- FIELD AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private AudioSource _source;
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _timeToleranceAdjustment = 0.1f;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float Volume
        {
            get => _source.volume;
            private set => _source.volume = value;
        }
        [FoldoutGroup("State",                  false, 5), ReadOnly, ShowInInspector] private float _defaultVolume;
        [FoldoutGroup("State",                  false, 5), ReadOnly, ShowInInspector] private J_Mono_AudioControls _controls;
        [FoldoutGroup("State - Playing Sounds", false, 5), ReadOnly, ShowInInspector]
        public AudioClip Sound => _source?.clip;
        [FoldoutGroup("State - Playing Sounds", false, 5), ReadOnly, ShowInInspector]
        private CoroutineHandle _soundHandle;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsPlaying => _controls != default;

        // --------------- INITIALIZATION --------------- //
        private void Awake() { InitThis(); }

        private void InitThis() { _defaultVolume = Volume; }

        // --------------- COMMAND - TWEAKINGS --------------- //
        /// <summary>
        /// Sets the volume of the audio.
        /// </summary>
        /// <param name="volume">The volume value to set.</param>
        /// <returns>The reference to the current instance of J_Mono_PlayingAudio.</returns>
        public J_Mono_PlayingAudio WithVolume(float volume)
        {
            Assert.IsTrue(volume >= 0f, $"{name} volume too low: {volume}");
            Assert.IsTrue(volume >= 0f, $"{name} volume too high: {volume}");
            _defaultVolume = Volume;
            Volume         = volume;
            return this;
        }

        /// <summary>
        /// just wait until the sound finished playing
        /// </summary>
        public async UniTask WaitForEndSound()
        {
            while (IsPlaying) { await UniTask.Delay(10); }
        }

        // --------------- PLAY --------------- //
        internal void PlayLoop(J_Mono_AudioControls controls, AudioClip sound)
        {
            _controls    = controls;
            _source.clip = sound;
            _source.loop = true;
            _source.Play();
        }

        internal void PlayAndGetBack(J_Mono_AudioControls controls, AudioClip sound)
        {
            _controls    = controls;
            _soundHandle = Timing.RunCoroutine(PlayAndComplete(sound), Segment.LateUpdate);
        }

        private IEnumerator<float> PlayAndComplete(AudioClip sound)
        {
            _source.clip = sound;
            _source.loop = false;
            _source.Play();
            var duration = sound.length;
            yield return Timing.WaitForSeconds(duration + _timeToleranceAdjustment);
            SendBack();
        }

        // --------------- STOP --------------- //
        /// <summary>
        /// used to stop the sound forcefully
        /// </summary>
        public void StopAndSendBack()
        {
            _source.Stop();
            Timing.KillCoroutines(_soundHandle);

            SendBack();
        }

        private void SendBack()
        {
            OnComplete?.Invoke(this);
            _controls.SoundComplete(this);
            Volume       = _defaultVolume;
            _source.clip = default;
            _soundHandle = default;
            _controls    = default;
        }
    }
}
