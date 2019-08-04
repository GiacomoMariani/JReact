#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;

namespace JReact
{
    public class ScriptableObjectButtonProcessor<T> : OdinPropertyProcessor<T>
        where T : ScriptableObject
    {
        UnityEngine.Object[] _selection = new UnityEngine.Object[1];
        public override void ProcessMemberProperties(List<InspectorPropertyInfo> propertyInfos)
        {
            propertyInfos.AddDelegate(name: "Select SO",
                                      @delegate: () =>
                                      {
                                          // This code runs when the button is clicked
                                          var so = this.Property.ValueEntry.WeakSmartValue as T;
                                          EditorApplication.ExecuteMenuItem("Window/General/Project");

                                          // --------------- SELECTION --------------- //
                                          _selection[0] = so;
                                          UnityEditor.Selection.objects = _selection;
                                          UnityEditor.Selection.SetActiveObjectWithContext(so, so);

                                          //set the new active object and make sure the project window is open
                                          UnityEditor.Selection.activeObject = _selection[0];
                                          EditorGUIUtility.PingObject(so);
                                          Debug.Log("Selected " + so.name + "!");
                                      },
                                      order: int.MinValue, // Place on top
                                      
                                      attributes: new Attribute[] { new ButtonAttribute(ButtonSizes.Medium), new BoxGroupAttribute("Editor", true, true, -1000),  });
        }
    }
}
#endif
