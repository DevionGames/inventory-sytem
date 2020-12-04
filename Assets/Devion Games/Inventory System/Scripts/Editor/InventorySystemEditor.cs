using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;

namespace DevionGames.InventorySystem
{
	public class InventorySystemEditor : EditorWindow
	{
	
		public static void ShowWindow ()
		{
	
			InventorySystemEditor[] objArray = Resources.FindObjectsOfTypeAll<InventorySystemEditor> ();
			InventorySystemEditor editor = (objArray.Length <= 0 ? ScriptableObject.CreateInstance<InventorySystemEditor> () : objArray [0]);

			editor.hideFlags = HideFlags.HideAndDontSave;
			editor.minSize = new Vector2 (690, 300);
			editor.titleContent = new GUIContent ("Inventory System");
			editor.SelectDatabase ();
		}

		public static InventorySystemEditor instance;

		private ItemDatabase database;
        private static ItemDatabase db;
        public static ItemDatabase Database {
            get {
                if (InventorySystemEditor.instance != null) {
                    db = InventorySystemEditor.instance.database;
                }
                return db;
            }
            set
            {
                db = value;
                if (InventorySystemEditor.instance != null)
                {
                    InventorySystemEditor.instance.database = value;
                }
            }
        }

		private List<ICollectionEditor> childEditors;

		[SerializeField]
		private int toolbarIndex;

		private string[] toolbarNames {
			get {
                string[] items = new string[childEditors.Count];
                for (int i = 0; i < childEditors.Count; i++)
                {
                    items[i] = childEditors[i].ToolbarName;
                }
                return items;
			}
		}

		private void OnEnable ()
		{
			instance = this;

            if (database == null) {
                SelectDatabase();
            }
			ResetChildEditors ();
		}

        private void OnDisable()
        {
			if (childEditors != null)
			{
				for (int i = 0; i < childEditors.Count; i++)
				{
					childEditors[i].OnDisable();
				}
			}
		}

        private void Update()
        {
			if (EditorWindow.mouseOverWindow == this)
			{
				Repaint();
			}
        }

        private void OnDestroy()
        {
            if (childEditors != null)
            {
                for (int i = 0; i < childEditors.Count; i++)
                {
                    childEditors[i].OnDestroy();
                }
            }
            instance = null;
        }

        private void OnGUI ()
		{
			if (childEditors != null) {
				EditorGUILayout.Space ();
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				SelectDatabase(Database, delegate { ResetChildEditors(); UtilityInstanceWindow.CloseWindow(); });
				toolbarIndex = GUILayout.Toolbar (toolbarIndex, toolbarNames, GUILayout.MinWidth (200));
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
				childEditors [toolbarIndex].OnGUI (new Rect (0f, 30f, position.width, position.height - 30f));
			}
		}

		public static void SelectDatabase(ItemDatabase current, UnityAction onSelect)
		{

			if (GUILayout.Button(current != null ? current.name : "Null", EditorStyles.objectField))
			{
				SelectDatabase(onSelect);
			}
		}

		private void SelectDatabase ()
		{
			SelectDatabase(delegate {
				ResetChildEditors();
				Show();
				UtilityInstanceWindow.CloseWindow();
			});
		}

		public static void SelectDatabase(UnityAction onSelect)
		{
			string searchString = "Search...";
			ItemDatabase[] databases = EditorTools.FindAssets<ItemDatabase>();

			UtilityInstanceWindow.ShowWindow("Select Database", delegate () {
				searchString = EditorTools.SearchField(searchString);

				for (int i = 0; i < databases.Length; i++)
				{
					if (!string.IsNullOrEmpty(searchString) && !searchString.Equals("Search...") && !databases[i].name.Contains(searchString))
					{
						continue;
					}
					GUIStyle style = new GUIStyle("button");
					style.wordWrap = true;
					if (GUILayout.Button(AssetDatabase.GetAssetPath(databases[i]), style))
					{
						Database = databases[i];
						if (Database.categories.Count == 0)
							CreateDefaultCategory(Database);
						onSelect?.Invoke();
					}
				}
				GUILayout.FlexibleSpace();
				Color color = GUI.backgroundColor;
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Create"))
				{
					ItemDatabase db = EditorTools.CreateAsset<ItemDatabase>(true);
					if (db != null)
					{
						CreateDefaultCategory(db);
						ArrayUtility.Add<ItemDatabase>(ref databases, db);

					}
				}
				GUI.backgroundColor = color;
			});

		}

		private static void CreateDefaultCategory(ItemDatabase database) {
			Category category = ScriptableObject.CreateInstance<Category>();
			category.Name = "None";
			category.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(category, database);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			database.categories.Add(category);
			EditorUtility.SetDirty(database);
		}

		private void ResetChildEditors ()
		{
			if (database != null) {
				database.items.RemoveAll(x=> x == null);
				EditorUtility.SetDirty(database);
                childEditors = new List<ICollectionEditor> ();
				childEditors.Add (new ItemCollectionEditor (database, database.items, database.categories.Select (x => x.Name).ToList ()));
                childEditors.Add(new ScriptableObjectCollectionEditor<Currency>(database, database.currencies));
                childEditors.Add (new ScriptableObjectCollectionEditor<Rarity> (database, database.raritys));
				childEditors.Add (new ScriptableObjectCollectionEditor<Category> (database, database.categories));
				childEditors.Add (new ScriptableObjectCollectionEditor<EquipmentRegion> (database, database.equipments));
                childEditors.Add(new ScriptableObjectCollectionEditor<ItemGroup>(database, database.itemGroups));
                childEditors.Add (new Configuration.ItemSettingsEditor (database,database.settings));

				for (int i = 0; i < childEditors.Count; i++)
				{
					childEditors[i].OnEnable();
				}
			}
		}
	}
}