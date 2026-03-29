#if FJMOD_HELPER
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.J_Audio.FMod
{
    public sealed class JFMOD_Bus : MonoBehaviour
    {
        // --------------- CONST --------------- //
        public const float _MinVolume = 0.0f;
        public const float _MaxVolume = 1.0f;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _busName;
        [BoxGroup("Setup", true, true, 0), SerializeField, Range(0, 1)] private float _defaultVolume;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _deafultIsMuted;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _defaultIsPaused;

        // --------------- MAIN REFERENCE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public Bus AttachedBus { get; private set; }

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsReady => AttachedBus.isValid();

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float Volume
        {
            get
            {
                AttachedBus.getVolume(out var result);
                return result;
            }
            private set { AttachedBus.setVolume(value); }
        }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsMuted
        {
            get
            {
                AttachedBus.getMute(out var result);
                return result;
            }
            private set { AttachedBus.setMute(value); }
        }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsPaused
        {
            get
            {
                AttachedBus.getPaused(out var result);
                return result;
            }
            private set { AttachedBus.setPaused(value); }
        }

        // --------------- COMMANDS --------------- //
        internal void SetVolume(float volume)
        {
            volume = Mathf.Clamp(volume, _MinVolume, _MaxVolume);
            Volume = volume;
        }

        internal void SetMuted(bool isMuted) { IsMuted = isMuted; }

        internal void SetPaused(bool isPaused) { IsPaused = isPaused; }

        // --------------- GENERATORS --------------- //
        private void OnEnable() { CreateDefault(); }

        private void CreateDefault()
        {
            if (AttachedBus.isValid()) { return; }

            AttachedBus = RuntimeManager.GetBus(_busName);

            SetVolume(_defaultVolume);
            SetMuted(_deafultIsMuted);
            SetPaused(_defaultIsPaused);
        }
    }
}
#endif
