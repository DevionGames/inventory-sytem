using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditorInternal;
using System.Collections.Generic;

namespace DevionGames.StatSystem
{
	[CustomEditor (typeof(StatsHandler), true)]
	public class StatsHandlerInspector : Editor
	{
		private SerializedProperty m_Stats;
		private ReorderableList m_StatList;

		private SerializedProperty m_Script;
		private int m_RenameIndex = -1;
		private int m_ClickCount;
	
		private void OnEnable ()
		{
			this.m_Stats = serializedObject.FindProperty ("stats");
			this.m_StatList = new ReorderableList (serializedObject, this.m_Stats, true, true, true, true) {
				drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate (DrawStatusHeader),
				drawElementCallback = new ReorderableList.ElementCallbackDelegate (DrawStatus),
				onSelectCallback = new ReorderableList.SelectCallbackDelegate (SelectStat),
				onAddCallback = new ReorderableList.AddCallbackDelegate (AddStat),
			};


            int layerIndex = EditorPrefs.GetInt ("StatusIndex" + target.GetInstanceID ().ToString (), -1);
			if (this.m_StatList.count > layerIndex) {
				this.m_StatList.index = layerIndex;
				SelectStat (this.m_StatList);
			}

			this.m_Script = serializedObject.FindProperty ("m_Script");
		}

		public override void OnInspectorGUI ()
		{

			serializedObject.Update ();
			EditorGUI.BeginChangeCheck ();
			bool enabled = GUI.enabled;
			GUI.enabled = false;
			EditorGUILayout.PropertyField (this.m_Script);
			GUI.enabled = enabled;
			DrawPropertiesExcluding (serializedObject, "m_Script", "stats");

			this.m_StatList.DoLayoutList ();

			if (this.m_StatList.index != -1) {
				GUILayout.Space (15f);
				DrawSelectedStat (this.m_Stats.GetArrayElementAtIndex (this.m_StatList.index));
			}

			if (EditorGUI.EndChangeCheck ()) {
				serializedObject.ApplyModifiedProperties ();
			}
		}

		private void DrawStatusHeader (Rect rect)
		{
			EditorGUI.LabelField (rect, "Stats");
		}

		private void DrawStatus (Rect rect, int index, bool isActive, bool isFocused)
		{
			rect.y += 2f;
			SerializedProperty element = this.m_Stats.GetArrayElementAtIndex (index);
			SerializedProperty name = element.FindPropertyRelative ("m_Name");

			if (index == this.m_RenameIndex) {

				string before = name.stringValue;
				GUI.SetNextControlName ("RenameStatusField");
				rect.height = rect.height - 4f;
				string after = EditorGUI.TextField (rect, name.stringValue);
				if (before != after) {
					name.stringValue = after;
				}
			} else {
				GUI.Label (rect, name.stringValue);
			}

			Event currentEvent = Event.current;
			switch (currentEvent.rawType) {
			case EventType.MouseDown:
				if (rect.Contains (currentEvent.mousePosition) && index == this.m_StatList.index && currentEvent.button == 0 && currentEvent.type == EventType.MouseDown) {
					this.m_ClickCount += 1;
				} 
				break;
			case EventType.KeyUp:
				if (currentEvent.keyCode == KeyCode.Return && this.m_RenameIndex != -1) {
					this.m_RenameIndex = -1;
					currentEvent.Use ();
				}
				break;
			case EventType.MouseUp:
				if (this.m_ClickCount > 0 && rect.Contains (currentEvent.mousePosition) && index == this.m_StatList.index && currentEvent.button == 0 && currentEvent.type == EventType.MouseUp) {
					this.m_RenameIndex = index;
					this.m_ClickCount = 0;
					EditorGUI.FocusTextInControl ("RenameStatusField");
					Event.current.Use ();

				} else if (!rect.Contains (Event.current.mousePosition) && Event.current.clickCount > 0 && index == this.m_StatList.index && this.m_RenameIndex != -1) {
					this.m_RenameIndex = -1;
					Event.current.Use ();
				}
				break;
			}
		}

		private void AddStat (ReorderableList list)
		{
			list.serializedProperty.serializedObject.Update ();
			list.serializedProperty.arraySize++;
			list.index = list.serializedProperty.arraySize - 1;
			list.serializedProperty.serializedObject.ApplyModifiedProperties ();
		}

        private void SelectStat (ReorderableList list)
		{
			EditorPrefs.SetInt ("StatusIndex" + target.GetInstanceID ().ToString (), list.index);
			this.m_RenameIndex = -1;
        }

		private void DrawSelectedStat (SerializedProperty element)
		{
            GUIStyle style = new GUIStyle("ProjectBrowserHeaderBgMiddle")
            {
                fontSize = 11,
                fontStyle = FontStyle.BoldAndItalic,
            };

            SerializedProperty name = element.FindPropertyRelative ("m_Name");
			EditorGUILayout.LabelField (name.stringValue, style);
			GUILayout.Space (8f);
			SerializedProperty formula = element.FindPropertyRelative("formula");
			SerializedProperty minValue = element.FindPropertyRelative("m_MinValue");
			SerializedProperty maxValue = element.FindPropertyRelative ("m_MaxValue");
			SerializedProperty regenerate = element.FindPropertyRelative("m_Regenerate");
			SerializedProperty regenerationRate = element.FindPropertyRelative("m_Rate");
			SerializedProperty displayDamage = element.FindPropertyRelative("m_DisplayDamage");


			EditorGUILayout.PropertyField (name);
			EditorGUILayout.PropertyField(formula);
			EditorGUILayout.PropertyField(minValue);
			EditorGUILayout.PropertyField (maxValue);
			EditorGUILayout.PropertyField(regenerate);
			if (regenerate.boolValue)
			{
				EditorGUILayout.PropertyField(regenerationRate);
			}
			EditorGUILayout.PropertyField(displayDamage);
			if (displayDamage.boolValue) {
				EditorGUI.indentLevel += 1;
				EditorGUILayout.PropertyField(element.FindPropertyRelative("m_DamageColor"));
				EditorGUILayout.PropertyField(element.FindPropertyRelative("m_CriticalDamageColor"));
				EditorGUI.indentLevel -= 1;
			}

		}
	}
}