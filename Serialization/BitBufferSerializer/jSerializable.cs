#if NX_BITBUFFER
using NetStack.Serialization;

namespace JReact.SaveSystem
{
    public interface jSerializable
    {
        void Serialize(BitBuffer   bitBuffer);
        void DeSerialize(BitBuffer bitBuffer);
    }
}
#endif