using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace DevionGames.InventorySystem
{
	[CustomEditor (typeof(ItemCollection), true)]
	public class ItemCollectionInspector : Editor
	{
		private SerializedProperty script;
        private SerializedProperty savebale;
	
        private SerializedProperty m_Items;
        private ReorderableList m_ItemList;

        private SerializedProperty m_Modifiers;
        private ReorderableList m_ModifierList;

		private void OnEnable ()
		{
			this.script = serializedObject.FindProperty ("m_Script");
            this.savebale = serializedObject.FindProperty("saveable");

            this.m_Items = serializedObject.FindProperty("m_Items");
            this.m_Modifiers = serializedObject.FindProperty("m_Modifiers");


           /* if (this.m_Items.arraySize > 0) {
                CheckForDatabase(this.m_Items.GetArrayElementAtIndex(0).objectReferenceValue);
            }*/
            CreateItemList(serializedObject, this.m_Items);
        }

        private void CreateItemList(SerializedObject serializedObject, SerializedProperty elements) {
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

        private void CreateModifierList(string title, SerializedObject serializedObject, SerializedProperty property) {

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

       /* private void CheckForDatabase(Object current)
        {
            if (InventorySystemEditor.Database == null && current != null)
            {
                if (EditorApplication.isPlaying) {
                    InventorySystemEditor.Database = InventoryManager.Database;
                }

                ItemDatabase[] databases = EditorTools.FindAssets<ItemDatabase>();

                for (int i = 0; i < databases.Length; i++)
                {
                    List<INameable> items = new List<INameable>();
                    items.AddRange(databases[i].items);
                    items.AddRange(databases[i].categories);
                    items.AddRange(databases[i].raritys);
                    items.AddRange(databases[i].equipments);
                    items.AddRange(databases[i].currencies);
                    items.AddRange(databases[i].itemGroups);

                    if (items.Find(x => x == (INameable)current) != null)
                    {
                        InventorySystemEditor.Database = databases[i];
                    }

                }
            }
        }*/

        public override void OnInspectorGUI ()
		{
            EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField (script);
            EditorGUI.EndDisabledGroup();

			serializedObject.Update ();
            EditorGUILayout.PropertyField(savebale);
			GUILayout.Space (3f);
			//m_ItemList.elementHeight = (InventorySystemEditor.Database != null && (InventorySystemEditor.Database.items.Count > 0 || InventorySystemEditor.Database.currencies.Count>0) || m_ItemList.count == 0 ? 21 : (30 + EditorGUIUtility.singleLineHeight + 4));
			m_ItemList.DoLayoutList ();
            EditorGUILayout.Space();
            if (this.m_ModifierList != null)
                this.m_ModifierList.DoLayoutList();
			serializedObject.ApplyModifiedProperties ();
		}
	}
}