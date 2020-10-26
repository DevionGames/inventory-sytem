using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DevionGames.InventorySystem{
	[System.Serializable]
	public class EquipmentItem : UsableItem {
		[EquipmentPicker(true)]
		[SerializeField]
		private List<EquipmentRegion> m_Region= new List<EquipmentRegion>();
		public List<EquipmentRegion> Region{
			get{return this.m_Region;}
			set{this.m_Region = value;}
		}
	}
}