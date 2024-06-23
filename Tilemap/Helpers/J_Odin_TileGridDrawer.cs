#if UNITY_EDITOR
using System.Collections;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace JReact.Tilemaps.J_Editor
{
    internal class J_Odin_TileGridDrawer<TArray> : TwoDimensionalArrayDrawer<TArray, JTile>
        where TArray : IList
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        private JTile _selectedTile;
        private Rect _selectedRect;

        // --------------- DRAW ELEMENT --------------- //
        protected override JTile DrawElement(Rect rect, JTile value)
        {
            //DRAWING
            Color color = (value.IsDefault()) ? Color.black : GetColor(value);
            SirenixEditorGUI.DrawSolidRect(rect, color);
            int borders = (_selectedTile.cellPosition.x == value.cellPosition.x &&
                           _selectedTile.cellPosition.y == value.cellPosition.y)
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
            EditorGUI.LabelField(rect,
                                 _selectedTile == default
                                     ? $"You may select a cell from above.\nCannot select null cell."
                                     : $"Cell - {_selectedTile}");

            SirenixEditorGUI.EndBox();
        }

        // --------------- SELECTION --------------- //
        private void Select(Rect rect, JTile value) => (_selectedRect, _selectedTile) = (rect, value);

        private void Deselect() => (_selectedRect, _selectedTile) = (default, default);

        private Color GetColor(JTile tile) { return J_Mono_MainTileBoard.GetInstanceSafe().GetGroundTileInfo(tile).TileColor; }
    }
}
#endif
