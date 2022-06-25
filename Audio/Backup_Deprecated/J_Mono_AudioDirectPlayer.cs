using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.J_Audio.View
{
    [RequireComponent(typeof(AudioSource))]
    public class J_Mono_AudioDirectPlayer : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private bool _loop = true;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SO_ABS_AudioBase _audio;

        // --------------- LISTENER SETUP --------------- //
        private void OnEnable() { _audio.PlaySafe(_loop); }
    }
}
