using UnityEngine;
using System;
using System.Collections;

namespace DevionGames.InventorySystem{
	public class ItemGroupPickerAttribute : PickerAttribute {
        public ItemGroupPickerAttribute() : this(true) { }

      
		private ItemGroupPickerAttribute(bool utility):base(utility){
        }
	}
}