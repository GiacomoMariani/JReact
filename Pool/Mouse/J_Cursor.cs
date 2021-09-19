using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Pool.SpecialEffect
{
    [Serializable]
    public class CursorData
    {
        [BoxGroup("Cursor", true, true), SerializeField, AssetsOnly, Required] public Texture2D Texture;
        [BoxGroup("Cursor", true, true), SerializeField] public Vector2 Offset;
        [BoxGroup("Cursor", true, true), SerializeField] public CursorMode Mode = CursorMode.Auto;
    }

    /// <summary>
    /// this class is used to change the mouse pointer
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Special Effects/Cursor")]
    public class J_Cursor : ScriptableObject
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup - Cursor", true, true), SerializeField, AssetsOnly, Required]
        private CursorData _defaultPointer;

        [FoldoutGroup("State", false, 15), ReadOnly, ShowInInspector] public CursorData CurrentCursor { get; private set; }

        // --------------- COMMANDS - CURSOR --------------- //
        /// <summary>
        /// enables a custom cursor
        /// </summary>
        public void SetCursor(CursorData cursorData)
        {
            CurrentCursor = cursorData;
            Cursor.SetCursor(cursorData.Texture, cursorData.Offset, cursorData.Mode);
            Cursor.visible = true;
        }

        /// <summary>
        /// hide the cursor
        /// </summary>
        public void HideCursor() { Cursor.visible = false; }

        /// <summary>
        /// hide the cursor
        /// </summary>
        public void ShowCursor() { Cursor.visible = true; }

        /// <summary>
        /// resets the cursor to default
        /// </summary>
        public void SetDefaultCursor() { SetCursor(_defaultPointer); }
    }
}
