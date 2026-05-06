using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.J_Audio.FMod
{
    public sealed class JFMOD_MusicPlayer : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private JFMOD_SoundDropper _dropper;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private JFMOD_OptionalEventInstance _music;

        public void PlayMusic(EventReference eventRef)
        {
            if (_music.IsAlive) { _dropper.StopAndDrop(_music); }
            
            _music = JFMOD_OptionalEventInstance.Create(eventRef);
            _music.Play();
        }
        
        public void StopMusic()
        {
            if (!_music.IsAlive) { return; }
            _dropper.StopAndDrop(_music);
            _music = JFMOD_OptionalEventInstance.Empty;
        }
    }
}
