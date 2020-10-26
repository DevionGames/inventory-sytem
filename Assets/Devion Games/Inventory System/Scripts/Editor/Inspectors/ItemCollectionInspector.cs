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
		private ReorderableList itemList;

		private void OnEnable ()
		{
			this.script = serializedObject.FindProperty ("m_Script");
            SerializedProperty items = serializedObject.FindProperty("items");
            if (items.arraySize > 0) {
                CheckForDatabase(items.GetArrayElementAtIndex(0).objectReferenceValue);
            }

            this.itemList = new ReorderableList (serializedObject, items, true, true, true, true);
			this.itemList.drawHeaderCallback = (Rect rect) => {  
				EditorGUI.LabelField (rect, "Items (Item, Amount, Property Randomizer)");
			};


			this.itemList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				SerializedProperty element = itemList.serializedProperty.GetArrayElementAtIndex (index);
                rect.y += 2;
				rect.height = EditorGUIUtility.singleLineHeight;
                rect.width = rect.width - 104f;
				EditorGUI.PropertyField (rect, element, GUIContent.none, true);

                SerializedProperty amounts = serializedObject.FindProperty("amounts");
                if (amounts.arraySize < items.arraySize) {
                    for (int i = amounts.arraySize; i < items.arraySize; i++) {
                        amounts.InsertArrayElementAtIndex(i);
                        amounts.GetArrayElementAtIndex(i).intValue = 1;
                    }
                }
                SerializedProperty amount = amounts.GetArrayElementAtIndex(index);
                rect.x += rect.width+2f;
                rect.width = 50f;
                if (InventorySystemEditor.Database == null || InventorySystemEditor.Database.items.Count > 0)
                {
                    //rect.y += (9 + EditorGUIUtility.singleLineHeight + 6);
                }
                if (EditorApplication.isPlaying) {
                    amount.intValue = element.objectReferenceValue != null ? (element.objectReferenceValue as Item).Stack:amount.intValue;
                }
                EditorGUI.PropertyField(rect,amount,GUIContent.none);


                SerializedProperty randomProperties = serializedObject.FindProperty("randomProperty");
                if (randomProperties.arraySize < items.arraySize)
                {
                    for (int i = randomProperties.arraySize; i < items.arraySize; i++)
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
  
            this.itemList.onReorderCallbackWithDetails = (ReorderableList list, int oldIndex,int newIndex) => {
                SerializedProperty amounts = serializedObject.FindProperty("amounts");
                SerializedProperty randomProperties = serializedObject.FindProperty("randomProperty");
                amounts.MoveArrayElement(oldIndex, newIndex);
                randomProperties.MoveArrayElement(oldIndex,newIndex);
            };

            this.itemList.onRemoveCallback = (ReorderableList list) =>
            {
                SerializedProperty amounts = serializedObject.FindProperty("amounts");
                amounts.DeleteArrayElementAtIndex(list.index);
                SerializedProperty randomProperties = serializedObject.FindProperty("randomProperty");
                randomProperties.DeleteArrayElementAtIndex(list.index);
                list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            };
		}

        private void CheckForDatabase(Object current)
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

                    /*if (typeof(Item).IsAssignableFrom(current.GetType()))
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
                    }*/
                }
            }
        }

        public override void OnInspectorGUI ()
		{
			bool uiState = GUI.enabled;
			GUI.enabled = false;
			EditorGUILayout.PropertyField (script);
			GUI.enabled = uiState;

			serializedObject.Update ();
			GUILayout.Space (3f);
			itemList.elementHeight = (InventorySystemEditor.Database != null && (InventorySystemEditor.Database.items.Count > 0 || InventorySystemEditor.Database.currencies.Count>0) || itemList.count == 0 ? 21 : (30 + EditorGUIUtility.singleLineHeight + 4));
			itemList.DoLayoutList ();
			serializedObject.ApplyModifiedProperties ();
		}
	}
}