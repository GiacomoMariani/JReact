using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.UiView.Collections
{
    [CreateAssetMenu(menuName = "Reactive/Collection/Views/Pager Event")]
    public class J_PagerEvents : ScriptableObject
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        public event Action<int> OnTotalChanged;
        public event Action<int> OnIndexChanged;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Current { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Total { get; private set; }

        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool CanGoForward => Current < Total - 1;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool CanGoBack => Current    > 0;

        internal void SetTotal(int total)
        {
            Total = total;
            OnTotalChanged?.Invoke(total);
        }

        internal void SetIndex(int index)
        {
            Current = index;
            OnIndexChanged?.Invoke(index);
        }
    }
}
