using UnityEngine;
using System.Collections;

namespace DevionGames.InventorySystem{
	[System.Serializable]
	public class Rarity : ScriptableObject, INameable {
		[SerializeField]
		private new string name="";
		public string Name{
			get{return this.name;}
			set{this.name = value;}
		}

		[SerializeField]
		private bool m_UseAsNamePrefix = false;
		public bool UseAsNamePrefix {
			get { return this.m_UseAsNamePrefix; }
		}


		[SerializeField]
		private Color color=Color.white;
		public Color Color{
			get{return this.color;}
			set{this.color = value;}
		}

		//Percent to get this rarity
		[SerializeField]
		private int chance = 100;
		public int Chance
		{
			get { return this.chance; }
			set { this.chance = value; }
		}

		[InspectorLabel("Property Multiplier")]
		[SerializeField]
		private float multiplier = 1.0f;
		public float Multiplier
		{
			get { return this.multiplier; }
			set { this.multiplier = value; }
		}

		[InspectorLabel("Price Multiplier")]
		[SerializeField]
		private float m_PriceMultiplier = 1.0f;
		public float PriceMultiplier
		{
			get { return this.m_PriceMultiplier; }
			set { this.m_PriceMultiplier = value; }
		}
	}
}