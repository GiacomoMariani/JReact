using System;
using System.IO;
using System.IO.Compression;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.SaveSystem
{
    [CreateAssetMenu(menuName = "Reactive/Serialize/File Operator", fileName = "J_SO_FileOperator", order = 0)]
    public class J_SO_FileOperator : ScriptableObject
    {
        // --------------- ENUM AND EVENTS --------------- //
        private enum PathType : byte { Persistent = 0, Application = 10, Custom = 100 }

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private PathType _pathType;
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _fileExtension = ".bin";

        [ShowIf(nameof(IsCustomPath)), BoxGroup("Setup", true, true, 0), SerializeField]
        private string _customPath;

        // --------------- STATE --------------- //
        public bool IsCustomPath => _pathType == PathType.Custom;

        // --------------- SAVE --------------- //
        /// <summary>
        /// defines the strategy to write data
        /// </summary>
        public  void WriteToFile(string fileName, byte[] bytes, int fileSize, int offset = 0)
        {
            Assert.IsNotNull(bytes, $"Received null bytes");
            string path = GetPath(fileName);

            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, fileSize);
            stream.Write(bytes, offset, fileSize);
        }

        /// <summary>
        /// defines the strategy to write data
        /// </summary>
        public  void WriteToFileCompressed(string fileName, byte[] bytes, int fileSize, int offset = 0)
        {
            Assert.IsNotNull(bytes, $"Received null bytes");

            string    path   = GetPath(fileName);
            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, fileSize);

            using var compressedStream = new DeflateStream(stream, CompressionMode.Compress);
            compressedStream.Write(bytes, 0, fileSize);
        }

        // --------------- LOAD --------------- //
        /// <summary>
        /// get the bytes from the file, or other sources
        /// </summary>
        public  byte[] ReadFromFile(string fileName) => File.ReadAllBytes(GetPath(fileName));

        /// <summary>
        /// gets the bytes from a compressed file
        /// </summary>
        public  byte[] ReadFromFileCompressed(string fileName)
        {
            string path = GetPath(fileName);
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
        public string GetPath(string fileName)
        {
            string filePath = "";
            switch (_pathType)
            {
                case PathType.Persistent:
                    //where you can store data that you want to be kept between runs
                    //https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
                    filePath = Application.persistentDataPath; break;
                case PathType.Application:
                    //Contains the path to the game data folder on the target device (Read Only).
                    //https://docs.unity3d.com/ScriptReference/Application-dataPath.html
                    filePath = Application.dataPath; break;
                case PathType.Custom: filePath = _customPath; break;
                default:              throw new ArgumentOutOfRangeException();
            }

            filePath += fileName + _fileExtension;
            return filePath;
        }

        public long GetFileLength(string fileName)
        {
            FileInfo fileInfo = GetFileInfo(fileName);
            return fileInfo.Exists ? fileInfo.Length : -1;
        }
        
        public FileInfo GetFileInfo(string fileName)
        {
            string filePath = GetPath(fileName);
            return new FileInfo(filePath);
        }
    }
}
