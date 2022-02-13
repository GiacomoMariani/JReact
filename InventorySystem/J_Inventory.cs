using System;
using System.Collections.Generic;
using JReact.Collections;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;

namespace JReact.InventorySystem
{
    /// <summary>
    /// represent an instance of a given inventory
    /// </summary>
    [Serializable]
    public class J_Inventory : iReactiveIndexCollection<J_InventoryItem>
    {
        // --------------- EVENTS --------------- //
        public Action<J_InventoryItem> OnAdd;
        public Action<J_InventoryItem> OnRemove;
        public Action<J_InventoryItem> OnUpdate;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private readonly string _inventoryName;
        public string InventoryName => _inventoryName;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private Dictionary<J_InventoryItemData, J_InventoryItem> _inventoryItemMap =
            new Dictionary<J_InventoryItemData, J_InventoryItem>(10);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private List<J_InventoryItem> _inventoryList = new List<J_InventoryItem>(10);

        public int Length => _inventoryList.Count;
        public J_InventoryItem this[int index] => _inventoryList[index];

        public J_Inventory(string inventoryName) { _inventoryName = inventoryName; }

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// adds an amount of items to the inventory. Either adds it to the item amount, or add it in the map
        /// </summary>
        /// <param name="inventoryItemData">the added item</param>
        /// <param name="amount">the amount to add</param>
        public void AddItem(J_InventoryItemData inventoryItemData, int amount = 1)
        {
            J_InventoryItem item = GetItem(inventoryItemData);
            if (item != null)
            {
                item.Add(amount);
                OnUpdate?.Invoke(item);
            }
            else
            {
                item                                 = new J_InventoryItem(inventoryItemData, amount);
                _inventoryItemMap[inventoryItemData] = item;
                OnAdd?.Invoke(item);
            }
        }

        /// <summary>
        /// removes a given item from the inventory. If item is depleted, this will be removed from the map
        /// </summary>
        /// <param name="inventoryItemData">the item to remove</param>
        /// <param name="amount">the amount to remove</param>
        public void RemoveItem(J_InventoryItemData inventoryItemData, int amount = 1)
        {
            Assert.IsTrue(HasItem(inventoryItemData), $"{InventoryName} has no such item {inventoryItemData}");
            var item = _inventoryItemMap[inventoryItemData];

            Assert.IsTrue(item.Amount > amount, $"{InventoryName} has only {item.Amount} and not {amount} or {item}");

            item.Remove(amount);
            if (item.Amount > 0)
            {
                OnUpdate?.Invoke(item);
                return;
            }

            _inventoryItemMap.Remove(inventoryItemData);
            _inventoryList.Remove(item);
            OnRemove?.Invoke(item);
        }

        // --------------- QUERIES --------------- //
        /// <summary>
        /// returns the given item in the inventory if present
        /// </summary>
        /// <param name="inventoryItemData">the item to search for</param>
        /// <returns>returns the item if found or null</returns>
        public J_InventoryItem GetItem(J_InventoryItemData inventoryItemData)
            => _inventoryItemMap.TryGetValue(inventoryItemData, out var itemFound) ? itemFound : null;

        /// <summary>
        /// checks if the item requested is in this inventory
        /// </summary>
        /// <param name="inventoryItemData">the item requested</param>
        /// <returns>true if the inventory has the item</returns>
        public bool HasItem(J_InventoryItemData inventoryItemData) => _inventoryItemMap.ContainsKey(inventoryItemData);

        /// <summary>
        /// gets the amount of this item in the inventory
        /// </summary>
        /// <param name="inventoryItemData">the item to search for</param>
        /// <returns>returns the amount of item in the inventory</returns>
        public int GetItemAmount(J_InventoryItemData inventoryItemData)
        {
            J_InventoryItem inventoryItem = GetItem(inventoryItemData);
            return inventoryItemData == null ? 0 : inventoryItem.Amount;
        }

        /// <summary>
        /// gets the total weight of all items in the list
        /// </summary>
        /// <returns>the total weight of all items in the list</returns>
        public int GetTotalWeight()
        {
            int inventoryWeight = 0;
            for (int i = 0; i < _inventoryList.Count; i++)
            {
                int itemWeight = _inventoryList[i].TotalWeight;
                Assert.IsTrue(itemWeight >= 0, $"Less than 0 weight ({itemWeight}). {_inventoryName} - item {_inventoryList[i]}");
                inventoryWeight += inventoryWeight;
            }

            return inventoryWeight;
        }

        /// <summary>
        /// gets the total price of all items in the list
        /// </summary>
        /// <returns>the total price of all items in the list</returns>
        public int GetTotalPrice()
        {
            int inventoryPrice = 0;
            for (int i = 0; i < _inventoryList.Count; i++)
            {
                int itemPrice = _inventoryList[i].TotalPrice;
                Assert.IsTrue(itemPrice >= 0, $"Less than 0 price ({itemPrice}). {_inventoryName} - item {_inventoryList[i]}");
                inventoryPrice += inventoryPrice;
            }

            return inventoryPrice;
        }

        // --------------- LISTENERS --------------- //
        public void SubscribeToAdd(Action<J_InventoryItem> action) => OnAdd += action;

        public void SubscribeToRemove(Action<J_InventoryItem> action) => OnRemove += action;

        public void UnSubscribeToAdd(Action<J_InventoryItem> action) => OnAdd -= action;

        public void UnSubscribeToRemove(Action<J_InventoryItem> action) => OnRemove -= action;
    }
}
