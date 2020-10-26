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

        private void OnEnable()
        {
            this.m_Script = serializedObject.FindProperty("m_Script");
            this.m_GroupName = serializedObject.FindProperty("m_GroupName");
            this.m_Items = serializedObject.FindProperty("m_Items");
            if (this.m_Items.arraySize > 0)
            {
                CheckForDatabase(this.m_Items.GetArrayElementAtIndex(0).objectReferenceValue);
            }


            this.m_ItemList = new ReorderableList(serializedObject, this.m_Items, true, true, true, true);
            this.m_ItemList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Items (Item, Amount, Property Randomizer)");
            };

            this.m_ItemList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                SerializedProperty element = this.m_ItemList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width = rect.width - 104f;
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
                if (InventorySystemEditor.Database == null || InventorySystemEditor.Database.items.Count == 0)
                {
                    rect.y += (9 + EditorGUIUtility.singleLineHeight + 6);
                }
                if (EditorApplication.isPlaying)
                {
                    amount.intValue = element.objectReferenceValue != null ? (element.objectReferenceValue as Item).Stack : amount.intValue;
                }
                EditorGUI.PropertyField(rect, amount, GUIContent.none);


                SerializedProperty randomProperties = serializedObject.FindProperty("m_RandomProperty");
                if (randomProperties.arraySize < this.m_Items.arraySize)
                {
                    for (int i = randomProperties.arraySize; i < this.m_Items.arraySize; i++)
                    {
                        randomProperties.InsertArrayElementAtIndex(i);
                        randomProperties.GetArrayElementAtIndex(i).floatValue = 0f;
                    }
                }
                SerializedProperty randomProperty = randomProperties.GetArrayElementAtIndex(index);
                rect.x += rect.width + 2f;
                rect.width = 50f;
                EditorGUI.PropertyField(rect, randomProperty, GUIContent.none);
            };


            this.m_ItemList.onReorderCallbackWithDetails = (ReorderableList list, int oldIndex, int newIndex) => {
                SerializedProperty amounts = serializedObject.FindProperty("m_Amounts");
                SerializedProperty randomProperties = serializedObject.FindProperty("m_RandomProperty");
                amounts.MoveArrayElement(oldIndex, newIndex);
                randomProperties.MoveArrayElement(oldIndex, newIndex);
            };

            this.m_ItemList.onRemoveCallback = (ReorderableList list) =>
            {
                SerializedProperty amounts = serializedObject.FindProperty("m_Amounts");
                amounts.DeleteArrayElementAtIndex(list.index);
                SerializedProperty randomProperties = serializedObject.FindProperty("m_RandomProperty");
                randomProperties.DeleteArrayElementAtIndex(list.index);
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
            EditorGUILayout.PropertyField(this.m_GroupName);
            this.m_ItemList.DoLayoutList();
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