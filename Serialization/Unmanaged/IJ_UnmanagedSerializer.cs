namespace JReact.SaveSystem.Unmanaged
{
    public interface IJ_UnmanagedSerializer
    {
        void Serialize(ref JBinaryWriter bitBuffer);
        void DeSerialize(in JBinaryReader  bitBuffer);
    }
}
