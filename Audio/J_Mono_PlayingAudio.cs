using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.J_Audio
{
    public class J_Mono_PlayingAudio : MonoBehaviour
    {
        // --------------- EVENTS --------------- //
        public event Action<J_Mono_PlayingAudio> OnComplete;

        // --------------- FIELD AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private AudioSource _source;
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _timeToleranceAdjustment = 0.1f;

        [FoldoutGroup("State",                  false, 5), ReadOnly, ShowInInspector] private J_Mono_AudioControls _controls;
        [FoldoutGroup("State - Playing Sounds", false, 5), ReadOnly, ShowInInspector]
        public AudioClip Sound => _source?.clip;
        [FoldoutGroup("State - Playing Sounds", false, 5), ReadOnly, ShowInInspector]
        private CoroutineHandle _soundHandle;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsPlaying => _controls != default;

        // --------------- COMMAND - TWEAKINGS --------------- //
        public J_Mono_PlayingAudio WithVolume(float volume)
        {
            Assert.IsTrue(volume >= 0f, $"{name} volume too low: {volume}");
            Assert.IsTrue(volume >= 0f, $"{name} volume too high: {volume}");
            _source.volume = volume;
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
            _source.clip = default;
            _soundHandle = default;
            _controls    = default;
        }
    }
}
