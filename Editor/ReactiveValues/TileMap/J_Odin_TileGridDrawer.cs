#if UNITY_EDITOR
using System.Collections;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace JReact.Tilemaps.J_Editor
{
    //
    // In Character.cs we have a two dimention array of ItemSlots which is our inventory.
    // And instead of using the the TableMatrix attribute to customize it there, we in this case
    // instead create a custom drawer that will work for all two-dimentional ItemSlot arrays,
    // so we don't have to make the same CustomDrawer via the TableMatrix attribute again and again.
    //

    internal class J_Odin_TileGridDrawer<TArray, T> : TwoDimensionalArrayDrawer<TArray, T>
        where TArray : IList
        where T : J_Tile
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private T _selectedTile;
        private Rect _selectedRect;

        // --------------- DRAW ELEMENT --------------- //
        protected override T DrawElement(Rect rect, T value)
        {
            //DRAWING
            Color color = (value == null || value.Ground == null) ? Color.black : value.Ground.TileColor;
            SirenixEditorGUI.DrawSolidRect(rect, color);
            int borders = (value                      != null &&
                           _selectedTile              != null &&
                           _selectedTile.CellPosition == value.CellPosition)
                              ? 3
                              : 1;

            SirenixEditorGUI.DrawBorders(rect, borders);

            // --------------- SELECTION --------------- //
            Vector2   mousePosition = Event.current.mousePosition;
            EventType current       = Event.current.type;
            if (current == EventType.MouseDown &&
                rect.Contains(mousePosition)) Select(rect, value);

            return value;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            base.DrawPropertyLayout(label);

            SirenixEditorGUI.BeginBox();
            Rect rect = GUILayoutUtility.GetRect(0, _selectedTile == null ? 35 : 150).Padding(2);
            EditorGUI.LabelField(rect, _selectedTile == null ? $"You may select a cell from above.\nCannot select null cell." : $"Cell - {_selectedTile}");
            SirenixEditorGUI.EndBox();
        }

        // --------------- SELECTION --------------- //
        private void Select(Rect rect, T value) => (_selectedRect, _selectedTile) = (rect, value);

        private void Deselect() => (_selectedRect, _selectedTile) = (default, null);
    }
}
#endif
