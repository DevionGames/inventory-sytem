using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace DevionGames.InventorySystem{
	[CustomPropertyDrawer(typeof(CurrencyPickerAttribute))]
	public class CurrencyPickerDrawer : PickerDrawer<Currency> {

		protected override List<Currency> Items {
			get {
				return Database.currencies;
			}
		}
	}
}