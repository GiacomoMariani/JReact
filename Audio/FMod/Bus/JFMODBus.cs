#if FJMOD_HELPER
using System;
using System.Collections.Generic;
using FMOD.Studio;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.J_Audio.FMod
{
    [Serializable]
    public struct JFMODBus
    {
        // --------------- CONST --------------- //
        public const float _MinVolume = 0.0f;
        public const float _MaxVolume = 1.0f;

        // --------------- MAIN REFERENCE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]public readonly Bus AttachedBus;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]public readonly bool IsReady => AttachedBus.isValid();
        
        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float Volume
        {
            readonly get
            {
                AttachedBus.getVolume(out var result);
                return result;
            }
            private set { AttachedBus.setVolume(value); }
        }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsMuted
        {
            readonly get
            {
                AttachedBus.getMute(out var result);
                return result;
            }
            private set { AttachedBus.setMute(value); }
        }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsPaused
        {
            readonly get
            {
                AttachedBus.getPaused(out var result);
                return result;
            }
            private set { AttachedBus.setPaused(value); }
        }

        // --------------- CONSTRUCTO --------------- //
        internal JFMODBus(Bus bus) { AttachedBus = bus; }

        
        // --------------- COMMANDS --------------- //
        internal void SetVolume(float volume)
        {
            volume = Mathf.Clamp(volume, _MinVolume, _MaxVolume);
            Volume = volume;
        }

        internal void SetMuted(bool isMuted) { IsMuted = isMuted; }

        internal void SetPaused(bool isPaused) { IsPaused = isPaused; }
    }
    
    public static class JFMODBusCache
    {
        public static Dictionary<int, JFMODBus> _allBuses = new Dictionary<int, JFMODBus>();

        internal static void AddBus(int busID, JFMODBus bus) { _allBuses.Add(busID, bus); }

        internal static void RemoveBus(int busID) { _allBuses.Remove(busID); }

        internal static void RemoveAllBuses() { _allBuses.Clear(); }

        internal static JFMODBus GetBus(int busID)
        {
            bool bus = _allBuses.TryGetValue(busID, out var busFound);
            return busFound;
        }
    }
}
#endif
