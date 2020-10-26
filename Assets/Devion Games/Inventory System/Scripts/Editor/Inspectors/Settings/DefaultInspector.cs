using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace DevionGames.InventorySystem.Configuration
{
    [CustomEditor(typeof(Default))]
    public class DefaultInspector : Editor
    {
        private SerializedProperty m_ShowAllComponents;

        private void OnEnable()
        {
            this.m_ShowAllComponents = serializedObject.FindProperty("showAllComponents");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, this.m_ShowAllComponents.propertyPath);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_ShowAllComponents);
            if (EditorGUI.EndChangeCheck()) {
                EditorPrefs.SetBool("InventorySystem.showAllComponents", this.m_ShowAllComponents.boolValue);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
