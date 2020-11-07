using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DevionGames.InventorySystem{
	[CustomPropertyDrawer(typeof(ItemPickerAttribute))]
	public class ItemPickerDrawer : PickerDrawer<Item> {

		protected override List<Item> Items {
			get {
                List<Item> items = new List<Item>(Database.currencies.Cast<Item>());
                items.AddRange(Database.items);
				System.Type type = fieldInfo.FieldType;

				if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType)) {
					type = Utility.GetElementType(fieldInfo.FieldType);
				}

				return items.Where(x => type.IsAssignableFrom(x.GetType())).ToList();
			}
		}
		

	}
}