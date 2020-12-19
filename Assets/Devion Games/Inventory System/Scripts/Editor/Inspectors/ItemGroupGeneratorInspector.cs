using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(ItemGroupGenerator),true)]
    public class ItemGroupGeneratorInspector : Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_From;
        private SerializedProperty m_Filters;
        private SerializedProperty m_MinStack;
        private SerializedProperty m_MaxStack;
        private SerializedProperty m_MinAmount;
        private SerializedProperty m_MaxAmount;
        private SerializedProperty m_Chance;

        private ReorderableList m_FilterList;

        private SerializedProperty m_Modifiers;
        private ReorderableList m_ModifierList;


        protected virtual void OnEnable()
        {
            this.m_Script = serializedObject.FindProperty("m_Script");
            this.m_From = serializedObject.FindProperty("m_From");
            this.m_Filters = serializedObject.FindProperty("m_Filters");
            this.m_MinStack = serializedObject.FindProperty("m_MinStack");
            this.m_MaxStack = serializedObject.FindProperty("m_MaxStack");
            this.m_MinAmount = serializedObject.FindProperty("m_MinAmount");
            this.m_MaxAmount = serializedObject.FindProperty("m_MaxAmount");
            this.m_Chance = serializedObject.FindProperty("m_Chance");
            this.m_Modifiers = serializedObject.FindProperty("m_Modifiers");

            CreateModifierList("Modifiers", serializedObject, this.m_Modifiers);

            this.m_FilterList = new ReorderableList(serializedObject, this.m_Filters, true, true, true, true);
            this.m_FilterList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Filters");
            };


            this.m_FilterList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                SerializedProperty element = this.m_FilterList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(rect, (element.objectReferenceValue as INameable).Name+" ("+element.objectReferenceValue.GetType().Name+")");
            };

  
            this.m_FilterList.onRemoveCallback = (ReorderableList list) =>
            {
                list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            };

            this.m_FilterList.onAddDropdownCallback = (Rect rect, ReorderableList list) => {

                GenericMenu menu = new GenericMenu();
                string[] guids = AssetDatabase.FindAssets("t:ItemDatabase");
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    ItemDatabase database = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDatabase)) as ItemDatabase;
                    for (int j = 0; j < database.categories.Count; j++) {
                        Category category = database.categories[j];
                        menu.AddItem(new GUIContent(database.name + "/Category/" + database.categories[j].Name), false, () => {
                            serializedObject.Update();
                            this.m_Filters.InsertArrayElementAtIndex(this.m_Filters.arraySize);
                            SerializedProperty property = this.m_Filters.GetArrayElementAtIndex(this.m_Filters.arraySize - 1);
                            property.objectReferenceValue = category;
                            serializedObject.ApplyModifiedProperties();
                        });
                    }
                    for (int j = 0; j < database.raritys.Count; j++)
                    {
                        Rarity rarity = database.raritys[j];
                        menu.AddItem(new GUIContent(database.name + "/Rarity/" + database.raritys[j].Name), false, () => {
                            serializedObject.Update();
                            this.m_Filters.InsertArrayElementAtIndex(this.m_Filters.arraySize);
                            SerializedProperty property = this.m_Filters.GetArrayElementAtIndex(this.m_Filters.arraySize - 1);
                            property.objectReferenceValue = rarity;
                            serializedObject.ApplyModifiedProperties();
                        });
                    }
                }
                menu.DropDown(rect);
            };

        }



        private void CreateModifierList(string title, SerializedObject serializedObject, SerializedProperty property)
        {

            this.m_ModifierList = new ReorderableList(serializedObject, property.FindPropertyRelative("modifiers"), true, true, true, true);
            this.m_ModifierList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, title);
            };
            this.m_ModifierList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                float verticalOffset = (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y = rect.y + verticalOffset;
                SerializedProperty element = this.m_ModifierList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };

            this.m_ModifierList.onRemoveCallback = (ReorderableList list) =>
            {
                list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            };
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(this.m_Script);
            EditorGUI.EndDisabledGroup();
            serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_From);
     
            this.m_FilterList.DoLayoutList();
            EditorGUILayout.PropertyField(this.m_MinStack);
            EditorGUILayout.PropertyField(this.m_MaxStack);
            EditorGUILayout.PropertyField(this.m_MinAmount);
            EditorGUILayout.PropertyField(this.m_MaxAmount);
            EditorGUILayout.PropertyField(this.m_Chance);
            EditorGUILayout.Space();
            this.m_ModifierList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        

    }
}