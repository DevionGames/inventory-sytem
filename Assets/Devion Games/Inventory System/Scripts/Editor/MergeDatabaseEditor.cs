using DevionGames.InventorySystem.ItemActions;
using DevionGames.InventorySystem.Restrictions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace DevionGames.InventorySystem
{
	public class MergeDatabaseEditor : EditorWindow
	{

		private ItemDatabase m_Source;
		private ItemDatabase m_Destination;
		public static void ShowWindow()
		{
			MergeDatabaseEditor window = EditorWindow.GetWindow<MergeDatabaseEditor>(true, "Merge Database");
			Vector2 size = new Vector2(380f, 72f);
			window.minSize = size;
			window.wantsMouseMove = true;
		}

        private void OnGUI()
        {
			EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Source Database", GUILayout.Width(150f));
			SelectDatabase(this.m_Source,delegate(ItemDatabase db) { this.m_Source = db; });
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Destination Database", GUILayout.Width(150f));
			SelectDatabase(this.m_Destination,delegate (ItemDatabase db) { this.m_Destination = db; });
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(5);
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUI.BeginDisabledGroup(this.m_Source == null || this.m_Destination == null);
			if (GUILayout.Button("Merge", GUILayout.Width(70))) {
				MergeItems();
				MergeRarities();
				MergeCategories();
				MergeCurrencies();
				MergeEquipmentRegions();
				MergeItemGroups();
				UpdateReferences();
			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}

		private void UpdateReferences()
		{
			List<INameable> items = new List<INameable>();
			items.AddRange(this.m_Destination.items);
			items.AddRange(this.m_Destination.categories);
			items.AddRange(this.m_Destination.raritys);
			items.AddRange(this.m_Destination.equipments);
			items.AddRange(this.m_Destination.currencies);
			items.AddRange(this.m_Destination.itemGroups);
			for (int i = 0; i < this.m_Destination.items.Count; i++)
			{
				Item item = this.m_Destination.items[i];
                UpdateReference(item, items);
			}

			for (int i = 0; i < this.m_Destination.itemGroups.Count; i++)
			{
				ItemGroup group = this.m_Destination.itemGroups[i];
				UpdateReference(group, items);
			}

			for (int i = 0; i < this.m_Destination.currencies.Count; i++)
			{
				Currency currency = this.m_Destination.currencies[i];
				UpdateReference(currency, items);
			}

		}

		private void UpdateReference(object source, List<INameable> destItems)
		{
			if (source == null) {
				return;
			}
			FieldInfo[] fields = source.GetType().GetAllSerializedFields();
			for (int j = 0; j < fields.Length; j++)
			{
				FieldInfo fieldInfo = fields[j];

				if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
				{
					System.Type elementType = fieldInfo.FieldType.GetElementType();
					if (elementType == null) { elementType = fieldInfo.FieldType.GetGenericArguments()[0]; }

					if ( typeof(INameable).IsAssignableFrom(elementType) && typeof(ScriptableObject).IsAssignableFrom(elementType))
					{
						//Debug.Log("INameable List: "+(source as INameable).Name+" "+fieldInfo.Name + " (" + fieldInfo.FieldType + ")");
						IList array = fieldInfo.GetValue(source) as IList;

                        System.Type targetType = typeof(List<>).MakeGenericType(Utility.GetElementType(fieldInfo.FieldType));
						IList items = (IList)Activator.CreateInstance(targetType);
						for (int i = 0; i < array.Count; i++)
						{
							INameable replacement = destItems.Find(x => x.Name == (array[i] as INameable).Name);
							items.Add(replacement);
						}
						if (fieldInfo.FieldType.IsArray)
						{
							Array arr = Array.CreateInstance(Utility.GetElementType(fieldInfo.FieldType), items.Count);
							items.CopyTo(arr, 0);
							items=arr;
						}

						fieldInfo.SetValue(source, items);
					}else
					{
						//Debug.Log("Custom Class List: " + (source as INameable).Name + " " + fieldInfo.Name + " (" + fieldInfo.FieldType.GetElementType().ToString() + ")");
						IList list = fieldInfo.GetValue(source) as IList;
						foreach (var o in list)
						{
							UpdateReference(o, destItems);
						}
					}
				}
				else if (typeof(INameable).IsAssignableFrom(fieldInfo.FieldType) && typeof(ScriptableObject).IsAssignableFrom(fieldInfo.FieldType))
				{
					//Debug.Log("Direct INameable Field: " +source.GetType()+" "+ fieldInfo.Name + " (" + fieldInfo.FieldType + ")");
					INameable item = (INameable)fieldInfo.GetValue(source);
					if (item != null)
					{
						INameable replacement = destItems.Find(x => x.Name == item.Name);
						fieldInfo.SetValue(source, replacement);
					}
				}
				else 
				{
					//Debug.Log("Custom Class: "+ (source as INameable).Name + " " + fieldInfo.Name + " (" + fieldInfo.FieldType + ")");
					object subSource = fieldInfo.GetValue(source);
					UpdateReference(subSource, destItems);

				}
			}
		}

		private void MergeItems() {
			List<Item> items = this.m_Source.items.Where(y => !this.m_Destination.items.Any(z => z.Name == y.Name)).ToList();
			for (int i = 0; i < items.Count; i++)
			{
				Item item = ScriptableObject.Instantiate(items[i]);
				item.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(item, this.m_Destination);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				this.m_Destination.items.Add(item);
				/*if (item is UsableItem) {
					List<ItemAction> actions = (item as UsableItem).actions;
					for (int j = 0; j < actions.Count; j++) {
						ItemAction action = ScriptableObject.Instantiate(actions[j]);
						action.hideFlags = HideFlags.HideInHierarchy;
						AssetDatabase.AddObjectToAsset(action, item);
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
						actions[j] = action;
						EditorUtility.SetDirty(item);
					}

				}*/

				EditorUtility.SetDirty(this.m_Destination);
			}
		}

		private void MergeCurrencies() {
			List<Currency> items = this.m_Source.currencies.Where(y => !this.m_Destination.currencies.Any(z => z.Name == y.Name)).ToList();
			for (int i = 0; i < items.Count; i++)
			{
				Currency item = ScriptableObject.Instantiate(items[i]);
				item.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(item, this.m_Destination);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				this.m_Destination.currencies.Add(item);
				EditorUtility.SetDirty(this.m_Destination);
			}


		}

		private void MergeRarities() {
			List<Rarity> items = this.m_Source.raritys.Where(y => !this.m_Destination.raritys.Any(z => z.Name == y.Name)).ToList();
			for (int i = 0; i < items.Count; i++)
			{
				Rarity item = ScriptableObject.Instantiate(items[i]);
				item.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(item, this.m_Destination);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				this.m_Destination.raritys.Add(item);
				EditorUtility.SetDirty(this.m_Destination);
			}
		}

		private void MergeCategories()
		{
			List<Category> items = this.m_Source.categories.Where(y => !this.m_Destination.categories.Any(z => z.Name == y.Name)).ToList();
			for (int i = 0; i < items.Count; i++)
			{
				Category item = ScriptableObject.Instantiate(items[i]);
				item.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(item, this.m_Destination);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				this.m_Destination.categories.Add(item);
				EditorUtility.SetDirty(this.m_Destination);
			}
		}

		private void MergeEquipmentRegions()
		{
			List<EquipmentRegion> items = this.m_Source.equipments.Where(y => !this.m_Destination.equipments.Any(z => z.Name == y.Name)).ToList();
			for (int i = 0; i < items.Count; i++)
			{
				EquipmentRegion item = ScriptableObject.Instantiate(items[i]);
				item.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(item, this.m_Destination);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				this.m_Destination.equipments.Add(item);
				EditorUtility.SetDirty(this.m_Destination);
			}
		}

		private void MergeItemGroups() {
			List<ItemGroup> items = this.m_Source.itemGroups.Where(y => !this.m_Destination.itemGroups.Any(z => z.Name == y.Name)).ToList();
			for (int i = 0; i < items.Count; i++)
			{
				ItemGroup item = ScriptableObject.Instantiate(items[i]);
				item.hideFlags = HideFlags.HideInHierarchy;
				AssetDatabase.AddObjectToAsset(item, this.m_Destination);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				this.m_Destination.itemGroups.Add(item);
				EditorUtility.SetDirty(this.m_Destination);
			}
		}


		private ScriptableObject CreateAsset(ScriptableObject target, System.Type type) 
		{

			ScriptableObject asset = ScriptableObject.CreateInstance(type);
			asset.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(asset, target);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.SetDirty(target);
			return asset;
		}

		private void SelectDatabase(ItemDatabase current,UnityAction<ItemDatabase> result)
		{

			if (GUILayout.Button(current != null ? current.name : "Null", EditorStyles.objectField))
			{
				string searchString = "Search...";
				ItemDatabase[] databases = EditorTools.FindAssets<ItemDatabase>();

				UtilityInstanceWindow.ShowWindow("Select Database", delegate ()
				{
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
							result.Invoke(databases[i]);
							UtilityInstanceWindow.CloseWindow();
							Repaint();
						}
					}
				});
			}


		}
	}
}
