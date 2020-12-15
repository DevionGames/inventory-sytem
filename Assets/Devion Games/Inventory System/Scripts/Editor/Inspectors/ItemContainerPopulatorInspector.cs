using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(ItemContainerPopulator))]
    public class ItemContainerPopulatorInspector : Editor
    {
        private SerializedProperty m_Entries;
        private ReorderableList m_EntryList;

        private void OnEnable()
        {
            this.m_Entries = serializedObject.FindProperty("m_Entries");
            CreateEntryList();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.BeginVertical();
            serializedObject.Update();
            this.m_EntryList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
            GUILayout.Space(-4.5f);

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus"), EditorStyles.toolbarButton, GUILayout.Width(28f)))
            {
                AddEntry();
                EditorGUI.FocusTextInControl("");
            }

            EditorGUI.BeginDisabledGroup(this.m_EntryList.index == -1);
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus"), EditorStyles.toolbarButton, GUILayout.Width(25f)))
            {
                this.serializedObject.Update();
                this.m_Entries.DeleteArrayElementAtIndex(this.m_EntryList.index);
                this.serializedObject.ApplyModifiedProperties();
                this.m_EntryList.index = this.m_Entries.arraySize - 1;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        protected void CreateEntryList()
        {
            this.m_EntryList = new ReorderableList(serializedObject, this.m_Entries, true, false, false, false);
            this.m_EntryList.headerHeight = 0f;
            this.m_EntryList.footerHeight = 0f;
            this.m_EntryList.showDefaultBackground = false;
            float defaultHeight = this.m_EntryList.elementHeight;
            float verticalOffset = (defaultHeight - EditorGUIUtility.singleLineHeight) * 0.5f;

            this.m_EntryList.elementHeight = (defaultHeight + verticalOffset) * 2;
            this.m_EntryList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y = rect.y + verticalOffset;
                SerializedProperty element = this.m_Entries.GetArrayElementAtIndex(index);
                if (!EditorGUIUtility.wideMode)
                {
                    EditorGUIUtility.wideMode = true;
                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212;
                }
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("name"));
                rect.y = rect.y + verticalOffset + defaultHeight;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("group"), true);

            };
            this.m_EntryList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) => {

                if (Event.current.type == EventType.Repaint)
                {
                    GUIStyle style = new GUIStyle("AnimItemBackground");
                    style.Draw(rect, false, isActive, isActive, isFocused);

                    GUIStyle style2 = new GUIStyle("RL Element");
                    style2.Draw(rect, false, isActive, isActive, isFocused);
                }
            };
        }

        private void AddEntry()
        {
            serializedObject.Update();
            this.m_Entries.arraySize++;
            serializedObject.ApplyModifiedProperties();
            this.m_EntryList.index = this.m_Entries.arraySize - 1;
        }
    }
}