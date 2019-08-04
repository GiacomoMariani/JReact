using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.UiView.Collections
{
    /// <summary>
    /// controls the pages, used to setup, show and move forward/back
    /// </summary>
    public sealed class J_Pager : MonoBehaviour, iResettable
    {
        // --------------- EVENTS --------------- //
        public event Action<J_UiView_Page> OnPage_Change;
        public event Action<J_UiView_Page> OnPage_Create;
        public event Action<J_UiView_Page> OnPage_Remove;

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, MinValue(1)] private int _itemsPerPage = 9;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required, AssetsOnly] private J_UiView_Page _pagePrefab;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private Dictionary<GameObject, J_UiView_Page> _itemOnPages = new Dictionary<GameObject, J_UiView_Page>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _currentIndex;
        private int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                Current.Close();
                _currentIndex = value;
                Current.Open();
                OnPage_Change?.Invoke(Current);
            }
        }

        // --------------- BOOK KEEPING --------------- //
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool CanGoForward
            => CurrentIndex < _allPages.Count - 1;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool CanGoBack => CurrentIndex > 0;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int TotalPages => _allPages?.Count ?? 0;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool IsEmpty => TotalPages == 0;

        // --------------- PAGES --------------- //
        [FoldoutGroup("Pages", false, 10), ReadOnly, ShowInInspector]
        private List<J_UiView_Page> _allPages = new List<J_UiView_Page>();
        [FoldoutGroup("Pages", false, 10), ReadOnly, ShowInInspector] public J_UiView_Page Last => TotalPages == 0
                                                                                                       ? null
                                                                                                       : PageFromIndex(_allPages.Count - 1);
        [FoldoutGroup("Pages", false, 10), ReadOnly, ShowInInspector] public J_UiView_Page Current => TotalPages == 0
                                                                                                          ? null
                                                                                                         : PageFromIndex(CurrentIndex);

        // --------------- COMMANDS --------------- //
        [ButtonGroup("Test", 200), Button(ButtonSizes.Medium)]
        public void GoForward()
        {
            if (!CanGoForward) return;
            CurrentIndex++;
        }

        [ButtonGroup("Test", 200), Button(ButtonSizes.Medium)]
        public void GoBack()
        {
            if (!CanGoBack) return;
            CurrentIndex--;
        }

        [ButtonGroup("Test", 200), Button(ButtonSizes.Medium)]
        public void Open()
        {
            // --------------- CHECKS --------------- //
            if (IsEmpty)
            {
                JLog.Warning($"{gameObject.name} has no pages. Open canceled.", JLogTags.UiView, this);
                return;
            }

            Mathf.Clamp(_currentIndex, 0, _allPages.Count - 1);

            // --------------- OPEN --------------- //
            if(!gameObject.activeSelf) gameObject.SetActive(true);
            Current.Open();
            OnPage_Change?.Invoke(Current);
        }

        [ButtonGroup("Test", 200), Button(ButtonSizes.Medium)]
        public void Close()
        {
            // --------------- CHECKS --------------- //
            if (IsEmpty)
            {
                JLog.Warning($"{gameObject.name} has no pages. Close canceled.", JLogTags.UiView, this);
                return;
            }

            // --------------- CLOSE --------------- //
            Current.Close();
            if(gameObject.activeSelf) gameObject.SetActive(false);
        }

        public void OpenAtIndex(int indexToSet)
        {
            _currentIndex = indexToSet;
            Current.Open();
            OnPage_Change?.Invoke(Current);
        }
        
        // --------------- ADD PAGE --------------- //
        private J_UiView_Page CreatePage()
        {
            // --------------- INSTANTIATION --------------- //
            var page = Instantiate(_pagePrefab, transform, false);

            // --------------- PAGE SETUP --------------- //
            page.Setup(_allPages.Count, _itemsPerPage);
            _allPages.Add(page);

            // --------------- RETURN AND EVENTS --------------- //
            OnPage_Create?.Invoke(page);
            return page;
        }

        // --------------- REMOVE PAGE --------------- //
        private void RemovePage(J_UiView_Page page)
        {
            // --------------- CHECKS --------------- //
            Assert.IsTrue(page == Last, $"{name} - {page.gameObject.name} is not the last.");
            Assert.IsTrue(page.IsEmpty, $"{name} - {Last.gameObject.name} has no pages to remove");

            // --------------- REMOVE --------------- //
            _allPages.Remove(page);

            // --------------- DESTROY AND EVENTS --------------- //
            OnPage_Remove?.Invoke(page);
            DestroyPageView(page);
        }

        private void DestroyPageView(J_UiView_Page page)
        {
            //Move back of one page if we're destroying the page on view
            if (page == Current)
            {
                if (CurrentIndex > 0) OpenAtIndex(CurrentIndex - 1);
                else OnPage_Create += OpenAtCreation;
            }

            Destroy(page.gameObject);
        }

        //selects the new page created
        private void OpenAtCreation(J_UiView_Page page)
        {
            OnPage_Create -= OpenAtCreation;
            CurrentIndex  =  page.Identifier;
            Open();
        }

        // --------------- ADD ITEM --------------- //
        internal void AddItem(GameObject view)
        {
            J_UiView_Page page = Last;
            if (page == null ||
                page.IsFull) page = CreatePage();

            AddItemOnPage(page, view);
        }

        private void AddItemOnPage(J_UiView_Page page, GameObject view)
        {
            _itemOnPages[view] = page;
            page.AddItem(view);
        }

        // --------------- REMOVE ITEM --------------- //
        public void RemoveItem(GameObject view)
        {
            //find the page from the dictionary
            var page = _itemOnPages[view];
            //destroy the view
            DestroyItemView(view, page);
            //remove this if it is the last item
            if (page.IsEmpty) RemovePage(page);
            //update recursively
            else UpdateAll(page);
        }

        private void DestroyItemView(GameObject view, J_UiView_Page page)
        {
            page.RemoveItem(view);
            _itemOnPages.Remove(view);
            Destroy(view.gameObject);
        }

        private void UpdateAll(J_UiView_Page page)
        {
            while (page       != Last &&
                   TotalPages > 0)
            {
                J_UiView_Page next = _allPages[page.Identifier + 1];
                GameObject    item = next.GetFirstItem();

                AddItemOnPage(page, item);
                next.RemoveItem(item);
                page = next;
            }

            if (TotalPages == 0) return;
            if (Last.IsEmpty) RemovePage(Last);
        }

        // --------------- HELPER --------------- //
        private J_UiView_Page PageFromIndex(int index)
        {
            if (!_allPages.ContainsIndex(index))
            {
                JLog.Error($"{name} {index} is not a valid index {index}. Returning null.", JLogTags.UiView, this);
                return null;
            }

            J_UiView_Page page = _allPages[index];

            if (page == null)
            {
                JLog.Error($"{name} found a null page at index {index}", JLogTags.UiView, this);
                return null;
            }

            return page;
        }

        // --------------- RESET --------------- //
        [Button]
        public void ResetThis()
        {
            while (TotalPages != 0) RemovePage(_allPages[0]);

            J_UiView_Page[] otherPages = GetComponentsInChildren<J_UiView_Page>(true);
            foreach (J_UiView_Page page in otherPages) Destroy(page.gameObject);

            _currentIndex = 0;
            _allPages.Clear();
            _itemOnPages.Clear();
        }
        
        // --------------- LISTENER SETUP --------------- //
        private void OnEnable() => Open();
        private void OnDisable() => Close();
    }
}
