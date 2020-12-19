using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System;

namespace DevionGames.InventorySystem
{
	[System.Serializable]
	public class ItemCollectionEditor : ScriptableObjectCollectionEditor<Item>
	{
		[SerializeField]
		protected List<string> searchFilters;
		[SerializeField]
		protected string searchFilter = "All";

        public override string ToolbarName
        {
            get
            {
                return "Items";
            }
        }

        public ItemCollectionEditor (UnityEngine.Object target, List<Item> items, List<string> searchFilters) : base (target, items)
		{
			this.target = target;
			this.items = items;
			this.searchFilters = searchFilters;
			this.searchFilters.Insert (0, "All");
            this.m_SearchString = "All";

			//Fix old items without category
			/*for (int i = 0; i < this.items.Count; i++) {
				if (this.items[i].Category == null && InventorySystemEditor.Database.categories.Count > 0) {
					this.items[i].Category = InventorySystemEditor.Database.categories[0];
					EditorUtility.SetDirty(this.items[i]);
				}
			}*/
        }

		protected override void Create()
		{
			Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => typeof(Item).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract && !typeof(Currency).IsAssignableFrom(type)).ToArray();
			if (types.Length > 1)
			{
				GenericMenu menu = new GenericMenu();
				foreach (Type type in types)
				{
					Type mType = type;
					menu.AddItem(new GUIContent(mType.Name), false, delegate () {
						CreateItem(mType);
					});
				}
				menu.ShowAsContext();
			}
			else
			{
				CreateItem(types[0]);
			}
		}

		protected override void DoSearchGUI ()
		{
			string[] searchResult = EditorTools.SearchField (m_SearchString, searchFilter, searchFilters);
			searchFilter = searchResult [0];
			m_SearchString = string.IsNullOrEmpty(searchResult [1])?searchFilter:searchResult[1] ;
		}

		protected override bool MatchesSearch (Item item, string search)
		{
			return (item.Name.ToLower ().Contains (search.ToLower ()) || m_SearchString == searchFilter || search.ToLower() == item.GetType().Name.ToLower()) && (searchFilter == "All" || item.Category.Name == searchFilter);
		}

		protected override string HasConfigurationErrors(Item item)
		{
			if (string.IsNullOrEmpty(item.Name))
				return "Name field can't be empty. Please enter a unique name.";

			if (Items.Any(x => !x.Equals(item) && x.Name == item.Name))
				return "Duplicate name. Item names need to be unique.";

			return string.Empty;
		}

        protected override void Duplicate(Item item)
        {
            Item duplicate = (Item)ScriptableObject.Instantiate(item);
			duplicate.Id = System.Guid.NewGuid().ToString();
			duplicate.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(duplicate, target);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Items.Add(duplicate);
			EditorUtility.SetDirty(target);
			Select(duplicate);
		}

        protected override void CreateItem(Type type)
        {
            Item item = (Item)ScriptableObject.CreateInstance(type);
			item.hideFlags = HideFlags.HideInHierarchy;
			ItemDatabase database = target as ItemDatabase;

			if (database.categories.Count > 0) {
				item.Category = database.categories[0];
			}
			if (database.currencies.Count > 0) {
				item.SellCurrency = database.currencies[0];
				item.BuyCurrency = database.currencies[0];
			}

			AssetDatabase.AddObjectToAsset(item, target);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Items.Add(item);
			Select(item);

			EditorUtility.SetDirty(target);
		}

        protected override void AddContextItem(GenericMenu menu)
		{
			base.AddContextItem(menu);
			menu.AddItem(new GUIContent("Sort/Category"), false, delegate {
                Item selected = selectedItem;
				Items.Sort(delegate (Item a, Item b) {
					return a.Category.Name.CompareTo(b.Category.Name); 
				});
				Select(selected);
				
			});
		}

		protected override void DrawItemLabel(int index, Item item)
		{
			GUILayout.BeginHorizontal();
			Color color = GUI.backgroundColor;
			GUI.backgroundColor = item.Category.EditorColor;
			Texture2D icon = null;
			if (item.Icon != null)
				icon = item.Icon.texture;
			GUILayout.Label(icon, Styles.indicatorColor, GUILayout.Width(17), GUILayout.Height(17));
			GUI.backgroundColor = color;
			GUILayout.Label(item.Name, Styles.selectButtonText);
			GUILayout.EndHorizontal();
		}
	}
}