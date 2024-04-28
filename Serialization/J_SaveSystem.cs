using System;
using System.IO;
using System.IO.Compression;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.SaveSystem
{
    public class J_SaveSystem : ScriptableObject
    {
        // --------------- ENUM AND EVENTS --------------- //
        private enum PathType : byte { Persistent = 0, Application = 10, Custom = 100 }

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private PathType _pathType;
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _customPath;
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _fileExtension = ".bin";

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private string _filePath;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private string _lastFile;

        // --------------- SAVE --------------- //
        /// <summary>
        /// defines the strategy to write data
        /// </summary>
        protected virtual void WriteToFile(string path, byte[] bytes, int fileSize, int offset = 0)
        {
            Assert.IsNotNull(bytes, $"Received null bytes");

            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, fileSize);
            stream.Write(bytes, offset, fileSize);
        }

        /// <summary>
        /// defines the strategy to write data
        /// </summary>
        protected virtual void WriteToFileCompressed(string path, byte[] bytes, int fileSize, int offset = 0)
        {
            Assert.IsNotNull(bytes, $"Received null bytes");

            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, fileSize);

            using var compressedStream = new DeflateStream(stream, CompressionMode.Compress);
            compressedStream.Write(bytes, 0, fileSize);
        }

        // --------------- LOAD --------------- //
        /// <summary>
        /// get the bytes from the file, or other sources
        /// </summary>
        protected virtual byte[] ReadFromFile(string path) => File.ReadAllBytes(path);

        /// <summary>
        /// gets the bytes from a compressed file
        /// </summary>
        /// <param name="path">the path where to take the bytes</param>
        /// <returns>returns the decompressed bytes</returns>
        protected virtual byte[] ReadFromFileCompressed(string path)
        {
            using FileStream file               = File.Open(path, FileMode.Open);
            using var        memoryStream       = new MemoryStream();
            using var        decompressedStream = new DeflateStream(file, CompressionMode.Decompress);

            decompressedStream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        // --------------- PATH CALCULATION --------------- //
        /// <summary>
        /// calculates the path where to save the file
        /// </summary>
        /// <param name="fileName">the file we want to set</param>
        /// <returns>returns the full path</returns>
        private void SetPath(string fileName)
        {
            if (_lastFile == fileName) return;
            switch (_pathType)
            {
                case PathType.Persistent:
                    //where you can store data that you want to be kept between runs
                    //https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
                    _filePath = Application.persistentDataPath;
                    break;
                case PathType.Application:
                    //Contains the path to the game data folder on the target device (Read Only).
                    //https://docs.unity3d.com/ScriptReference/Application-dataPath.html
                    _filePath = Application.dataPath;
                    break;
                case PathType.Custom:
                    _filePath = _customPath;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            _filePath += fileName + _fileExtension;
            _lastFile =  fileName;
        }
    }
}
