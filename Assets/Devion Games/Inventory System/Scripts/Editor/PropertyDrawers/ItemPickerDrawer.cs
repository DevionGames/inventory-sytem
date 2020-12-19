using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DevionGames.InventorySystem{
	[CustomPropertyDrawer(typeof(ItemPickerAttribute))]
	public class ItemPickerDrawer : PickerDrawer<Item> {

		protected override List<Item> GetItems(ItemDatabase database)
		{
			System.Type type = fieldInfo.FieldType;
			if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
			{
				type = Utility.GetElementType(fieldInfo.FieldType);
			}
			return database.allItems.Where(x => type.IsAssignableFrom(x.GetType())).ToList();
		}
	}
}