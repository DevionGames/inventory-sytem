using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;
using UnityEditorInternal;
using System.Linq;


namespace DevionGames
{
    [CustomEditor(typeof(BehaviorTrigger), true)]
    public class BehaviorTriggerInspector : BaseTriggerInspector
    {
        private void DrawInspector()
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            if (EditorTools.RightArrowButton(new GUIContent("Edit Behavior", "Trigger use behavior"), GUILayout.Height(20f)))
            {
                SerializedProperty actionList = serializedObject.FindProperty("actions");
                
                ObjectWindow.ShowWindow("Edit Behavior",serializedObject, actionList);
            }
            EditorGUI.EndDisabledGroup();
        }


    }
}
