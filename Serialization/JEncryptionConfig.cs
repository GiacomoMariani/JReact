namespace JReact.SaveSystem
{
    public readonly struct JEncryptionConfig
    {
        /// <field name="bufferSize">The size of the buffer used for reading the data.</field>
        public readonly int bufferSize;
        /// <field name="ivSize">The size of the initialization vector (IV) in bytes. Default is 16 bytes.</field>
        public readonly int ivSize;
        /// <field name="keySize">The size of the encryption key in bytes. Default is 16 bytes.</field>
        public readonly int keySize;
        /// <field name="iterations">The number of iterations for key derivation. Default is 100 iterations.</field>
        public readonly int iterations;

        public JEncryptionConfig(int bufferSize, int ivSize, int keySize, int iterations)
        {
            this.bufferSize = bufferSize;
            this.ivSize     = ivSize;
            this.keySize    = keySize;
            this.iterations = iterations;
        }

        public static readonly JEncryptionConfig DefaultConfig = new JEncryptionConfig(2048, 16, 16, 100);
    }
}
