using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [CustomEditor(typeof(StatsHandler))]
    public class StatsHandlerInspector : Editor
    {
        protected SerializedProperty m_Script;
        protected SerializedProperty m_Stats;
        protected ReorderableList m_StatList;
        protected SerializedProperty m_Effects;
        protected ReorderableList m_EffectsList;

        protected SerializedProperty m_StatOverrides;

        protected virtual void OnEnable() {
            if (target == null) return;
            this.m_Script = serializedObject.FindProperty("m_Script");
            this.m_Stats = serializedObject.FindProperty("m_Stats");
            this.m_StatList = CreateList("Stats", serializedObject, this.m_Stats);
            this.m_Effects = serializedObject.FindProperty("m_Effects");
            this.m_EffectsList = CreateList("Effects", serializedObject, this.m_Effects);
            this.m_StatOverrides = serializedObject.FindProperty("m_StatOverrides");

            int selectedStatIndex = EditorPrefs.GetInt("SelectedStatIndex." + target.GetInstanceID(), -1);
            this.m_StatList.index = selectedStatIndex;
        }


        protected virtual void OnDisable() {
            if (target == null) return;
            EditorPrefs.SetInt("SelectedStatIndex." + target.GetInstanceID(),this.m_StatList.index) ;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(this.m_Script);
            EditorGUI.EndDisabledGroup();

            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject,this.m_Script.propertyPath,this.m_Stats.propertyPath,this.m_Effects.propertyPath);

            this.m_StatList.DoLayoutList();

            if (this.m_StatOverrides.arraySize < this.m_Stats.arraySize)
            {
                for (int i = this.m_StatOverrides.arraySize; i < this.m_Stats.arraySize; i++)
                {
                    this.m_StatOverrides.InsertArrayElementAtIndex(i);
                }
            }

            int selectedStatIndex = this.m_StatList.index;
            if (selectedStatIndex > -1 && this.m_Stats.arraySize > 0)
            {
                SerializedProperty statOverride = this.m_StatOverrides.GetArrayElementAtIndex(selectedStatIndex);
                SerializedProperty overrideBaseValue = statOverride.FindPropertyRelative("overrideBaseValue");
                SerializedProperty baseValue = statOverride.FindPropertyRelative("baseValue");
              
                EditorGUILayout.PropertyField(overrideBaseValue);
                if (overrideBaseValue.boolValue)
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUILayout.PropertyField(baseValue);
                    EditorGUI.indentLevel -= 1;
                }
               

                /*SerializedProperty stat = this.m_Stats.GetArrayElementAtIndex(selectedStatIndex);
                if (stat.objectReferenceValue != null)
                {
                    SerializedObject statObj = new SerializedObject(stat.objectReferenceValue);
                    SerializedProperty inherit = statObj.FindProperty("m_InheritBaseValue");
                    SerializedProperty overrideBaseValue = statObj.FindProperty("m_OverrideBaseValue");
                    statObj.Update();
                    EditorGUILayout.PropertyField(inherit);
                    if (!inherit.boolValue)
                    {
                        EditorGUI.indentLevel += 1;
                        EditorGUILayout.PropertyField(overrideBaseValue);
                        EditorGUI.indentLevel -= 1;
                    }
                    statObj.ApplyModifiedProperties();
                }*/
            }


            EditorGUILayout.Space();
            this.m_EffectsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private ReorderableList CreateList(string title, SerializedObject serializedObject, SerializedProperty elements)
        {
            ReorderableList reorderableList = new ReorderableList(serializedObject, elements, true, true, true, true);
            reorderableList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, title);
            };

            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                float verticalOffset = (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y = rect.y + verticalOffset;
                SerializedProperty element = elements.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };

            reorderableList.onRemoveCallback = (ReorderableList list) =>
            {
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(list.index);
                if(element.propertyType == SerializedPropertyType.ObjectReference)
                    list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;

                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            };
            return reorderableList;
        }
    }
}