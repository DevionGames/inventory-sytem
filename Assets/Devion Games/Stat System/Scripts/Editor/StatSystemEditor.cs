using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace DevionGames.StatSystem
{
	public class StatSystemEditor : EditorWindow
	{

		public static void ShowWindow()
		{

			StatSystemEditor[] objArray = Resources.FindObjectsOfTypeAll<StatSystemEditor>();
			StatSystemEditor editor = (objArray.Length <= 0 ? ScriptableObject.CreateInstance<StatSystemEditor>() : objArray[0]);
			editor.hideFlags = HideFlags.HideAndDontSave;
			editor.minSize = new Vector2(690, 300);
			editor.titleContent = new GUIContent("Stat System");
			editor.SelectDatabase();
		}

		public static StatSystemEditor instance;

		private StatConfigurations database;
		private static StatConfigurations db;
		public static StatConfigurations Database
		{
			get
			{
				if (StatSystemEditor.instance != null)
				{
					db = StatSystemEditor.instance.database;
				}
				return db;
			}
			set
			{
				db = value;
				if (StatSystemEditor.instance != null)
				{
					StatSystemEditor.instance.database = value;
				}
			}
		}

		private List<ICollectionEditor> childEditors;

		[SerializeField]
		private int toolbarIndex;

		private string[] toolbarNames
		{
			get
			{
				string[] items = new string[childEditors.Count];
				for (int i = 0; i < childEditors.Count; i++)
				{
					items[i] = childEditors[i].ToolbarName;
				}
				return items;
			}

		}

		private void OnEnable()
		{
			instance = this;
			if (database == null)
			{
				SelectDatabase();
			}
			ResetChildEditors();
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
			db = null;
		}


		private void OnGUI()
		{
			if (childEditors != null)
			{
				EditorGUILayout.Space();
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarNames, GUILayout.MinWidth(200));
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				childEditors[toolbarIndex].OnGUI(new Rect(0f, 30f, position.width, position.height - 30f));
			}
		}

		private void SelectDatabase()
		{
			string searchString = "Search...";
			StatConfigurations[] databases = EditorTools.FindAssets<StatConfigurations>();

			UtilityInstanceWindow.ShowWindow("Select Stat Configurations", delegate () {
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
						database = databases[i];
						ResetChildEditors();
						Show();
						UtilityInstanceWindow.CloseWindow();
					}
				}
				GUILayout.FlexibleSpace();
				Color color = GUI.backgroundColor;
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Create"))
				{
					StatConfigurations db = EditorTools.CreateAsset<StatConfigurations>(true);
					if (db != null)
					{
						ArrayUtility.Add<StatConfigurations>(ref databases, db);
					}
				}
				GUI.backgroundColor = color;
			});

		}

		private void ResetChildEditors()
		{
			if (database != null)
			{
				if (childEditors != null)
				{
					for (int i = 0; i < childEditors.Count; i++)
					{
						childEditors[i].OnDestroy();
					}
				}
				childEditors = new List<ICollectionEditor>();
				childEditors.Add(new Configuration.StatSettingsEditor(database, database.settings, database.settings.Select(x => x.Name).ToList()));
			}
		}
	}
}