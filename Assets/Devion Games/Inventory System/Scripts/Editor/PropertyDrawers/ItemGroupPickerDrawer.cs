using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace DevionGames.InventorySystem{
	[CustomPropertyDrawer(typeof(ItemGroupPickerAttribute))]
	public class ItemGroupPickerDrawer : PickerDrawer<ItemGroup> {

		protected override List<ItemGroup> GetItems(ItemDatabase database) {
            return database.itemGroups;
		}

        protected override void DoSelection(Rect buttonRect, SerializedProperty property, GUIContent label, ItemGroup current)
        {

            GUIStyle buttonStyle = EditorStyles.objectField;
            GUIContent buttonContent = new GUIContent(current != null ? current.Name : "Database");
            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
            {
                ObjectPickerWindow.ShowWindow(buttonRect, typeof(ItemDatabase), BuildSelectableObjects(),
                    (UnityEngine.Object obj) => {
                        property.serializedObject.Update();
                        property.objectReferenceValue = obj;
                        property.serializedObject.ApplyModifiedProperties();
                    },
                    () => {

                    });
            }
        }
    }
}