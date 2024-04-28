#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Sirenix.Utilities;

namespace JReact.OdinDrawers
{
    public class JO_ReactiveBoolDrawer : OdinValueDrawer<J_ReactiveBool>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            //get the value and draw the element
            J_ReactiveBool value = ValueEntry.SmartValue;

            //just shows the empty field if the SO is missing
            if (value == null)
            {
                CallNextDrawer(label);
                return;
            }

            Rect rect = EditorGUILayout.GetControlRect();
            //NAME
            GUI.Label(rect.AlignLeft(100f), label);
            //VALUE
            value.Current = EditorGUI.Toggle(rect.AlignCenter(20f), value.Current);
            //REFERENCE
            ValueEntry.SmartValue =
                (J_ReactiveBool) SirenixEditorFields.UnityObjectField(rect.AlignRight(100f), value, typeof(J_ReactiveBool), false);
        }
    }

    public class JO_ReactiveIntDrawer : OdinValueDrawer<J_ReactiveInt>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            //get the value and draw the element
            J_ReactiveInt value = ValueEntry.SmartValue;

            //just shows the empty field if the SO is missing
            if (value == null)
            {
                CallNextDrawer(label);
                return;
            }

            Rect rect = EditorGUILayout.GetControlRect();
            //NAME
            GUI.Label(rect.AlignLeft(100f), label);
            //VALUE
            value.Current = SirenixEditorFields.IntField(rect.AlignCenter(200f), value.Current);
            //REFERENCE
            ValueEntry.SmartValue =
                (J_ReactiveInt) SirenixEditorFields.UnityObjectField(rect.AlignRight(100f), value, typeof(J_ReactiveInt), false);
        }
    }

    public class JO_ReactiveFloatDrawer : OdinValueDrawer<J_ReactiveFloat>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            //get the value and draw the element
            J_ReactiveFloat value = ValueEntry.SmartValue;

            //just shows the empty field if the SO is missing
            if (value == null)
            {
                CallNextDrawer(label);
                return;
            }

            Rect rect = EditorGUILayout.GetControlRect();
            //NAME
            GUI.Label(rect.AlignLeft(100f), label);
            //VALUE
            value.Current = SirenixEditorFields.FloatField(rect.AlignCenter(200f), value.Current);
            //REFERENCE
            ValueEntry.SmartValue =
                (J_ReactiveFloat) SirenixEditorFields.UnityObjectField(rect.AlignRight(100f), value, typeof(J_ReactiveFloat), false);
        }
    }

    public class JO_ReactiveStringDrawer : OdinValueDrawer<J_ReactiveString>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            //get the value and draw the element
            J_ReactiveString value = ValueEntry.SmartValue;

            //just shows the empty field if the SO is missing
            if (value == null)
            {
                CallNextDrawer(label);
                return;
            }

            Rect rect = EditorGUILayout.GetControlRect();
            //NAME
            GUI.Label(rect.AlignLeft(100f), label);

            //REFERENCE
            ValueEntry.SmartValue =
                (J_ReactiveString) SirenixEditorFields.UnityObjectField(rect.AlignRight(100f), value, typeof(J_ReactiveString), false);

            rect = EditorGUILayout.GetControlRect(false, 50f);
            //VALUE
            value.Current = SirenixEditorFields.TextField(rect, value.Current);
        }
    }
}
#endif
