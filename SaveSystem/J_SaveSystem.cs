using System;
using System.IO;
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
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _fileExtension = ".dat";

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private string _filePath;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private string _lastFile;

        // --------------- SAVE --------------- //
        public void WriteToFile(string fileName, ArraySegment<byte> bytes)
        {
            Assert.IsNotNull(bytes.Array, $"Received null bytes");
            using (FileStream file = File.OpenWrite(fileName)) { file.Write(bytes.Array, bytes.Offset, bytes.Count); }
        }

        /// <summary>
        /// saves a serializable
        /// </summary>
        /// <param name="serializable"></param>
        /// <typeparam name="T"></typeparam>
        public virtual void SaveSerializable<T>(jSerializable<T> serializable)
            => SaveData<T>(serializable.ConvertToData(), serializable.NameOfThis);

        /// <summary>
        /// saves the data directly into a filename
        /// </summary>
        public virtual void SaveData<T>(T data, string fileName)
        {
            SetPath(fileName);
            Assert.IsFalse(string.IsNullOrEmpty(_filePath), $"{name} - {nameof(_filePath)} is not set");
            byte[] bytes = ConvertToBytes(data);
            WriteToFile(_filePath, bytes);
        }

        /// <summary>
        /// converts the data into bytes
        /// </summary>
        protected virtual byte[] ConvertToBytes<T>(T data) => SerializationUtility.SerializeValue(data, DataFormat.Binary);

        /// <summary>
        /// defines the strategy to write data
        /// </summary>
        protected virtual void WriteToFile(string path, byte[] bytes) => File.WriteAllBytes(path, bytes);

        // --------------- LOAD --------------- //
        /// <summary>
        /// commands to load the data from a serializable, it also send the command directly to the serializable
        /// </summary>
        public void LoadSerializable<T>(jSerializable<T> serializable)
        {
            LoadData(serializable.NameOfThis, out T data);
            serializable.LoadFrom(data);
        }

        /// <summary>
        /// loads the data directly out of data, given a file name 
        /// </summary>
        public bool LoadData<T>(string fileName, out T data)
        {
            SetPath(fileName);
            data = default;
            if (!File.Exists(_filePath)) return false;
            Assert.IsTrue(File.Exists(_filePath), $"{name} - no file at path {_filePath} for {fileName}");
            byte[] bytes = GetBytes(_filePath);
            data = ConvertToData<T>(bytes);
            return true;
        }

        /// <summary>
        /// get the bytes from the file, or other sources
        /// </summary>
        protected virtual byte[] GetBytes(string path) => File.ReadAllBytes(path);

        /// <summary>
        /// deserialize the bytes into the data
        /// </summary>
        protected virtual T ConvertToData<T>(byte[] bytes) => SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary);

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
                    _filePath = Application.persistentDataPath;
                    break;
                case PathType.Application:
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
