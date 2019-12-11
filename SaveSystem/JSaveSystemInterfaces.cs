namespace JReact.SaveSystem
{
    public interface jSerializable<T>
    {
        string NameOfThis { get; }
        T LastData { get; }
        T ConvertToData();
        void LoadFrom(T data);
    }

}
