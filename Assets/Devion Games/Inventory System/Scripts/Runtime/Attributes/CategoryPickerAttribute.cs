using UnityEngine;
using System;
using System.Collections;

namespace DevionGames.InventorySystem{
	public class CategoryPickerAttribute : PickerAttribute {

		public CategoryPickerAttribute():this(false){}
		
		public CategoryPickerAttribute(bool utility):base(utility){}
	}
}