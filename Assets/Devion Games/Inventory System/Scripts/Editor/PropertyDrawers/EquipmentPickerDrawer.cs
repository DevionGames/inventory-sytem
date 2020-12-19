using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace DevionGames.InventorySystem{
	[CustomPropertyDrawer(typeof(EquipmentPickerAttribute))]
	public class EquipmentPickerDrawer : PickerDrawer<EquipmentRegion> {

		protected override List<EquipmentRegion> GetItems(ItemDatabase database) {
			return database.equipments;
		}
	}
}