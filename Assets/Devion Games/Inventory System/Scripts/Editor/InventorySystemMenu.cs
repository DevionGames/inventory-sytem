using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DevionGames.InventorySystem
{
	public static class InventorySystemMenu
	{
		private const string componentToolbarMenu = "Tools/Devion Games/Inventory System/Components/";

		[MenuItem ("Tools/Devion Games/Inventory System/Editor", false, 0)]
		private static void OpenItemEditor ()
		{
			InventorySystemEditor.ShowWindow ();
		}

		[MenuItem("Tools/Devion Games/Inventory System/Merge Database", false, 1)]
		private static void OpenMergeDatabaseEditor()
		{
			MergeDatabaseEditor.ShowWindow();
		}

		[MenuItem("Tools/Devion Games/Inventory System/Item Reference Updater", false, 2)]
		private static void OpenItemReferenceEditor()
		{
			ItemReferenceEditor.ShowWindow();
		}

		[MenuItem ("Tools/Devion Games/Inventory System/Create Inventory Manager", false, 3)]
		private static void CreateInventoryManager ()
		{
			GameObject go = new GameObject ("Inventory Manager");
			go.AddComponent<InventoryManager> ();
			Selection.activeGameObject = go;
		}

		[MenuItem ("Tools/Devion Dames/Inventory System/Create Inventory Manager", true)]
		static bool ValidateCreateInventoryManager()
		{
			return GameObject.FindObjectOfType<InventoryManager> () == null;
		}
	}
}