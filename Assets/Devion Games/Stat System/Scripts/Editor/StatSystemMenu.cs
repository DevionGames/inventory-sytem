using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DevionGames.StatSystem{
	public static class StatSystemMenu {
		private const string componentToolbarMenu="Tools/Devion Games/Stat System/Components/";

		[MenuItem("Tools/Devion Games/Stat System/Configurations",false, 0)]
		private static void OpenEditor(){
			StatSystemEditor.ShowWindow ();
		}

		[MenuItem("Tools/Devion Games/Stat System/Create Stats Manager",false, 1)]
		private static void CreateStatManager(){
			GameObject go = new GameObject ("Stats Manager");
			go.AddComponent<StatsManager> ();
			Selection.activeGameObject = go;
		}
		
		[MenuItem ("Tools/Devion Games/Stat System/Create Stats Manager", true)]
		private static bool ValidateCreateStatusSystem() {
			return GameObject.FindObjectOfType<StatsManager> () == null;
		}
	}
}