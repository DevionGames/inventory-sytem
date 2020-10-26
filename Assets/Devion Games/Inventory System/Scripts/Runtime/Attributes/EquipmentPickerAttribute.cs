using UnityEngine;
using System;
using System.Collections;

namespace DevionGames.InventorySystem{
	public class EquipmentPickerAttribute : PickerAttribute {
		
		public EquipmentPickerAttribute():this(false){}
		
		public EquipmentPickerAttribute(bool utility):base(utility){}
	}
}