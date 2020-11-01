using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(PropertyModifier), true)]
    public class PropertyModifierInspector : Editor
    {
        protected SerializedProperty m_ApplyToAll;
        protected AnimBool m_ApplyToAllOptions;

        protected SerializedProperty m_Properties;
        protected ReorderableList m_PropertyList;

        protected SerializedProperty m_ModifierType;
        protected SerializedProperty m_Range;

        protected virtual void OnEnable()
        {
            this.m_ApplyToAll = serializedObject.FindProperty("m_ApplyToAll");
            this.m_ApplyToAllOptions = new AnimBool(!this.m_ApplyToAll.boolValue);
            this.m_ApplyToAllOptions.valueChanged.AddListener(new UnityAction(Repaint));

            this.m_Properties = serializedObject.FindProperty("m_Properties");
            this.m_PropertyList = new ReorderableList(serializedObject, this.m_Properties, true, true, true, true);
            this.m_PropertyList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Properties");
            };
            this.m_PropertyList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                float verticalOffset = (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y = rect.y + verticalOffset;
                SerializedProperty element = this.m_PropertyList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };
            this.m_ModifierType = serializedObject.FindProperty("m_ModifierType");
            this.m_Range = serializedObject.FindProperty("m_Range");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_ModifierType);
            EditorGUILayout.PropertyField(this.m_Range);

            EditorGUILayout.PropertyField(this.m_ApplyToAll);
            this.m_ApplyToAllOptions.target = !this.m_ApplyToAll.boolValue;
            if (EditorGUILayout.BeginFadeGroup(this.m_ApplyToAllOptions.faded))
            {
                this.m_PropertyList.DoLayoutList();
            }
            EditorGUILayout.EndFadeGroup();
     
            serializedObject.ApplyModifiedProperties();
        }

    }
}