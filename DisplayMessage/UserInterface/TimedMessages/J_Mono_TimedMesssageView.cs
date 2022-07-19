using System.Collections.Generic;
using JReact.Pool;
using MEC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace JReact.ScreenMessage.MessageLogs
{
    public sealed class J_Mono_TimedMesssageView : MonoBehaviour, IPoolableItem<J_Mono_TimedMesssageView>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, ChildGameObjectsOnly, Required]
        private TextMeshProUGUI _text;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private IPool<J_Mono_TimedMesssageView> _parentPool;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private CoroutineHandle _handle;

        public void Display(string text, float seconds)
        {
            _text.SetText(text);
            _text.gameObject.SetActive(true);

            _handle = Timing.RunCoroutine(WaitThenDespawn(seconds).CancelWith(this), Segment.SlowUpdate);
        }

        private IEnumerator<float> WaitThenDespawn(float seconds)
        {
            yield return Timing.WaitForSeconds(seconds);
            _parentPool.DeSpawn(this);
        }

        public void SetPool(IPool<J_Mono_TimedMesssageView> pool) { _parentPool = pool; }

        private void OnDisable()
        {
            Timing.KillCoroutines(_handle);
            _text.SetText(JConstants.EmptyString);
        }
    }
}
