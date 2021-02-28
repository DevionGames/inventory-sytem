using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace DevionGames
{
	[CustomEditor (typeof(ThirdPersonCamera))]
	public class ThirdPersonCameraEditor : Editor
	{
		private SerializedProperty m_Presets;
		private ReorderableList m_PresetList;
		private SerializedProperty m_Script;
		private int m_RenameIndex = -1;
		private int m_ClickCount;
		private ThirdPersonCamera m_Camera;

		private void OnEnable ()
		{
			this.m_Camera = target as ThirdPersonCamera;
			this.m_Presets = serializedObject.FindProperty ("m_Presets");
			this.m_PresetList = new ReorderableList (serializedObject, this.m_Presets, true, true, true, true) {
				drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate (DrawPresetHeader),
				drawElementCallback = new ReorderableList.ElementCallbackDelegate (DrawPreset),
				onSelectCallback = new ReorderableList.SelectCallbackDelegate (SelectPreset),
				onAddCallback = new ReorderableList.AddCallbackDelegate (AddPreset),
				drawElementBackgroundCallback = new ReorderableList.ElementCallbackDelegate(DrawPresetBackground)
			};

			int layerIndex = EditorPrefs.GetInt ("CameraPresetIndex" + target.GetInstanceID ().ToString (), -1);
			if (this.m_PresetList.count > layerIndex) {
				this.m_PresetList.index = layerIndex;
				SelectPreset (this.m_PresetList);
			}

			this.m_Script = serializedObject.FindProperty ("m_Script");
		}

		public override void OnInspectorGUI ()
		{

			if (this.m_Camera.Presets == null || m_Camera.Presets.Length == 0)
			{
				AddPreset(m_PresetList);
				serializedObject.ApplyModifiedProperties();
			}

			serializedObject.Update ();
			EditorGUI.BeginChangeCheck ();
			bool enabled = GUI.enabled;
			GUI.enabled = false;
			EditorGUILayout.PropertyField (this.m_Script);
			GUI.enabled = enabled;
			DrawPropertiesExcluding (serializedObject, "m_Script", "m_Presets");
			
			this.m_PresetList.DoLayoutList ();

			if (this.m_PresetList.index != -1) {
				GUILayout.Space (15f);
				DrawSelectedPreset (this.m_Presets.GetArrayElementAtIndex (this.m_PresetList.index));
			}

			if (EditorGUI.EndChangeCheck ()) {
				serializedObject.ApplyModifiedProperties ();
			}
		}

		private void DrawPresetHeader (Rect rect)
		{
			EditorGUI.LabelField (rect, "Preset");
		}

		private void DrawPresetBackground(Rect rect, int index, bool isActive, bool isFocused)
		{
			Color color = GUI.color;
			if (this.m_Camera.Presets != null)
			{
				for (int i = 0; i < this.m_Camera.Presets.Length; i++)
				{
					CameraSettings preset = this.m_Camera.Presets[i];
					if (i == index)
					{
						if (preset.IsActive)
						{
							GUI.color = Color.green * 0.8f;
							ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, true, false, true);
						}
						else
						{
							ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, isFocused, true);
						}
					}
					GUI.color = color;
				}
			}
		}


		private void DrawPreset (Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty element = this.m_Presets.GetArrayElementAtIndex (index);
			SerializedProperty name = element.FindPropertyRelative ("m_Name");

			EditorGUI.BeginDisabledGroup(name.stringValue == "Default");
			if (index == this.m_RenameIndex) {

				string before = name.stringValue;
				GUI.SetNextControlName ("RenamePresetField");
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
				if (rect.Contains (currentEvent.mousePosition) && index == m_PresetList.index && currentEvent.button == 0 && currentEvent.type == EventType.MouseDown) {
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
				if (this.m_ClickCount > 0 && rect.Contains (currentEvent.mousePosition) && index == m_PresetList.index && currentEvent.button == 0 && currentEvent.type == EventType.MouseUp) {
					this.m_RenameIndex = index;
					this.m_ClickCount = 0;
					EditorGUI.FocusTextInControl ("RenamePresetField");
					Event.current.Use ();

				} else if (!rect.Contains (Event.current.mousePosition) && Event.current.clickCount > 0 && index == this.m_PresetList.index && this.m_RenameIndex != -1) {
					this.m_RenameIndex = -1;
					Event.current.Use ();
				}
				break;
			}
			EditorGUI.EndDisabledGroup();
		}

		private void AddPreset (ReorderableList list)
		{
			list.serializedProperty.serializedObject.Update ();
			list.serializedProperty.arraySize++;
			list.index = list.serializedProperty.arraySize - 1;
			SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex (list.index);
			list.serializedProperty.serializedObject.ApplyModifiedProperties ();

			element.FindPropertyRelative ("m_Name").stringValue = list.serializedProperty.arraySize==1?"Default":"Camera Preset";
			element.FindPropertyRelative ("m_Offset").vector2Value = new Vector2 (0f, 1.5f);
			element.FindPropertyRelative ("m_Distance").floatValue = 2.5f;
			element.FindPropertyRelative("m_InheritDistance").boolValue = true;

			element.FindPropertyRelative ("m_YawLimit").vector2Value = new Vector2 (-180f, 180f);
			element.FindPropertyRelative ("m_PitchLimit").vector2Value = new Vector2 (-60f, 60f);
			element.FindPropertyRelative ("m_TurnSpeed").floatValue = 2f;
			element.FindPropertyRelative ("m_TurnSmoothing").floatValue = 0.05f;
			element.FindPropertyRelative ("m_MoveSmoothing").floatValue = 0.07f;
			element.FindPropertyRelative ("m_ZoomSpeed").floatValue = 5f;
			element.FindPropertyRelative ("m_ZoomLimit").vector2Value = new Vector2 (1.5f, 6f);
			element.FindPropertyRelative ("m_ZoomSmoothing").floatValue = 0.1f;
			element.FindPropertyRelative ("m_CollisionLayer").intValue = 1 << 0;
			element.FindPropertyRelative ("m_CollisionRadius").floatValue = 0.4f;
			element.FindPropertyRelative ("m_Offset").vector2Value = new Vector3 (0f, 1.5f);
		}

		private void SelectPreset (ReorderableList list)
		{
			EditorPrefs.SetInt ("CameraPresetIndex" + target.GetInstanceID ().ToString (), list.index);
			this.m_RenameIndex = -1;
		}

		private void DrawSelectedPreset (SerializedProperty property)
		{

			GUIStyle style = new GUIStyle ("ProjectBrowserHeaderBgMiddle") {
				fontSize = 12,
				fontStyle = FontStyle.Bold,
				//contentOffset = new Vector2 (3, -2),
			};

			EditorGUILayout.LabelField (property.FindPropertyRelative ("m_Name").stringValue, style);
			GUILayout.Space (8f);
		
			property.Next (true);
			int depth = property.depth;
			property.Next (false);
			do {
				EditorGUILayout.PropertyField (property, false);
			} while (property.NextVisible (false) && property.depth == depth);
				
			//EditorGUILayout.LabelField ("", style);
		}
	}
}