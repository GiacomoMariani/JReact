using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.InventorySystem
{
    /// <summary>
    /// an item that might be equippable
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Equipment/Item", fileName = "Item")]
    public class J_InventoryItemData : ScriptableObject
    {
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _id;
        public int Id => _id;
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _displayName;
        public string DisplayName => _displayName;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _weight = 1;
        public int Weight => _weight;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _price = 1;
        public int Price => _price;
        //todo implement non-stackable items
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _canStack;
        public bool CanStack => _canStack;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Sprite _view;
        public Sprite View => _view;
        //null = not equippable?
        [BoxGroup("Setup", true, true, 0), SerializeField] private J_EquipmentCategory _category;
        public J_EquipmentCategory Category => _category;

        public override string ToString() => $"{_id}-{_displayName}/{_category}. Price: {_price}. Weight: {_weight}";
    }
}
