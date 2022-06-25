using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JReact.J_Audio.View
{
    public class J_UiView_OnHoverAudio : MonoBehaviour, IPointerEnterHandler
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _singleSound = true;
        [BoxGroup("Setup", true, true, 0), SerializeField] private J_AudioEnum _audioType = J_AudioEnum.UI;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private AudioClip _clip;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Mono_PlayingAudio _playingSound;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_playingSound != null && _singleSound) { _playingSound.StopAndSendBack(); }

            _playingSound            =  _clip.PlaySound(_audioType);
            _playingSound.OnComplete += RemoveSound;
        }

        private void RemoveSound(J_Mono_PlayingAudio sound)
        {
            _playingSound.OnComplete -= RemoveSound;
            _playingSound            =  null;
        }
    }
}
