using System.IO;
using System.Security.Cryptography;

namespace JReact.SaveSystem
{
    public static class JByteEncryption
    {
        // --------------- ENCRYPTION --------------- //
        /// <summary>
        /// Encrypts the given data using AES encryption.
        /// </summary>
        /// <param name="data">The data to be encrypted.</param>
        /// <param name="password">The password used for encryption.</param>
        /// <returns>The encrypted data as a byte array.</returns>
        public static byte[] DefaultEncrypt(byte[] data, string password) => Encrypt(data, password, JEncryptionConfig.DefaultConfig);

        /// <summary>
        /// Encrypts the given data using AES encryption.
        /// </summary>
        /// <param name="data">The data to be encrypted.</param>
        /// <param name="password">The password used for encryption.</param>
        /// <param name="encryptionConfig"></param>
        /// <returns>The encrypted data as a byte array.</returns>
        private static byte[] Encrypt(byte[] data, string password, JEncryptionConfig encryptionConfig)
        {
            using var sourceStream = new MemoryStream(data);
            using var resultStream = new MemoryStream();
            Encrypt_Impl(sourceStream, resultStream, password, encryptionConfig);
            return resultStream.ToArray();
        }

        /// <summary>
        /// Encrypts the given data from the source stream using AES encryption and writes the encrypted data to the result stream.
        /// </summary>
        private static void Encrypt_Impl(MemoryStream      sourceStream, MemoryStream resultStream, string password,
                                         JEncryptionConfig encryptionConfig)
        {
            sourceStream.Position = 0;

            using var aesAlgorithm = Aes.Create();
            aesAlgorithm.Mode    = CipherMode.CBC;
            aesAlgorithm.Padding = PaddingMode.PKCS7;
            aesAlgorithm.GenerateIV();

            byte[] initVector = aesAlgorithm.IV;
            aesAlgorithm.Key = GenerateKey(password, initVector, encryptionConfig.keySize, encryptionConfig.iterations);

            resultStream.Write(initVector, 0, encryptionConfig.ivSize);
            using ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor();
            using var              cs        = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write);

            CopyStream(sourceStream, cs, encryptionConfig.bufferSize);
        }

        /// <summary>
        /// Generates a key for encryption based on the provided password, salt, key size, and number of iterations.
        /// </summary>
        /// <param name="password">The password used for generating the key.</param>
        /// <param name="salt">The salt value used for key derivation.</param>
        /// <param name="keySize">The size of the key in bytes.</param>
        /// <param name="iterations">The number of iterations for key derivation.</param>
        /// <returns>A byte array representing the generated key.</returns>
        private static byte[] GenerateKey(string password, byte[] salt, int keySize, int iterations)
        {
            var keyGenerator = new Rfc2898DeriveBytes(password, salt, iterations);
            return keyGenerator.GetBytes(keySize);
        }

        // --------------- DECRYPTION --------------- //
        /// <summary>
        /// Decrypts the given data using AES decryption.
        /// </summary>
        /// <param name="bytes">The data to be decrypted as a byte array.</param>
        /// <param name="password">The password used for decryption.</param>
        /// <param name="bufferSize">The size of the buffer used for reading the data.</param>
        /// <returns>The decrypted data as a byte array.</returns>
        public static byte[] DefaultDecrypt(byte[] bytes, string password)
            => Decrypt(bytes, password, JEncryptionConfig.DefaultConfig);

        /// <summary>
        /// Decrypts the given data using AES decryption.
        /// </summary>
        /// <param name="bytes">The data to be decrypted as a byte array.</param>
        /// <param name="encryptionConfig"></param>
        /// <param name="password">The password used for decryption.</param>
        /// <param name="bufferSize">The size of the buffer used for reading the data.</param>
        /// <returns>The decrypted data as a byte array.</returns>
        public static byte[] Decrypt(byte[] bytes, string password, JEncryptionConfig encryptionConfig)
        {
            using var sourceStream    = new MemoryStream(bytes);
            using var decryptedStream = new MemoryStream();

            Decrypt_Impl(sourceStream, decryptedStream, password, encryptionConfig);
            return decryptedStream.ToArray();
        }

        /// <summary>
        /// Decrypts the given data using AES decryption.
        /// </summary>
        private static void Decrypt_Impl(Stream sourceStream, Stream resultStream, string password, JEncryptionConfig encryptionConfig)
        {
            using Aes              aesAlgorithm = InitializeAlgorithm(sourceStream, encryptionConfig, password);
            using ICryptoTransform decryptImplementation = aesAlgorithm.CreateDecryptor();
            using var              cryptoStream = new CryptoStream(sourceStream, decryptImplementation, CryptoStreamMode.Read);
            CopyStream(cryptoStream, resultStream, encryptionConfig.bufferSize);
        }

        /// <summary>
        /// Initializes the AES algorithm for encryption or decryption.
        /// </summary>
        /// <returns>The initialized AES algorithm.</returns>
        private static Aes InitializeAlgorithm(Stream sourceStream, JEncryptionConfig encryptionConfig, string password)
        {
            var    aesAlgorithm = Aes.Create();
            byte[] thisIv       = new byte[encryptionConfig.ivSize];
            int    read         = sourceStream.Read(thisIv, 0, encryptionConfig.ivSize);
            aesAlgorithm.IV = thisIv;

            var key = new Rfc2898DeriveBytes(password, aesAlgorithm.IV, encryptionConfig.iterations);
            aesAlgorithm.Key = key.GetBytes(encryptionConfig.keySize);

            return aesAlgorithm;
        }

        // --------------- UTILITIES --------------- //
        /// <summary>
        /// Copies the data from one stream to another.
        /// </summary>
        /// <param name="source">The input stream to read data from.</param>
        /// <param name="result">The output stream to write data to.</param>
        /// <param name="bufferSize">The size of the buffer used for reading and writing data.</param>
        private static void CopyStream(Stream source, Stream result, int bufferSize)
        {
            byte[] buffer        = new byte[bufferSize];
            int    readByteCount = source.Read(buffer, 0, bufferSize);
            while (readByteCount > 0)
            {
                result.Write(buffer, 0, readByteCount);
                readByteCount = source.Read(buffer, 0, bufferSize);
            }
        }
    }
}
