#if UNITY_DOTS
using Unity.Entities;

namespace JReact
{
    public static class JBlobExtensions
    {
        public static void SafeDispose<T>(this BlobAssetReference<T> blobReference) where T : unmanaged
        {
            if(blobReference.IsCreated) { blobReference.Dispose(); }
        }
    }
}
#endif
