using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace DevionGames.StatSystem
{
	[CustomPropertyDrawer(typeof(StatPickerAttribute), true)]
	public class StatPickerDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			Stat current = property.GetValue() as Stat;
			position = EditorGUI.PrefixLabel(position, label);
			DoSelection(position, property, label, current);
			EditorGUI.EndProperty();
		}

		protected virtual void DoSelection(Rect buttonRect, SerializedProperty property, GUIContent label, Stat current)
		{

			GUIStyle buttonStyle = EditorStyles.objectField;
			GUIContent buttonContent = new GUIContent(current != null ? current.Name : "Null");
			if (GUI.Button(buttonRect, buttonContent, buttonStyle))
			{
				ObjectPickerWindow.ShowWindow(buttonRect, typeof(StatDatabase), BuildSelectableObjects(),
					(UnityEngine.Object obj) => {
						property.serializedObject.Update();
						property.objectReferenceValue = obj;
						property.serializedObject.ApplyModifiedProperties();
					},
					() => {
						StatDatabase db = EditorTools.CreateAsset<StatDatabase>(true);
					});
			}
		}

		protected virtual List<Stat> GetItems(StatDatabase database)
		{
			System.Type type = fieldInfo.FieldType;
			if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
			{
				type = Utility.GetElementType(fieldInfo.FieldType);
			}
			return database.items.Where(x => type.IsAssignableFrom(x.GetType())).ToList();
		}

		protected Dictionary<UnityEngine.Object, List<UnityEngine.Object>> BuildSelectableObjects()
		{
			Dictionary<UnityEngine.Object, List<UnityEngine.Object>> selectableObjects = new Dictionary<UnityEngine.Object, List<UnityEngine.Object>>();

			string[] guids = AssetDatabase.FindAssets("t:StatDatabase");
			for (int i = 0; i < guids.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(StatDatabase));
				List<UnityEngine.Object> items = GetItems(obj as StatDatabase).Cast<UnityEngine.Object>().ToList();
				for (int j = 0; j < items.Count; j++)
				{
					items[j].name = (items[j] as INameable).Name;
				}
				selectableObjects.Add(obj, items);
			}
			return selectableObjects;
		}
	}
}

