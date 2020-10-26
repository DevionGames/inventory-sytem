using UnityEngine;
using System;
using System.Collections;

namespace DevionGames.InventorySystem{
	public class ItemPickerAttribute : PickerAttribute {

		public ItemPickerAttribute():this(false){}
		
		public ItemPickerAttribute(bool utility):base(utility){}
	}
}