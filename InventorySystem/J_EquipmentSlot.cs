using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.InventorySystem
{
    /// <summary>
    /// an equipment slot, that might receive an equippable item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class J_EquipmentSlot<T> : jEquippableUser<T>
        where T : J_InventoryItemData
    {
        // --------------- FIELD AND PROPERTIES --------------- //
        public event Action<T> OnAssign;
        public event Action<T> OnRemove;

        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_EquipmentCategory _category;

        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] public T Equipped { get; private set; }

        // --------------- MAIN COMMAND --------------- //
        public void Equip(T item)
        {
            // --------------- VALIDATION --------------- //
            if (item.Category == _category)
            {
                JLog.Error($"Cannot accept {item.Category} on {_category.name}. Cancel Equip");
                return;
            }

            // --------------- REMOVE --------------- //
            if (Equipped != null) Remove();

            // --------------- EQUIP --------------- //
            Equipped = item;
            EquippedItem(item);
            OnAssign?.Invoke(item);
        }

        public void Remove()
        {
            // --------------- VALIDATION --------------- //
            if (Equipped == null)
            {
                JLog.Error($"No item equipped on slot for {_category.name}. Cancel Remove");
                return;
            }

            // --------------- REMOVE --------------- //
            T item = Equipped;
            Equipped = null;
            RemovedItem(item);
            OnRemove?.Invoke(item);
        }

        // --------------- FURTHER IMPLEMENTATION --------------- //
        protected virtual void EquippedItem(T item) {}
        protected virtual void RemovedItem(T  item) {}

        // --------------- SUBSCRIBERS --------------- //
        public void Subscribe(Action<T>   action) => OnAssign += action;
        public void UnSubscribe(Action<T> action) => OnAssign -= action;
    }
}
