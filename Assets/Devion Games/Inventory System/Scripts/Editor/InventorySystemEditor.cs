using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;

namespace DevionGames.InventorySystem
{
	public class InventorySystemEditor : EditorWindow
	{

		private InventorySystemInspector m_InventorySystemInspector;

		public static void ShowWindow ()
		{
	
			InventorySystemEditor[] objArray = Resources.FindObjectsOfTypeAll<InventorySystemEditor> ();
			InventorySystemEditor editor = (objArray.Length <= 0 ? ScriptableObject.CreateInstance<InventorySystemEditor> () : objArray [0]);

			editor.hideFlags = HideFlags.HideAndDontSave;
			editor.minSize = new Vector2 (690, 300);
			editor.titleContent = new GUIContent ("Inventory System");

			editor.Show();
		}

		private void OnEnable()
		{
			this.m_InventorySystemInspector = new InventorySystemInspector();
			this.m_InventorySystemInspector.OnEnable();
		}

		private void OnDisable()
		{
			this.m_InventorySystemInspector.OnDisable();
		}

		private void OnDestroy()
		{
			this.m_InventorySystemInspector.OnDestroy();
		}

		private void Update()
		{
			if (EditorWindow.mouseOverWindow == this)
				Repaint();
		}

		private void OnGUI()
		{
			this.m_InventorySystemInspector.OnGUI(position);
		}

	}
}