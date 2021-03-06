using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace DevionGames.InventorySystem{
	[CustomPropertyDrawer(typeof(EquipmentRegion))]
	public class EquipmentPickerDrawer : PickerDrawer<EquipmentRegion> {

		protected override List<EquipmentRegion> GetItems(ItemDatabase database) {
			return database.equipments;
		}
	}
}