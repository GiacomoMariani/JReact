using System;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using UnityEngine;
using Unity.Mathematics;

namespace JReact.UiViewMono
{
    [Serializable]
    public readonly struct J_GridData
    {
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public readonly float2 cellSize;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public readonly float2 margins;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public readonly float2 sizeWithMargins;

        /// <summary>
        /// padding is calculated from left and then anti clockwise like so:
        /// x = left border
        /// y = bottom bnorder
        /// z = right border
        /// w = top border
        /// </summary>
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public readonly float4 paddings;

        /// <summary>
        /// creates a grid fully customized
        /// </summary>
        /// <param name="cellSize">the cell size</param>
        /// <param name="margins">the margins are set  horizontally (x) vertically (y)</param>
        /// <param name="paddings">paddings in all directions, from left, anticlockwise. Left(x), bottom(y), right(z), top(w)(</param>
        public J_GridData(float2 cellSize, float2 margins, float4 paddings)
        {
            ValidateSize(cellSize);
            this.cellSize        = cellSize;
            this.margins         = margins;
            this.paddings        = paddings;
            this.sizeWithMargins = new float2(cellSize.x + margins.x, cellSize.y + margins.y);
        }

        private static void ValidateSize(float2 cellSize)
        {
            Assert.IsTrue(cellSize.x > 0, $"{cellSize.x} only positive sizes allowed - horizontal");
            Assert.IsTrue(cellSize.y > 0, $"{cellSize.y} only vertical sizes allowed - horizontal");
        }

        /// <summary>
        /// creates a grid with equal margin and padding
        /// </summary>
        /// <param name="cellSize">the cell base size</param>
        /// <param name="margin">the single margin, equal both for vertical and horizonatl</param>
        /// <param name="padding">the padding of the grid, equal in all directions</param>
        public J_GridData(float2 cellSize, float margin, float padding = 0)
        {
            ValidateSize(cellSize);
            this.cellSize        = cellSize;
            this.margins         = new float2(margin, margin);
            this.paddings        = new float4(padding, padding, padding, padding);
            this.sizeWithMargins = new float2(cellSize.x + margins.x, cellSize.y + margins.y);
        }

        /// <summary>
        /// creates a grid with equal margin and different padding for vertical and horizontal
        /// </summary>
        /// <param name="cellSize">the cell base size</param>
        /// <param name="margin">the single margin, equal both for vertical and horizontal</param>
        /// <param name="paddings">the padding, x is for horizontal, y is for vertical</param>
        public J_GridData(float2 cellSize, float margin, float2 paddings)
        {
            ValidateSize(cellSize);
            this.cellSize        = cellSize;
            this.margins         = new float2(margin, margin);
            this.paddings        = new float4(paddings.x, paddings.y, paddings.x, paddings.y);
            this.sizeWithMargins = new float2(cellSize.x + margins.x, cellSize.y + margins.y);
        }

        /// <summary>
        /// calculates the size of an amount of cells, it draws them without the padding
        /// </summary>
        /// <param name="cellsAmount">the amount of cells to calculate</param>
        /// <returns>returns the size of the cells</returns>
        public readonly float2 GetMultiCellsSize(int2 cellsAmount)
        {
            var totalSize = new float2(cellsAmount.x * sizeWithMargins.x,
                                       cellsAmount.y * sizeWithMargins.y);

            // --------------- REMOVING ONE MARGIN --------------- //
            // |CELL| MARGIN |CELL|
            // NOT: // |CELL| MARGIN |CELL| MARGIN
            totalSize.x -= margins.x;
            totalSize.y -= margins.y;
            return totalSize;
        }

        /// <summary>
        /// returns the full grid size, based on the amount of cells. Full grid size contains also the padding
        /// </summary>
        /// <param name="cellsAmount">the cells representing the grid</param>
        /// <returns>returns the grid size with the given padding</returns>
        public readonly float2 GetFullGridSize(int2 cellsAmount)
        {
            float2 totalSize = GetMultiCellsSize(cellsAmount);

            //margins might be different on all directions
            totalSize.x += paddings.x + paddings.z;
            totalSize.y += paddings.y + paddings.w;
            return totalSize;
        }

        /// <summary>
        /// calculate the origin point of a given cell coordinates, origin point on bottom left
        /// </summary>
        /// <param name="coordinates">the coordinates we want to check</param>
        /// <returns>returns the position of the requested coordinates, origin point on bottom left</returns>
        public readonly float2 GetOriginPosition(int2 coordinates)
        {
            float2 position = GetMultiCellsSize(coordinates);

            //to get a position we calculate only bottom and left padding
            //we assume always to draw from bottom left
            position.x += paddings.x;
            position.y += paddings.y;
            return position;
        }
    }

    public static class J_GridExtensions
    {
        /// <summary>
        /// place an item on a specific slot of the grid
        /// </summary>
        /// <param name="rectTransform">the rect transform we want to place</param>
        /// <param name="gridData">the grid data</param>
        /// <param name="coordinates">the coordinates where to place the rect transform</param>
        public static RectTransform PlaceOnGrid(this RectTransform rectTransform, J_GridData gridData, int2 coordinates)
        {
            rectTransform.anchoredPosition = gridData.GetOriginPosition(coordinates);
            return rectTransform;
        }

        /// <summary>
        /// draws the rect on all grids, removed the margin
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="gridData"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static RectTransform FillCells(this RectTransform rectTransform, J_GridData gridData, int2 position,
                                              int2               size)
        {
            //placing on the grid means using all the cells
            rectTransform.PlaceOnGrid(gridData, position)
                         .DrawCells(size, gridData);

            return rectTransform;
        }

        /// <summary>
        /// set the size of a rect transform based on a grid
        /// </summary>
        /// <param name="rectTransform">the rect transform that receives the change</param>
        /// <param name="cellsAmount">the amounts of cells occupied by this</param>
        /// <param name="gridData">the grid data</param>
        private static RectTransform DrawFullGrid(this RectTransform rectTransform, int2 cellsAmount, J_GridData gridData)
        {
            rectTransform.sizeDelta = gridData.GetFullGridSize(cellsAmount);
            return rectTransform;
        }

        /// <summary>
        /// drawing the cells is the same as drawing the grid, with no padding
        /// </summary>
        /// <param name="rectTransform">the rect transform that receives the change</param>
        /// <param name="cellsAmount">the amounts of cells occupied by this</param>
        /// <param name="gridData">the grid where to draw the cells</param>
        private static RectTransform DrawCells(this RectTransform rectTransform, int2 cellsAmount, J_GridData gridData)
        {
            rectTransform.sizeDelta = gridData.GetMultiCellsSize(cellsAmount);
            return rectTransform;
        }
    }
}
