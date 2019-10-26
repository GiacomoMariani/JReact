using System;
using JReact.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.UiView.Collections
{
    public abstract class J_Abs_Pager<T> : MonoBehaviour
    {
        // --------------- EVENTS --------------- //
        public event Action<int> OnPage_Change;
        public event Action<int> OnPage_Create;
        public event Action<int> OnPage_Remove;

        // --------------- ABSTRACT --------------- //
        protected abstract iReactiveIndexCollection<T> _Collection { get; }
        protected abstract J_Mono_Actor<T>[] _instances { get; }


        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField, Min(1)] private int _itemsPerPage = 2;

        // --------------- STATE --------------- //

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _currentPageIndex;
        public int PageIndex => _currentPageIndex; 
        
        // --------------- BOOK KEEPING --------------- //
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool CanGoForward => PageIndex < TotalPages - 1;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool CanGoBack => PageIndex > 0;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int TotalPages
            => (_Collection.Length / _itemsPerPage) + 1;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool IsEmpty => TotalPages == 0;

        // --------------- COMMANDS --------------- //
        [ButtonGroup("Test", 200), Button(ButtonSizes.Medium)] public void GoForward() => PageChange(1);

        [ButtonGroup("Test", 200), Button(ButtonSizes.Medium)] public void GoBack() => PageChange(-1);

        private void PageChange(int indexToChange)
        {
            _currentPageIndex += indexToChange;
            SetPage(_currentPageIndex);
        }

        private void SetPage(int pageToSet)
        {
            if (!_Collection.ContainsIndex(pageToSet)) return;
            ShowFrom(pageToSet, _itemsPerPage);
            OnPage_Change?.Invoke(pageToSet);
        }

        private void ShowFrom(int page, int itemsPerPage)
        {
            int startingIndex = _currentPageIndex * _itemsPerPage;
            for (int i = 0; i < _itemsPerPage; i++)
            {
                int currentItem = i + _itemsPerPage;
                _instances[i]
                   .ActorUpdate(currentItem >= _Collection.Length
                                    ? default
                                    : _Collection[i + _itemsPerPage]);
            }
        }

        [ButtonGroup("Test", 200), Button(ButtonSizes.Medium)]
        public void Open()
        {
            Mathf.Clamp(_currentPageIndex, 0, _Collection.Length - 1);
            SetPage(_currentPageIndex);
        }

        // --------------- LISTENER SETUP --------------- //
        private void OnEnable() { Open(); }
        private void OnDisable()  {}
    }
}
