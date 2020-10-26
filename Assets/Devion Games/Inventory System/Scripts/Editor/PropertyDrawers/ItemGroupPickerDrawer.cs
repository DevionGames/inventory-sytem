using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace DevionGames.InventorySystem{
	[CustomPropertyDrawer(typeof(ItemGroupPickerAttribute))]
	public class ItemGroupPickerDrawer : PickerDrawer<ItemGroup> {

		protected override List<ItemGroup> Items {
			get {
				return Database.itemGroups;
			}
		}

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ItemGroup current = (ItemGroup)property.GetValue();//GetCurrent(property);
            CheckForDatabase(current);

            if (Database == null || Items.Count == 0)
            {
                string errMsg = Database == null ? "No database selected. Please open the editor and select a database." : "There are no items in this database. Please open the editor and create at least one item.";
                position.height = 30;
                EditorGUI.HelpBox(position, errMsg, MessageType.Error);
                position.y += 32;
                position.height = EditorGUIUtility.singleLineHeight;
                //EditorGUI.Popup (position, System.Text.RegularExpressions.Regex.Replace (typeof(T).Name, "([a-z])_?([A-Z])", "$1 $2"), 0, new string[]{current!=null?current.Name:"Null"});
                EditorGUI.Popup(position, label.text, 0, new string[] { current != null ? current.Name : "Database" });
                return;
            }

            DoSelection(position, property, label, current);
        }

        protected override void DoSelection(Rect position, SerializedProperty property, GUIContent label, ItemGroup current)
        {
            if (!string.IsNullOrEmpty(label.text))
            {
                EditorGUI.LabelField(position, label);
                position.x += EditorGUIUtility.labelWidth;
                position.width = Screen.width - EditorGUIUtility.labelWidth - 18 * 2;
            }
            if (GUI.Button(position, current != null ? current.Name : "Database", EditorStyles.objectField))
            {
                string searchString = "Search...";
                UtilityInstanceWindow.ShowWindow("Item Group Picker", delegate () {
                    searchString = EditorTools.SearchField(searchString);

                    Color color = GUI.backgroundColor;
                    GUI.backgroundColor = current == null ? Color.green : color;
                    if (GUILayout.Button("Database"))
                    {
                        property.SetValue(null);
                    //   SetValue(null, property);
                        UtilityInstanceWindow.CloseWindow();
                    }
                    GUI.backgroundColor = color;

                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(searchString) && !searchString.Equals("Search...") && !Items[i].Name.ToLower().Contains(searchString.ToLower()))
                        {
                            continue;
                        }
                        color = GUI.backgroundColor;
                        GUI.backgroundColor = current != null && current.Name == Items[i].Name ? Color.green : color;
                        if (GUILayout.Button(Items[i].Name))
                        {
                            property.SetValue(Items[i]);
                            //SetValue(Items[i], property);
                            UtilityInstanceWindow.CloseWindow();
                        }
                        GUI.backgroundColor = color;
                    }
                });
            }

        }
    }
}