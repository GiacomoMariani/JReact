using System;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace JReact.Tilemaps
{
    public readonly struct JTile : IEquatable<JTile>
    {
        private static readonly JTile _DefaultTile = new JTile();

        [Sirenix.OdinInspector.ReadOnly, ShowInInspector] public readonly Vector3Int cellPosition;
        [Sirenix.OdinInspector.ReadOnly, ShowInInspector] public readonly int id;
        [Sirenix.OdinInspector.ReadOnly, ShowInInspector] public readonly float moveMultiplier;
        [Sirenix.OdinInspector.ReadOnly, ShowInInspector] public readonly JCollisionFlag collisionFlag;
        
        public int2 Position => cellPosition.ToInt2();

        static JTile() { _DefaultTile = new JTile(default, default, default, default); }

        public JTile(Vector3Int cellPosition, int id, float moveMultiplier = 0, JCollisionFlag collisionFlag = JCollisionFlag.None)
        {
            this.cellPosition   = cellPosition;
            this.id             = id;
            this.moveMultiplier = moveMultiplier;
            this.collisionFlag  = collisionFlag;
        }

        public JTile(Vector3Int cellPosition, J_TileInfo tileInfo)
        {
            this.cellPosition = cellPosition;
            id                = tileInfo.TileInfoId;
            moveMultiplier    = tileInfo.MoveMultiplier;
            collisionFlag     = tileInfo.CollisionFlag;
        }

        public bool IsDefault() => this.Equals(_DefaultTile);

        public Vector3 ToWorldPosition(J_Mono_MapGrid mapGrid) => mapGrid.GetWorldPosition(this);

        private int ConvertToIndex(int2 adjustments, int width)
        {
            int x = cellPosition.x + adjustments.x;
            int y = cellPosition.y + adjustments.y;
            return (y * width) + x;
        }

        public int ConvertToIndex(J_Mono_MainTileBoard board) => ConvertToIndex(board.StartPoint.ToInt2(), board.Width);

        public override bool Equals(object obj) { return obj is JTile other && Equals(other); }

        public override int GetHashCode() => HashCode.Combine(cellPosition, id);

        public static bool operator ==(JTile left, JTile right) => left.Equals(right);
        public static bool operator !=(JTile left, JTile right) => !left.Equals(right);

        public          bool   Equals(JTile other) => cellPosition.Equals(other.cellPosition) && id == other.id;
        public override string ToString()          => $"{cellPosition}_({id})";
        
        public static int HashNativeArray(in NativeArray<JTile> array)
        {
            unchecked // Allow arithmetic overflow (wrap around)
            {
                int hash = 17;
                foreach (JTile value in array) { hash = hash * 31 + value.id; }
                return hash;
            }
        }
    }
}
