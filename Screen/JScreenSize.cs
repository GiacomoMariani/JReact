using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.JScreen
{
    [Serializable]
    public struct JScreenSize : IEquatable<JScreenSize>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _width;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _height;

        public int Width => _width;
        public int Height => _height;

        public Resolution ToResolution() => new Resolution { width = _width, height = _height };

        public bool Equals(JScreenSize other) => _width == other._width && _height == other._height;

        public override bool Equals(object obj) => obj is JScreenSize other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(_width, _height);

        public override string ToString() => $"{Width}x{Height}";
    }
}
