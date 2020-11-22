using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.J_Audio.View
{
    [RequireComponent(typeof(AudioSource))]
    public class J_Mono_AudioDirectPlayer : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private AudioSource _source;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SOAudio_Item _audio;

        // --------------- LISTENER SETUP --------------- //
        private void OnEnable()  { _audio.DirectPlay(_source); }
        private void OnDisable() { _source.Stop(); }
    }
}
