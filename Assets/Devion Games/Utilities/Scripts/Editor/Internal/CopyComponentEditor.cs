using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DevionGames
{
	public class CopyComponentEditor : EditorWindow
	{
		private GameObject m_Source;
		private GameObject m_Destination;

		[UnityEditor.MenuItem("Tools/Devion Games/Internal/Copy Components", false)]
		public static void ShowWindow()
		{
			CopyComponentEditor window = EditorWindow.GetWindow<CopyComponentEditor>("Copy Components");
			Vector2 size = new Vector2(300f, 80f);
			window.minSize = size;
			window.wantsMouseMove = true;
		}

        private void OnGUI()
        {
			this.m_Source = EditorGUILayout.ObjectField("Source",this.m_Source, typeof(GameObject),true) as GameObject;
			this.m_Destination= EditorGUILayout.ObjectField("Destination",this.m_Destination, typeof(GameObject), true) as GameObject;
			if (this.m_Source == null || this.m_Destination == null)
				return;

			Component[] components = this.m_Source.GetComponents<Component>().Where(x=>x.hideFlags == HideFlags.None).ToArray();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Copy Components")) {
				for (int i = 0; i < components.Length; i++)
				{
					if (ComponentUtility.CopyComponent(components[i]))
					{
						Component component = this.m_Destination.AddComponent(components[i].GetType()) as Component;
						ComponentUtility.PasteComponentValues(component);
					}
				}
				Selection.activeObject = m_Destination;
			}

		}
    }
}