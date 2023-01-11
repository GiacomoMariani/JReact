using Cysharp.Threading.Tasks;
using UnityEngine;

namespace JReact.J_Audio
{
    public static class J_AudioExtensions
    {
        private static J_MonoS_Audio _Audio = J_MonoS_Audio.GetInstanceSafe();

        internal static void AssignNew(J_MonoS_Audio audio) => _Audio = audio;

        public static J_Mono_PlayingAudio PlaySound(this AudioClip sound, J_AudioEnum audioType, bool loop = false)
            => _Audio.PlaySound(sound, audioType, loop);

        public static J_Mono_PlayingAudio PlayRandomSound(this AudioClip[] sounds, J_AudioEnum audioType, bool loop = false)
            => _Audio.PlaySound(sounds.GetRandomElement(), audioType, loop);

        public static J_Mono_PlayingAudio PlaySoundAtPosition(this AudioClip sound, Vector3 position, J_AudioEnum audioType,
                                                              bool           loop = false)
            => _Audio.PlaySoundAtPosition(sound, position, audioType, loop);

        public static J_Mono_PlayingAudio PlayRandomSoundAtPosition(this AudioClip[] sounds, Vector3 position, J_AudioEnum audioType,
                                                                    bool             loop = false)
            => _Audio.PlaySoundAtPosition(sounds.GetRandomElement(), position, audioType, loop);

        public static async UniTask<J_Mono_PlayingAudio> AttachSoundAfter(this J_Mono_PlayingAudio audio,    AudioClip   sound,
                                                                          Vector3                  position, J_AudioEnum audioType,
                                                                          bool                     loop = false)
        {
            await audio.WaitForEndSound();
            return PlaySoundAtPosition(sound, position, audioType, loop);
        }
    }
}
