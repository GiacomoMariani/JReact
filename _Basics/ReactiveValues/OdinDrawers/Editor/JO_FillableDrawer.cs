#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Sirenix.Utilities;

namespace JReact.OdinDrawers
{
    /// <summary>
    /// draws a fillable on the inspector
    /// </summary>
    public class JO_FillableDrawer : OdinValueDrawer<J_Fillable>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            //get the value and draw the element
            J_Fillable value = ValueEntry.SmartValue;

            //just shows the empty field if the SO is missing
            if (value == null)
            {
                CallNextDrawer(label);
                return;
            }

            //BOX START
            SirenixEditorGUI.BeginBox();
            // --------------- LINE 1 - Reference to the SO --------------- //
            CallNextDrawer(label);

            // --------------- LINE 2 - LABELS AND CURRENT --------------- //
            Rect rect = EditorGUILayout.GetControlRect();
            GUI.Label(rect, "Min |", SirenixGUIStyles.LeftAlignedGreyMiniLabel);
            value.Current = SirenixEditorFields.IntField(rect.AlignCenterX(200), value.Current);
            GUI.Label(rect, "| Max", SirenixGUIStyles.RightAlignedGreyMiniLabel);

            // --------------- LINE 3 - INT FIELDS AND SLIDER--------------- //
            rect      = EditorGUILayout.GetControlRect();
            value.Min = SirenixEditorFields.IntField(rect.AlignLeft(80f), value.Min);
            value.Current =
                (int) SirenixEditorFields.SegmentedProgressBarField(rect.AlignCenterX(rect.width - 80f * 2 - 4), value.Current,
                                                                    value.Min, value.Max);

            value.Max = SirenixEditorFields.IntField(rect.AlignRight(80f), value.Max);

            // --------------- LINE 4 - ERROR IF SOMETHING IS NOT SET PROPERLY--------------- //
            if (!value.SanityChecks()) SirenixEditorGUI.ErrorMessageBox("Make sure that MIN <= CURRENT <= MAX");

            //BOX END
            SirenixEditorGUI.EndBox();

            GUIHelper.RequestRepaint();
        }
    }
}
#endif
