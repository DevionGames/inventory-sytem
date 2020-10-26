using UnityEngine;
using System.Collections;

namespace DevionGames.InventorySystem{
	[System.Serializable]
	public class EquipmentRegion : ScriptableObject, INameable{
		[SerializeField]
		private new string name="";
		public string Name{
			get{return this.name;}
			set{this.name = value;}
		}
	}
}