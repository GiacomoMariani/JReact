using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.J_Audio
{
    [CreateAssetMenu(menuName = "Reactive/Audio/Playlist/PlaylistCollection", fileName = "SO_PlaylistCollection", order = 0)]
    public sealed class J_SOAudio_Playlist : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_SOAudio_Sound[] _songs;
        [BoxGroup("Setup", true, true, 0), SerializeField, Min(0f)] private float _delayBetweenSongs = 0.1f;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Current { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public J_SOAudio_Sound CurrentSong { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool RandomPlay { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool Loop { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsPlaying => _handle.IsRunning;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private CoroutineHandle _handle;

        // --------------- ORDERED --------------- //
        public void StartPlaylist(int firstSong = 0, bool loop = true)
        {
            Loop    = loop;
            Current = firstSong;
            if (IsPlaying) { StopPlaylist(); }

            _handle = Timing.RunCoroutine(OrderedPlayList(), Segment.FixedUpdate);
        }

        private IEnumerator<float> OrderedPlayList()
        {
            while (true)
            {
                if (Current >= _songs.Length)
                {
                    if (Loop) { Current = 0; }
                    else
                    {
                        StopPlaylist();
                        yield break;
                    }
                }

                CurrentSong = _songs[Current];
                CurrentSong.PlayAudio();
                yield return Timing.WaitForSeconds(CurrentSong.SecondsDuration + _delayBetweenSongs);
                Current++;
            }
        }

        // --------------- RANDOM --------------- //
        public void StartRandom()
        {
            if (IsPlaying) { StopPlaylist(); }

            _handle = Timing.RunCoroutine(RandomPlayList(), Segment.FixedUpdate);
        }

        private IEnumerator<float> RandomPlayList()
        {
            while (true)
            {
                CurrentSong = _songs.GetRandomElement();
                CurrentSong.PlayAudio();
                yield return Timing.WaitForSeconds(CurrentSong.SecondsDuration + _delayBetweenSongs);
            }
        }

        // --------------- STOP --------------- //
        public void StopPlaylist() { Timing.KillCoroutines(_handle); }
    }
}
