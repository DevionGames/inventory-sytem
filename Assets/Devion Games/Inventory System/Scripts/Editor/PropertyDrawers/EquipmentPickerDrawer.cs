using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace DevionGames.InventorySystem{
	[CustomPropertyDrawer(typeof(EquipmentPickerAttribute))]
	public class EquipmentPickerDrawer : PickerDrawer<EquipmentRegion> {

		protected override List<EquipmentRegion> Items {
			get {
				return Database.equipments;
			}
		}
	}
}