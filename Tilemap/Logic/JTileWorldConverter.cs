using JMath2D.JPhysics;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace JReact.Tilemaps.Logic
{
    public readonly struct JTileWorldConverter
    {
        private readonly int gridWidth;
        private readonly int gridHeight;
        private readonly float2 origin;
        private readonly float2 cellSize;
        private readonly float2 invertedCellSize;

        public JTileWorldConverter(int gridWidth, int gridHeight, float2 origin, float2 cellSize)
        {
            this.gridWidth   = gridWidth;
            this.gridHeight  = gridHeight;
            this.origin      = origin;
            this.cellSize    = cellSize;
            invertedCellSize = 1f / cellSize;
        }

        public JTile GetTile(float2 position, NativeArray<JTile>.ReadOnly tiles)
        {
            float2 normalizedPosition = position - origin;
            int2   cellPosition       = (int2)(normalizedPosition * invertedCellSize);
            int    index              = cellPosition.x + cellPosition.y * gridWidth;
            return tiles[index];
        }

        public NativeList<JAabbBox2D> GetNeighbourCollisions(float2         position, NativeArray<JTile>.ReadOnly tiles,
                                                            JCollisionFlag collisionMask,
                                                            Allocator      allocator)
        {
            var result = new NativeList<JAabbBox2D>(8, allocator);

            JTile      originTile         = GetTile(position, tiles);
            Vector3Int originTilePosition = originTile.cellPosition;

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    //ignore same tile
                    if (x == 0 &&
                        y == 0) { continue; }

                    int2 cellPosition = new int2(originTilePosition.x + x, originTilePosition.y + y);
                    if (IsOutOfBorders(cellPosition))
                    {
                        result.Add(JAabbBox2D.FromTile(cellPosition, cellSize));
                        continue;
                    }

                    if (NeighbourHasCollisions(tiles, collisionMask, cellPosition))
                    {
                        result.Add(JAabbBox2D.FromTile(cellPosition, cellSize));
                    }
                }
            }

            return result;
        }

        private bool NeighbourHasCollisions(NativeArray<JTile>.ReadOnly tiles, JCollisionFlag collisionMask, int2 cellPosition)
        {
            int   index     = cellPosition.x + cellPosition.y * gridWidth;
            JTile neighbour = tiles[index];
            return CollisionMaskCheck(neighbour, collisionMask);
        }

        private bool IsOutOfBorders(int2 cellPosition) => cellPosition.x < 0          ||
                                                          cellPosition.x >= gridWidth ||
                                                          cellPosition.y < 0          ||
                                                          cellPosition.y >= gridHeight;

        private bool CollisionMaskCheck(JTile neighbour, JCollisionFlag collisionMask)
            => collisionMask.HasCollisionWith(neighbour.collisionFlag);

        public override string ToString() => $"GridWidth: {gridWidth}, Origin: {origin}, CellSize: {cellSize}";
    }
}
