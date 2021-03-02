using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DevionGames.InventorySystem{
	[System.Serializable]
	public class EquipmentItem : UsableItem {
		[SerializeField]
		protected GameObject m_OverrideEquipPrefab;
		public GameObject EquipPrefab { 
			get { 
				if(this.m_OverrideEquipPrefab != null)
				return this.m_OverrideEquipPrefab;
				return this.Prefab;
			} 
		}

		[EquipmentPicker]
		[SerializeField]
		private List<EquipmentRegion> m_Region= new List<EquipmentRegion>();
		public List<EquipmentRegion> Region{
			get{return this.m_Region;}
			set{this.m_Region = value;}
		}
	}
}