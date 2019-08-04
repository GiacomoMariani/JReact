using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.UiView.Collections
{
    /// <summary>
    /// a single page with an amount of elements
    /// </summary>
    [RequireComponent(typeof(GridLayoutGroup))]
    public sealed class J_UiView_Page : MonoBehaviour, iResettable
    {
        // --------------- EVENTS --------------- //
        public event Action<J_UiView_Page> OnItem_Add;
        public event Action<J_UiView_Page> OnItem_Remove;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Identifier { get; private set; } = -1;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int MaxItems { get; private set; } = -1;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<GameObject> _items = new List<GameObject>();

        // --------------- BOOK KEEPING --------------- //
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public int TotalItems => _items.Count;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool IsEmpty => _items.Count == 0;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public bool IsFull => _items.Count  == MaxItems;

        // --------------- SETUP --------------- //
        internal void Setup(int pageId, int maxItems)
        {
            ResetThis();
            gameObject.name = pageId.ToString();
            Identifier      = pageId;
            MaxItems        = maxItems;
            _items          = new List<GameObject>(maxItems);
            //pages initialize disabled
            Close();
        }

        // --------------- ADD AND REMOVE --------------- //
        internal void AddItem(GameObject item)
        {
            if (IsFull)
            {
                JLog.Error($"{name} is full with {_items.Count} items.\n{_items.PrintAll()}", JLogTags.UiView, this);
                return;
            }

            _items.Add(item);
            item.transform.SetParent(transform);
            OnItem_Add?.Invoke(this);
        }

        internal void RemoveItem(GameObject item)
        {
            Assert.IsTrue(_items.Contains(item), $"{name} does not contain {item.name}");
            if (!_items.Contains(item))
            {
                JLog.Error($"{name} does not contain {item.name}", JLogTags.UiView, this);
                return;
            }

            _items.Remove(item);
            OnItem_Remove?.Invoke(this);
        }

        internal GameObject GetFirstItem() => _items[0];

        // --------------- OPEN AND CLOSE --------------- //
        internal void Open() => gameObject.SetActive(true);

        internal void Close() => gameObject.SetActive(false);

        // --------------- RESET AND DISABLE --------------- //
        public void ResetThis()
        {
            while (!IsEmpty) RemoveItem(_items[0]);
            _items.Clear();
        }

        private void OnEnable()
        {
            //fit the page
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.FitParent();
        }
    }
}
