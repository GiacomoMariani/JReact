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

        public NativeList<JTileAABB> GetNeighbourCollisions(float2    position, NativeArray<JTile>.ReadOnly tiles, int collisionMask,
                                                            Allocator allocator)
        {
            var result = new NativeList<JTileAABB>(8, allocator);

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
                        result.Add(new JTileAABB(cellPosition, cellSize));
                        continue;
                    }

                    if (NeighbourHasCollisions(tiles, collisionMask, cellPosition))
                    {
                        result.Add(new JTileAABB(cellPosition, cellSize));
                    }
                }
            }

            return result;
        }

        private bool NeighbourHasCollisions(NativeArray<JTile>.ReadOnly tiles, int collisionMask, int2 cellPosition)
        {
            int   index     = cellPosition.x + cellPosition.y * gridWidth;
            JTile neighbour = tiles[index];
            return CollisionMaskCheck(neighbour, collisionMask);
        }

        private bool IsOutOfBorders(int2 cellPosition) => cellPosition.x < 0          ||
                                                          cellPosition.x >= gridWidth ||
                                                          cellPosition.y < 0          ||
                                                          cellPosition.y >= gridHeight;

        //todo implement collision mask
        private bool CollisionMaskCheck(JTile neighbour, int collisionMask) { return neighbour.id > 10; }

        public override string ToString() => $"GridWidth: {gridWidth}, Origin: {origin}, CellSize: {cellSize}";
    }

    public readonly struct JTileAABB
    {
        public readonly float xMin;
        public readonly float xMax;
        public readonly float yMin;
        public readonly float yMax;

        public JTileAABB(int2 tilePosition, float2 cellSize)
        {
            float halfWidth  = cellSize.x * 0.5f;
            float halfHeight = cellSize.y * 0.5f;

            xMin = tilePosition.x * cellSize.x - halfWidth;
            xMax = xMin                        + cellSize.x;
            yMin = tilePosition.y * cellSize.y - halfHeight;
            yMax = yMin                        + cellSize.y;
        }

        public override string ToString() => $"XRange: ({xMin}, {xMax}), YRange: ({yMin}, {yMax})";
    }
}
