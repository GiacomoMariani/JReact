namespace JReact.SaveSystem
{
    public interface iSaveable<T>
    {
        T LastData { get; }
        T ConvertToData();
        void LoadFrom<T>(T data);
    }

}
