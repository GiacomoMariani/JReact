using System;
using Sirenix.OdinInspector;

namespace JReact.InventorySystem
{
    /// <summary>
    /// the instance of an item ownable by an inventory
    /// </summary>
    [Serializable]
    public class J_InventoryItem
    {
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_InventoryItemData _inventoryItemData;
        public J_InventoryItemData InventoryItemData => _inventoryItemData;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _amount;
        public int Amount => _amount;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int TotalWeight => _amount * _inventoryItemData.Weight;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int TotalPrice => _amount  * _inventoryItemData.Price;

        public J_InventoryItem(J_InventoryItemData inventoryItemData, int amount = 1)
        {
            _inventoryItemData = inventoryItemData;
            _amount            = amount;
        }

        /// <summary>
        /// adds a given amount of this item to this
        /// </summary>
        /// <param name="amount">the amount to add</param>
        public void Add(int amount = 1) { _amount += amount; }

        /// <summary>
        /// removes a given amount of this item to this
        /// </summary>
        /// <param name="amount">the amount to remove</param>
        public void Remove(int amount = 1) { _amount -= amount; }

        public override string ToString() => $"Item Instance of {_inventoryItemData} - {Amount} weighting {TotalWeight}";
    }
}
