using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace DevionGames.InventorySystem
{
	[CustomPropertyDrawer (typeof(PickerAttribute), true)]
	public abstract class PickerDrawer<T> : PropertyDrawer where T: ScriptableObject, INameable
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			T current = (T)property.GetValue();
			position = EditorGUI.PrefixLabel(position, label);
			DoSelection (position, property, label, current);
			EditorGUI.EndProperty();
		}

		protected virtual void DoSelection (Rect buttonRect, SerializedProperty property, GUIContent label, T current)
		{

			GUIStyle buttonStyle = EditorStyles.objectField;
			GUIContent buttonContent = new GUIContent(current != null ? current.Name : "Null");
			if (GUI.Button(buttonRect, buttonContent, buttonStyle))
			{
				ObjectPickerWindow.ShowWindow(buttonRect, typeof(ItemDatabase), BuildSelectableObjects(),
					(UnityEngine.Object obj) => {
						property.serializedObject.Update();
						property.objectReferenceValue = obj;
						property.serializedObject.ApplyModifiedProperties();
					},
					() => {
						ItemDatabase db = EditorTools.CreateAsset<ItemDatabase>(true);
					}, (attribute as PickerAttribute).acceptNull);
			}
		}

		protected abstract List<T> GetItems(ItemDatabase database);

		protected Dictionary<UnityEngine.Object,List<UnityEngine.Object>> BuildSelectableObjects()
		{
			Dictionary<UnityEngine.Object,List<UnityEngine.Object>> selectableObjects = new Dictionary<UnityEngine.Object, List<UnityEngine.Object>>();

			string[] guids = AssetDatabase.FindAssets("t:ItemDatabase");
			for (int i = 0; i < guids.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDatabase));
				List<UnityEngine.Object> items = GetItems(obj as ItemDatabase).Cast<UnityEngine.Object>().ToList();
				for (int j = 0; j < items.Count; j++){
					items[j].name = (items[j] as INameable).Name; 
				}
				selectableObjects.Add(obj, items);
			}
			return selectableObjects;
		}
	}
}