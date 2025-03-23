namespace JReact.SaveSystem.Unmanaged
{
    public struct JSerializeUtils
    {
        public readonly uint SerializationPrefix => 1989;
        public readonly uint SerializationSuffix => 2016;
        public readonly int FileSizeKb => 1024 * 100;
        public readonly int EmptySlotsDefault => 20;
        public readonly uint ZeroUInt => 0;
    }
}
