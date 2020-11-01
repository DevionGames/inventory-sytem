using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(ItemGroup))]
    public class ItemGroupInspector : Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_GroupName;

        private SerializedProperty m_Items;
        private ReorderableList m_ItemList;

        private SerializedProperty m_Modifiers;
        private ReorderableList m_ModifierList;

        private void OnEnable()
        {
            this.m_Script = serializedObject.FindProperty("m_Script");
            this.m_GroupName = serializedObject.FindProperty("m_GroupName");

            this.m_Items = serializedObject.FindProperty("m_Items");
            this.m_Modifiers = serializedObject.FindProperty("m_Modifiers");


            if (this.m_Items.arraySize > 0)
            {
                CheckForDatabase(this.m_Items.GetArrayElementAtIndex(0).objectReferenceValue);
            }

            CreateItemList(serializedObject, this.m_Items);
        }

        private void CreateItemList(SerializedObject serializedObject, SerializedProperty elements)
        {
            this.m_ItemList = new ReorderableList(serializedObject, elements, true, true, true, true);
            this.m_ItemList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Items (Item, Amount)");
            };

            this.m_ItemList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                float verticalOffset = (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y = rect.y + verticalOffset;
                rect.width = rect.width - 52f;
                SerializedProperty element = elements.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);

                SerializedProperty amounts = serializedObject.FindProperty("m_Amounts");
                if (amounts.arraySize < this.m_Items.arraySize)
                {
                    for (int i = amounts.arraySize; i < this.m_Items.arraySize; i++)
                    {
                        amounts.InsertArrayElementAtIndex(i);
                        amounts.GetArrayElementAtIndex(i).intValue = 1;
                    }
                }
                SerializedProperty amount = amounts.GetArrayElementAtIndex(index);
                rect.x += rect.width + 2f;
                rect.width = 50f;

                if (EditorApplication.isPlaying)
                {
                    amount.intValue = element.objectReferenceValue != null ? (element.objectReferenceValue as Item).Stack : amount.intValue;
                }
                EditorGUI.PropertyField(rect, amount, GUIContent.none);

            };

            this.m_ItemList.onReorderCallbackWithDetails = (ReorderableList list, int oldIndex, int newIndex) => {
                this.m_Modifiers.MoveArrayElement(oldIndex, newIndex);
                SerializedProperty amounts = serializedObject.FindProperty("m_Amounts");
                amounts.MoveArrayElement(oldIndex, newIndex);
            };

            this.m_ItemList.onAddCallback = (ReorderableList list) => {
                ReorderableList.defaultBehaviours.DoAddButton(list);
                this.m_Modifiers.InsertArrayElementAtIndex(list.index);
            };

            this.m_ItemList.onRemoveCallback = (ReorderableList list) =>
            {
                this.m_Modifiers.DeleteArrayElementAtIndex(list.index);
                this.m_ModifierList = null;
                SerializedProperty amounts = serializedObject.FindProperty("m_Amounts");
                amounts.DeleteArrayElementAtIndex(list.index);

                list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            };

            this.m_ItemList.onSelectCallback = (ReorderableList list) =>
            {
                if (this.m_Modifiers.arraySize < this.m_Items.arraySize)
                {
                    for (int i = m_Modifiers.arraySize; i < this.m_Items.arraySize; i++)
                    {
                        m_Modifiers.InsertArrayElementAtIndex(i);
                    }
                }
                CreateModifierList("Modifiers", serializedObject, this.m_Modifiers.GetArrayElementAtIndex(list.index));
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
        }


        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(this.m_Script);
            EditorGUI.EndDisabledGroup();

            serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_GroupName);
            this.m_ItemList.DoLayoutList();
            EditorGUILayout.Space();
            if (this.m_ModifierList != null)
                this.m_ModifierList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();


        }

        private void CheckForDatabase(Object current)
        {
            if (InventorySystemEditor.Database == null && current != null)
            {
                if (EditorApplication.isPlaying)
                {
                    InventorySystemEditor.Database = InventoryManager.Database;
                }

                ItemDatabase[] databases = EditorTools.FindAssets<ItemDatabase>();

                for (int i = 0; i < databases.Length; i++)
                {
                    if (typeof(Item).IsAssignableFrom(current.GetType()))
                    {

                        if (databases[i].items.Contains((Item)current))
                        {
                            InventorySystemEditor.Database = databases[i];
                            break;
                        }
                    }
                    if (typeof(Currency).IsAssignableFrom(current.GetType()))
                    {

                        if (databases[i].currencies.Contains((Currency)current))
                        {
                            InventorySystemEditor.Database = databases[i];
                            break;
                        }
                    }
                    if (typeof(EquipmentRegion).IsAssignableFrom(current.GetType()))
                    {

                        if (databases[i].equipments.Contains((EquipmentRegion)current))
                        {
                            InventorySystemEditor.Database = databases[i];
                            break;
                        }
                    }
                    if (typeof(Rarity).IsAssignableFrom(current.GetType()))
                    {

                        if (databases[i].raritys.Contains((Rarity)current))
                        {
                            InventorySystemEditor.Database = databases[i];
                            break;
                        }
                    }
                    if (typeof(Category).IsAssignableFrom(current.GetType()))
                    {

                        if (databases[i].categories.Contains((Category)current))
                        {
                            InventorySystemEditor.Database = databases[i];
                            break;
                        }
                    }
                }
            }
        }
    }
}