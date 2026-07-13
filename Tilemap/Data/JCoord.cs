using System;
using UnityEngine;

namespace JReact.Tilemaps
{
    /// <summary>
    /// A 2D integer grid coordinate. Replaces the use of <see cref="Vector3Int"/> for tile cells,
    /// which were always 2D (z was never read).
    /// </summary>
    public readonly struct JCoord : IEquatable<JCoord>
    {
        public readonly int X;
        public readonly int Y;

        public JCoord(int x, int y)
        {
            X = x;
            Y = y;
        }

        // --------------- ORTHOGONAL ADJACENCY (default) --------------- //
        public JCoord Right => new JCoord(X + 1, Y);
        public JCoord Left  => new JCoord(X - 1, Y);
        public JCoord Up    => new JCoord(X, Y + 1);
        public JCoord Down  => new JCoord(X, Y - 1);

        // --------------- DIAGONAL ADJACENCY (opt-in) --------------- //
        public JCoord UpRight   => new JCoord(X + 1, Y + 1);
        public JCoord UpLeft    => new JCoord(X - 1, Y + 1);
        public JCoord DownRight => new JCoord(X + 1, Y - 1);
        public JCoord DownLeft  => new JCoord(X - 1, Y - 1);

        // --------------- DISTANCE --------------- //
        /// <summary>4-neighbour (taxicab) distance.</summary>
        public int ManhattanDistanceTo(JCoord other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y);

        /// <summary>8-neighbour (chessboard) distance.</summary>
        public int ChebyshevDistanceTo(JCoord other) => Math.Max(Math.Abs(X - other.X), Math.Abs(Y - other.Y));

        // --------------- VECTOR2INT INTEROP --------------- //
        public Vector2Int ToVector2Int() => new Vector2Int(X, Y);
        public static JCoord FromVector2Int(Vector2Int v) => new JCoord(v.x, v.y);

        // --------------- VECTOR3INT INTEROP (Unity Tilemap/Grid APIs require Vector3Int; z is always 0) --------------- //
        public Vector3Int ToVector3Int() => new Vector3Int(X, Y, 0);
        public static JCoord FromVector3Int(Vector3Int v) => new JCoord(v.x, v.y);

        // --------------- EQUALITY --------------- //
        public bool Equals(JCoord other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is JCoord other && Equals(other);
        public override int GetHashCode() => unchecked((X * 397) ^ Y);

        public static bool operator ==(JCoord a, JCoord b) => a.Equals(b);
        public static bool operator !=(JCoord a, JCoord b) => !a.Equals(b);

        // --------------- DEBUG (preserves Vector3Int's readable ToString) --------------- //
        public override string ToString() => $"({X}, {Y})";
    }
}
