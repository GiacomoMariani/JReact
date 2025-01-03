#if FJMOD_HELPER
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.J_Audio.FMod
{
    //sample
    public sealed class FJMOD_Bank : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        //not current, but later we might want to connect the sounds with a bank
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required]
        private Bank _relatedBank;

        public Bank RelatedBank => _relatedBank;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [SerializeField] private EventReference _sample;
        public EventInstance NewSampleInstance => _sample.CreateInstance();
    }
}
#endif
