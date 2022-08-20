using System.Collections.Generic;
using JReact.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.J_Audio
{
    public class J_MonoS_Audio : J_MonoSingleton<J_MonoS_Audio>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private Dictionary<J_AudioEnum, J_Mono_AudioControls> _controls = new Dictionary<J_AudioEnum, J_Mono_AudioControls>();

        protected internal override void InitThis()
        {
            base.InitThis();
            //find all audio in the scene
            var controls = GetComponentsInChildren<J_Mono_AudioControls>(true);
            Assert.IsTrue(controls.Length > 1, $"{name} found no audio controls");
            for (int i = 0; i < controls.Length; i++)
            {
                var audioControl = controls[i];
                var audioType    = audioControl.AudioType;
                audioControl.InitThis();
                if (_controls.ContainsKey(audioType))
                {
                    JLog.Warning($"{name} already contains {audioType}:/n" +
                                 $" {_controls[audioType].gameObject}. Replacing with {audioControl.gameObject}", JLogTags.Audio,
                                 this);
                }

                _controls[audioType] = audioControl;
            }
        }

        /// <summary>
        /// play a given audio directly, using the specified type
        /// </summary>
        /// <param name="sound">the sound we want to play</param>
        /// <param name="audioType">the type of sound, such as 2d or 3d</param>
        /// <param name="loop">if we want the sound looping</param>
        /// <returns>returns the sound control for further flexibility (IE stop sound on demand)</returns>
        public J_Mono_PlayingAudio PlaySound(AudioClip sound, J_AudioEnum audioType, bool loop)
        {
            Assert.IsTrue(_controls.ContainsKey(audioType), $"{name} has no {audioType} registered");
            return _controls[audioType].PlayAudio(sound, loop);
        }

        /// <summary>
        /// play a given audio directly, using the specified type
        /// removes all other previous sounds 
        /// </summary>
        /// <param name="sound">the sound we want to play</param>
        /// <param name="audioType">the type of sound, such as 2d or 3d</param>
        /// <param name="loop">if we want the sound looping</param>
        /// <returns>returns the sound control for further flexibility (IE stop sound on demand)</returns>
        public J_Mono_PlayingAudio PlaySoundAlone(AudioClip sound, J_AudioEnum audioType, bool loop)
        {
            Assert.IsTrue(_controls.ContainsKey(audioType), $"{name} has no {audioType} registered");
            _controls[audioType].StopEverything();
            return _controls[audioType].PlayAudio(sound, loop);
        }

        /// <summary>
        /// play a given audio directly, using the specified type, at a specific position
        /// </summary>
        /// <param name="sound">the sound we want to play</param>
        /// <param name="audioType">the type of sound, such as 2d or 3d</param>
        /// <param name="loop">if we want the sound looping</param>
        /// <returns>returns the sound control for further flexibility (IE stop sound on demand)</returns>
        public J_Mono_PlayingAudio PlaySoundAtPosition(AudioClip sound, Vector3 position, J_AudioEnum audioType, bool loop)
        {
            Assert.IsTrue(_controls.ContainsKey(audioType), $"{name} has no {audioType} registered");
            return _controls[audioType].PlayAudioAtPosition(sound, position, loop);
        }

        /// <summary>
        /// gets an audio control of a given type
        /// </summary>
        /// <param name="audioType">the related </param>
        /// <returns>returns the requested audio controls</returns>
        public J_Mono_AudioControls GetControls(J_AudioEnum audioType)
        {
            Assert.IsTrue(_controls.ContainsKey(audioType), $"{name} has no {audioType} registered");
            return _controls[audioType];
        }
    }
}
