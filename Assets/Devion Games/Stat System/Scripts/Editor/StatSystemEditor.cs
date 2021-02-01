using UnityEngine;
using UnityEditor;

namespace DevionGames.StatSystem
{
	public class StatSystemEditor : EditorWindow
	{

		private StatSystemInspector m_StatSystemInspector;

		public static void ShowWindow()
		{

			StatSystemEditor[] objArray = Resources.FindObjectsOfTypeAll<StatSystemEditor>();
			StatSystemEditor editor = (objArray.Length <= 0 ? ScriptableObject.CreateInstance<StatSystemEditor>() : objArray[0]);

			editor.hideFlags = HideFlags.HideAndDontSave;
			editor.minSize = new Vector2(690, 300);
			editor.titleContent = new GUIContent("Stat System");

			editor.Show();
		}

		private void OnEnable()
		{
			this.m_StatSystemInspector = new StatSystemInspector();
			this.m_StatSystemInspector.OnEnable();
		}

		private void OnDisable()
		{
			this.m_StatSystemInspector.OnDisable();
		}

		private void OnDestroy()
		{
			this.m_StatSystemInspector.OnDestroy();
		}

		private void Update()
		{
			if (EditorWindow.mouseOverWindow == this)
				Repaint();
		}

		private void OnGUI()
		{
			this.m_StatSystemInspector.OnGUI(position);
		}

	}
}