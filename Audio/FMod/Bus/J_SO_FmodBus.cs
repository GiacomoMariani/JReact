#if FJMOD_HELPER
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.J_Audio.FMod
{
    [CreateAssetMenu(menuName = "Reactive/FMod/Bus", fileName = "FMOD_Bus", order = 0)]
    public sealed class J_SO_FmodBus : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _busName;
        [BoxGroup("Setup", true, true, 0), SerializeField, Range(0, 1)] private float _defaultVolume;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _deafultIsMuted;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _defaultIsPaused;
        
        // --------------- GENERATORS --------------- //
        internal JFMODBus CreateDefault()
        {
            Bus bus = RuntimeManager.GetBus(_busName);

            var busInstance = new JFMODBus(bus); 
                
            busInstance.SetVolume(_defaultVolume);
            busInstance.SetMuted(_deafultIsMuted);
            busInstance.SetPaused(_defaultIsPaused);

            JFMODBusCache.AddBus(GetHashCode(), busInstance);
            return busInstance;
        }
        
        public JFMODBus Create(float volume, bool isMuted = false, bool isPaused = false)
        {
            Bus bus = RuntimeManager.GetBus(_busName);

            var busInstance = new JFMODBus(bus); 
                
            busInstance.SetVolume(volume);
            busInstance.SetMuted(isMuted);
            busInstance.SetPaused(isPaused);

            JFMODBusCache.AddBus(GetHashCode(), busInstance);
            return busInstance;
        }
        
        // --------------- GETTERS --------------- //
        public JFMODBus GetBus() => JFMODBusCache.GetBus(GetHashCode());
    }
}
#endif
