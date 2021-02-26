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
        private SerializedProperty m_Script;

        private void OnEnable()
        {
            if (target == null) return;
            this.m_Script = serializedObject.FindProperty("m_Script");
            this.m_ShowAllComponents = serializedObject.FindProperty("showAllComponents");
        }

        public override void OnInspectorGUI()
        {
            if (target == null) return;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(this.m_Script);
            EditorGUI.EndDisabledGroup();

            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, this.m_ShowAllComponents.propertyPath, this.m_Script.propertyPath);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_ShowAllComponents);
            if (EditorGUI.EndChangeCheck()) {
                EditorPrefs.SetBool("InventorySystem.showAllComponents", this.m_ShowAllComponents.boolValue);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
