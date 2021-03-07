#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    /// <summary>
    /// used to display an array and convert into a dictionary with the name of the array element as key and the array value as value
    /// it can be used to reload the elements
    /// </summary>
    /// <typeparam name="TValue">the element to track</typeparam>
    /// <typeparam name="TKey">the key to track the item</typeparam>
    public abstract class J_ItemRetriever<TKey, TValue> : J_ReactiveDictionary<TKey, TValue>
    {
        //the array of buildings we want to have
        [BoxGroup("Setup", true, true), SerializeField] protected List<TValue> _items = new List<TValue>();

        /// <summary>
        /// checks if an element with given id can be found in this retriever
        /// </summary>
        public bool ContainsId(TKey id) => _Dictionary.ContainsKey(id);

        /// <summary>
        /// retrieves the item from the id
        /// </summary>
        /// <param name="id">the id to retrieve</param>
        /// <returns>returns the value requests</returns>
        public TValue GetItemFromId(TKey id)
        {
            if (_Dictionary       == null) { PopulateThis(); }

            Assert.IsTrue(_Dictionary.ContainsKey(id), $"{name} Key -{id}- not found");
            return _Dictionary[id];
        }

        /// <summary>
        /// checks if an item is in the array, low performance
        /// </summary>
        public bool ContainsThisValue(TValue item)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (EqualityComparer<TValue>.Default.Equals(item, _items[i])) { return true; }
            }

            return false;
        }

        /// <summary>
        /// add an item to the list, also at runtime,  low performance method (uses linq)
        /// </summary>
        public void AddItem(TValue item)
        {
            _items.Add(item);
            Add(GetItemId(item), item);
        }

        /// <summary>
        /// this is the main implementation to get the name from the element
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract TKey GetItemId(TValue item);

        // --------------- DISABLE AND RESET --------------- //
        [BoxGroup("Commands", true, true, 100), Button(ButtonSizes.Medium)]
        public virtual void PopulateThis()
        {
            //reset this, then add all the required item to the dictionary 
            Clear();
            for (int i = 0; i < _items.Count; i++) { Add(GetItemId(_items[i]), _items[i]); }
        }

        // --------------- ID SETTER --------------- //
#if UNITY_EDITOR
        [BoxGroup("Editor Only", true, true, 0), Button]
        private void SetAllIds()
        {
            for (ushort i = 0; i < _items.Count; i++)
            {
                SetIdOnToken(i, _items[i]);
                if (_items[i] is Object) { EditorUtility.SetDirty(_items[i] as Object); }
            }
        }

        protected virtual void SetIdOnToken(ushort index, TValue item) {}
#endif
    }
}
