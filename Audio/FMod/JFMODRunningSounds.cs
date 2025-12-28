#if FJMOD_HELPER

using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using PLAYBACK_STATE = FMOD.Studio.PLAYBACK_STATE;

namespace JReact.J_Audio.FMod
{
    public class JFMODRunningSounds
    {
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _maxSoundsPlaying = 5;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<EventInstance> _currentlyPlaying;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsPlaying => _currentlyPlaying.Count > 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int SoundsPlaying => _currentlyPlaying?.Count ?? 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int MaxSoundsPlaying => _maxSoundsPlaying;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int AvailableSlots => _maxSoundsPlaying - SoundsPlaying;

        public JFMODRunningSounds(int maxSoundsPlaying = 5)
        {
            Assert.IsTrue(maxSoundsPlaying > 0, $"{nameof(maxSoundsPlaying)} must be higher than 0. Current {maxSoundsPlaying}");
            _maxSoundsPlaying = maxSoundsPlaying;
            _currentlyPlaying = new List<EventInstance>(maxSoundsPlaying);
        }

        public void Update()
        {
            for (int i = _currentlyPlaying.Count - 1; i >= 0; i--)
            {
                EventInstance playingInstance = _currentlyPlaying[i];
                playingInstance.getPlaybackState(out PLAYBACK_STATE state);
                if (state != PLAYBACK_STATE.STOPPED) { continue; }

                playingInstance.release();
                _currentlyPlaying.RemoveAt(i);
            }
        }

        public void AddSound(EventReference eventReference)
        {
            if (_currentlyPlaying.Count >= _maxSoundsPlaying) { return; }

            EventInstance instance = eventReference.CreateInstance();
            instance.Play();
            _currentlyPlaying.Add(instance);
        }

        public void Add3dSound(EventReference eventReference, Vector3 position)
        {
            if (_currentlyPlaying.Count >= _maxSoundsPlaying) { return; }

            EventInstance instance = eventReference.CreateInstance();
            instance.PlayAtPosition(position);
            _currentlyPlaying.Add(instance);
        }
    }
}
#endif
