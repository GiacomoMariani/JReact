#if NX_BITBUFFER
using System;
using NetStack.Serialization;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;

namespace JReact.SaveSystem
{
    //requires https://github.com/nxrighthere/NetStack/blob/master/Source/NetStack.Serialization/BitBuffer.cs
    public class JSerializer
    {
        private const int defaultCapacity = 375; // 375 * 4 = 1500 bytes
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public static JSerializer _Instance;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isReading;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isWriting;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool IsBusy => _isReading || _isWriting;

        private byte[] _byteCache;
        private BitBuffer _buffer;

        public static JSerializer GetMainInstance(int capacity = defaultCapacity) { return _Instance ??= new JSerializer(capacity); }

        public JSerializer(int capacity)
        {
            _buffer    = new BitBuffer(capacity);
            _byteCache = new byte[capacity];
        }

        // --------------- CHECKS --------------- //
        private void CheckForStartup()   { Assert.IsTrue(!IsBusy,    $"Was busy - reading ({_isReading}) - writing ({_isWriting})"); }
        private void IsReadyForWriting() { Assert.IsTrue(_isWriting, $"Writer not ready."); }
        private void IsReadyForReading() { Assert.IsTrue(_isReading, $"Reader not ready."); }

        // --------------- SERIALIZE --------------- //
        public JSerializer StartWriter()
        {
            CheckForStartup();
            _isWriting = true;
            return this;
        }

        public JSerializer Serialize(jSerializable serializable)
        {
            IsReadyForWriting();
            serializable.Serialize(_buffer);
            return this;
        }

        public byte[] ToByteArray(out int length)
        {
            IsReadyForWriting();
            _isWriting = false;
            Array.Clear(_byteCache, 0, _byteCache.Length);
            _buffer.ToArray(_byteCache);
            length = _buffer.Length;
            return _byteCache;
        }

        // --------------- DESERIALIZE --------------- //
        public JSerializer StartReaderFromBytes(byte[] bytes)
        {
            CheckForStartup();
            _isReading = true;
            _buffer.FromArray(bytes, bytes.Length);
            return this;
        }

        public JSerializer Deserialize(jSerializable serializable)
        {
            IsReadyForReading();
            serializable.DeSerialize(_buffer);
            return this;
        }

        public void EndReader() { _isReading = false; }
    }
}
#endif
