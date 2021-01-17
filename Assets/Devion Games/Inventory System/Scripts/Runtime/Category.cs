using UnityEngine;
using System.Collections;

namespace DevionGames.InventorySystem{
	[System.Serializable]
	public class Category : ScriptableObject,INameable {
		[CategoryPicker(true)]
		[SerializeField]
		protected Category m_Parent;

		public Category Parent
        {
			get { return this.m_Parent; }
			set { this.m_Parent = value; }
        }

		[SerializeField]
		private new string name="";
		public string Name{
			get{return this.name;}
			set{this.name = value;}
		}


		[SerializeField]
		protected Color m_EditorColor = Color.clear;
		public Color EditorColor {
			get { return this.m_EditorColor; }
		}

        [SerializeField]
        protected float m_Cooldown = 1f;
        public float Cooldown {
            get { return this.m_Cooldown; }
        }

		public bool IsAssignable(Category other) {
			if (other == null)
				return false;

			if (this.Name == other.Name) {
				return true;
			}

			if (other.Parent != null) {
				return IsAssignable(other.Parent);
			}
			return false;
		}
	}
}