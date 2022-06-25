using System;
using System.Collections.Generic;
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
        private AudioClip _sound;
        [FoldoutGroup("State - Playing Sounds", false, 5), ReadOnly, ShowInInspector]
        private CoroutineHandle _soundHandle;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsPlaying => _controls != default;

        internal void PlayLoop(J_Mono_AudioControls controls, AudioClip sound)
        {
            _controls    = controls;
            _sound       = sound;
            _source.clip = sound;
            _source.loop = true;
            _source.Play();
        }

        internal void PlayAndGetBack(J_Mono_AudioControls controls, AudioClip sound)
        {
            _controls    = controls;
            _sound       = sound;
            _soundHandle = Timing.RunCoroutine(PlayAndComplete(_source, sound), Segment.LateUpdate);
        }

        private IEnumerator<float> PlayAndComplete(AudioSource source, AudioClip sound)
        {
            source.clip = sound;
            source.loop = false;
            source.Play();
            var duration = sound.length;
            yield return Timing.WaitForSeconds(duration + _timeToleranceAdjustment);
            Assert.IsFalse(source.isPlaying, $"{name} - {source.clip.name} still playing on {source.gameObject.name}");
            SendBack();
        }

        public void StopAndSendBack()
        {
            if (_source.isPlaying)
            {
                _source.Stop();
                Timing.KillCoroutines(_soundHandle);
            }

            SendBack();
        }

        private void SendBack()
        {
            OnComplete?.Invoke(this);
            _controls.SoundComplete(this);
            _sound       = default;
            _soundHandle = default;
            _controls    = default;
        }
    }
}
