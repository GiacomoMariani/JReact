#if UNITY_DOTS
using JMath2D.JPhysics;
using Unity.Collections;
using Unity.Mathematics;

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

        public float2 GetWorldCenter(JCoord coord)
            => origin + (new float2(coord.X, coord.Y) + 0.5f) * cellSize;

        public NativeList<JAabbBox2D> GetNeighbourCollisions(float2        position, NativeArray<JTile>.ReadOnly tiles,
                                                            JCollisionFlag collisionMask,
                                                            Allocator      allocator)
        {
            var result = new NativeList<JAabbBox2D>(8, allocator);

            JTile       originTile         = GetTile(position, tiles);
            JCoord      originTilePosition = originTile.cellPosition;
            JGridBounds grid               = new JGridBounds(new JCoord(0, 0), new JCoord(gridWidth - 1, gridHeight - 1));

            // the 8 neighbours: the 3x3 box around the origin tile, minus the origin itself
            foreach (JCoord neighbour in new JGridBounds(originTilePosition.DownLeft, originTilePosition.UpRight))
            {
                if (neighbour == originTilePosition) { continue; }

                int2 cellPosition = new int2(neighbour.X, neighbour.Y);
                if (!grid.Contains(neighbour))
                {
                    result.Add(JAabbBox2D.FromTile(cellPosition, cellSize));
                    continue;
                }

                if (NeighbourHasCollisions(tiles, collisionMask, cellPosition))
                {
                    result.Add(JAabbBox2D.FromTile(cellPosition, cellSize));
                }
            }

            return result;
        }

        /// <summary>
        /// Simil-raycast on the grid: fills result with every tile the segment [from -> to] touches,
        /// in walk order. Supercover: steps one axis at a time (never a diagonal jump), so the walked
        /// path is 4-connected and a diagonal wall cannot be crossed unseen. Both endpoint tiles are
        /// included. Clears result first.
        /// Returns false if the segment leaves the grid (out of bounds = caller treats as blocked);
        /// result then holds the tiles visited up to the exit.
        /// </summary>
        public bool GetTilesOnSegment(float2 from, float2 to, NativeArray<JTile>.ReadOnly tiles, ref NativeList<JTile> result)
        {
            result.Clear();

            // positions in cell space: integer part = cell coordinates, fraction = position inside the cell
            float2 fromCell = (from - origin) * invertedCellSize;
            float2 toCell   = (to   - origin) * invertedCellSize;

            int2 cell    = (int2)math.floor(fromCell);
            int2 endCell = (int2)math.floor(toCell);

            if (!TryAddTile(cell, tiles, ref result)) { return false; }

            float2 direction = toCell - fromCell;
            int2   step      = new int2(direction.x > 0f ? 1 : -1, direction.y > 0f ? 1 : -1);

            // t per full cell on each axis, and t to the first boundary crossing; infinity when parallel
            float2 absDirection = math.abs(direction);
            float2 tDelta = math.select(1f / absDirection, new float2(float.PositiveInfinity), absDirection < 1e-8f);
            float2 firstBoundaryDistance = new float2(step.x > 0 ? (cell.x + 1) - fromCell.x : fromCell.x - cell.x,
                                                      step.y > 0 ? (cell.y + 1) - fromCell.y : fromCell.y - cell.y);

            float2 tMax = firstBoundaryDistance * tDelta;

            // one axis per iteration => exactly the manhattan distance in steps, always landing on endCell
            int totalSteps = math.abs(endCell.x - cell.x) + math.abs(endCell.y - cell.y);
            for (int i = 0; i < totalSteps; i++)
            {
                bool stepX;
                if      (cell.x == endCell.x) { stepX = false; }
                else if (cell.y == endCell.y) { stepX = true; }
                else                          { stepX = tMax.x <= tMax.y; }

                if (stepX) { cell.x += step.x; tMax.x += tDelta.x; }
                else       { cell.y += step.y; tMax.y += tDelta.y; }

                if (!TryAddTile(cell, tiles, ref result)) { return false; }
            }

            return true;
        }

        private bool TryAddTile(int2 cell, NativeArray<JTile>.ReadOnly tiles, ref NativeList<JTile> result)
        {
            if (cell.x < 0 ||
                cell.y < 0 ||
                cell.x >= gridWidth ||
                cell.y >= gridHeight) { return false; }

            result.Add(tiles[cell.x + cell.y * gridWidth]);
            return true;
        }

        private bool NeighbourHasCollisions(NativeArray<JTile>.ReadOnly tiles, JCollisionFlag collisionMask, int2 cellPosition)
        {
            int   index     = cellPosition.x + cellPosition.y * gridWidth;
            JTile neighbour = tiles[index];
            return CollisionMaskCheck(neighbour, collisionMask);
        }

        private bool CollisionMaskCheck(JTile neighbour, JCollisionFlag collisionMask)
            => collisionMask.HasCollisionWith(neighbour.collisionFlag);

        public override string ToString() => $"GridWidth: {gridWidth}, Origin: {origin}, CellSize: {cellSize}";
    }
}
#endif
