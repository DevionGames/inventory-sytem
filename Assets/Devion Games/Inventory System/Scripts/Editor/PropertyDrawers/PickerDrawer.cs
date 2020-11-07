using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DevionGames.InventorySystem
{
	[CustomPropertyDrawer (typeof(PickerAttribute), true)]
	public abstract class PickerDrawer<T> : PropertyDrawer where T: ScriptableObject, INameable
	{

		protected ItemDatabase Database {
			get {
				if (InventorySystemEditor.Database != null) {
					return InventorySystemEditor.Database;
				}
                return null;
			}
		}

		protected abstract List<T> Items { get; }

		protected virtual string[] Names {
			get {
				string[] items = new string[Items.Count];
				for (int i = 0; i < Items.Count; i++) {
					items [i] = Items [i].Name;
				}
				return items;
			}
		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			T current = (T)property.GetValue();
			
            CheckForDatabase(current);

            if (Database == null || Items.Count == 0) {
				string errMsg = Database == null ? "No database selected. Please open the editor and select a database." : "There are no items in this database. Please open the editor and create at least one item.";
				position.height = 30;
				EditorGUI.HelpBox (position, errMsg, MessageType.Error);
				position.y += 32;
				position.height = EditorGUIUtility.singleLineHeight;
				//EditorGUI.Popup (position, System.Text.RegularExpressions.Regex.Replace (typeof(T).Name, "([a-z])_?([A-Z])", "$1 $2"), 0, new string[]{current!=null?current.Name:"Null"});
				EditorGUI.Popup (position, label.text, 0, new string[]{ current != null ? current.Name : "Null" });
				return;
			}

			DoSelection (position, property, label, current);
			EditorGUI.EndProperty();
		}

        protected void CheckForDatabase(T current) {
            if (Database == null)
            {
                if (current != null)
                {
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

						if (items.Find(x => x == current) != null) {
							InventorySystemEditor.Database = databases[i];
						}
                    }
                }
            }
        }

		protected virtual void DoSelection (Rect position, SerializedProperty property, GUIContent label, T current)
		{
			
			if ((attribute as PickerAttribute).utility) {
				if (!string.IsNullOrEmpty (label.text)) {
					EditorGUI.LabelField (position, label);
					position.x += EditorGUIUtility.labelWidth;

                    position.width = Screen.width - position.x - EditorStyles.inspectorDefaultMargins.padding.right; //- 18 * 2);
				}
				if (GUI.Button (position, current != null ? current.Name : "Null", EditorStyles.objectField)) {
					string searchString = "Search...";
					UtilityInstanceWindow.ShowWindow (typeof(T).Name + " Picker ("+this.Database.name+")", delegate() {
						searchString = EditorTools.SearchField (searchString);
						for (int i = 0; i < Items.Count; i++) {
							if (!string.IsNullOrEmpty (searchString) && !searchString.Equals ("Search...") && !Items [i].Name.ToLower ().Contains (searchString.ToLower ())) {
								continue;
							}
							Color color = GUI.backgroundColor;
							GUI.backgroundColor = current != null && current.Name == Items [i].Name ? Color.green : color;
							if (GUILayout.Button (Items [i].Name)) {
								property.SetValue(Items[i]);
                             //   SetValue (Items [i], property);
                                UtilityInstanceWindow.CloseWindow ();
							}
							GUI.backgroundColor = color;
						}
					});
				}
			} else {
				int selectedIndex = Items.IndexOf (current);
				selectedIndex = Mathf.Clamp (selectedIndex, 0, Items.Count);
				// selectedIndex = EditorGUI.Popup(position, selectedIndex, Names);
				int index = EditorGUI.Popup (position, System.Text.RegularExpressions.Regex.Replace (typeof(T).Name, "([a-z])_?([A-Z])", "$1 $2"), selectedIndex, Names);
				if (selectedIndex != index)
				{
					property.SetValue(Items[index]);
					
				}
               // SetValue (Items [selectedIndex], property);
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
            if (Database == null || (Items.Count == 0 )) {
				return 30 + EditorGUIUtility.singleLineHeight + 2;
			} 
			return base.GetPropertyHeight (property, label);
		}
	}
}