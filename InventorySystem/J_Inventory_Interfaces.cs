namespace JReact.InventorySystem
{
    public interface jEquippableUser<T>
    where T : J_InventoryItemData
    {
        T Equipped { get; }
        void Equip(T item);
        void Remove();
    }
}
