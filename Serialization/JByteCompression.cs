using System.IO;
using System.IO.Compression;

namespace JReact.SaveSystem
{
    public static class JByteCompression
    {
        /// <summary>
        /// Compresses an array of bytes using the Deflate algorithm.
        /// </summary>
        /// <param name="binaryData">The array of bytes to be compressed.</param>
        /// <returns>The compressed array of bytes.</returns>
        public static byte[] Compress(byte[] binaryData)
        {
            using var resultStream  = new MemoryStream();
            using var deflateStream = new DeflateStream(resultStream, CompressionMode.Compress);
            deflateStream.Write(binaryData, 0, binaryData.Length);
            return resultStream.ToArray();
        }

        /// <summary>
        /// Decompresses an array of bytes using the Deflate algorithm.
        /// </summary>
        /// <param name="binaryCompressedData">The array of bytes to be decompressed.</param>
        /// <returns>The decompressed array of bytes.</returns>
        public static byte[] Decompress(byte[] binaryCompressedData)
        {
            using var sourceStream    = new MemoryStream(binaryCompressedData);
            using var deflatingStream = new DeflateStream(sourceStream, CompressionMode.Decompress);
            using var resultStream    = new MemoryStream();
            deflatingStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }
    }
}
