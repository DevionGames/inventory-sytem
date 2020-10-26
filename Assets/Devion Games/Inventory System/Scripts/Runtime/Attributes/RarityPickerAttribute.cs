using UnityEngine;
using System;
using System.Collections;

namespace DevionGames.InventorySystem{
	public class RarityPickerAttribute : PickerAttribute {

		public RarityPickerAttribute():this(false){}
		
		public RarityPickerAttribute(bool utility):base(utility){}
	}
}