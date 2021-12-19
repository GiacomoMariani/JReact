using JReact.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.UiView.Collections
{
    public abstract class J_Abs_Pager<T> : MonoBehaviour
    {
        // --------------- ABSTRACT --------------- //
        protected abstract iReactiveIndexCollection<T> _Collection { get; }
        protected abstract J_Mono_Actor<T>[] _instances { get; }

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_PagerEvents _events;
        [BoxGroup("Setup", true, true), SerializeField] private bool _resetPageAtClose = true;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _ItemsPerPage => _instances?.Length ?? 0;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _currentPageIndex;
        public int PageIndex => _currentPageIndex;

        // --------------- BOOK KEEPING --------------- //
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int TotalPages
        {
            get
            {
                if (_ItemsPerPage == 0) return 0;
                return (_Collection.Length + _ItemsPerPage - 1) / _ItemsPerPage;
            }
        }
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool IsEmpty => TotalPages == 0;

        // --------------- INIT --------------- //
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        private void SanityChecks()
        {
            Assert.IsNotNull(_events, $"{gameObject.name} requires a {nameof(_events)}");
            Assert.IsTrue(_instances.ArrayIsValid(), $"{gameObject.name} requires {nameof(_instances)}");
            Assert.IsNotNull(_Collection, $"{gameObject.name} requires a {nameof(_Collection)}");
            for (int i = 0; i < _instances.Length; i++)
                Assert.IsNotNull(_instances[i], $"{gameObject.name}. {nameof(_instances)} is null at index {i}");
        }

        private void InitThis()
        {
            _events.SetIndex(_currentPageIndex);
            _events.SetTotal(TotalPages);
        }

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
            ShowFrom(pageToSet, _ItemsPerPage);
            _events.SetIndex(pageToSet);
        }

        private void ShowFrom(int page, int itemsPerPage)
        {
            int startingIndex = page * itemsPerPage;
            for (int i = 0; i < itemsPerPage; i++)
            {
                int currentItem = i + startingIndex;
                _instances[i]
                   .ActorUpdate(currentItem >= _Collection.Length
                                    ? default
                                    : _Collection[currentItem]);
            }
        }

        [ButtonGroup("Test", 200), Button(ButtonSizes.Medium)]
        public void Open()
        {
            Mathf.Clamp(_currentPageIndex, 0, _Collection.Length - 1);
            SetPage(_currentPageIndex);
        }

        [ButtonGroup("Test", 200), Button(ButtonSizes.Medium)]
        public void Close()
        {
            if (_resetPageAtClose) _currentPageIndex = 0;
            if (gameObject.activeSelf) gameObject.SetActive(false);
        }

        // --------------- CHANGE EVENTS --------------- //
        private void ItemRemoved(T item)
        {
            if (IsEmpty)
            {
                Close();
                return;
            }

            //change page if the previous become empty
            if (_currentPageIndex                  >= TotalPages) PageChange(-1);
            if (_Collection.Length % _ItemsPerPage == 0) _events.SetTotal(TotalPages);
        }

        private void ItemAdded(T item)
        {
            if (_Collection.Length % _ItemsPerPage == 1) _events.SetTotal(TotalPages);
        }

        // --------------- LISTENER SETUP --------------- //
        private void OnEnable()
        {
            _Collection.SubscribeToAdd(ItemAdded);
            _Collection.SubscribeToRemove(ItemRemoved);
            Open();
        }

        private void OnDisable()
        {
            _Collection.UnSubscribeToAdd(ItemAdded);
            _Collection.UnSubscribeToRemove(ItemRemoved);
        }
    }
}
