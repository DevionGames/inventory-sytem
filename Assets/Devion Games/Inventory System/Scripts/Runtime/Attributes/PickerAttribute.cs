using UnityEngine;
using System;
using System.Collections;

namespace DevionGames.InventorySystem{
	public class PickerAttribute : PropertyAttribute {

		public bool acceptNull;

		public PickerAttribute():this(false){}
		
		public PickerAttribute(bool acceptNull){
			this.acceptNull = acceptNull;
		}
	}
}