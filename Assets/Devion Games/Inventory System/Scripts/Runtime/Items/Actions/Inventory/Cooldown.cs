using UnityEngine;
using System.Collections;
using DevionGames.UIWidgets;

namespace DevionGames.InventorySystem.ItemActions{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Cooldown")]
	[System.Serializable]
	public class Cooldown : ItemAction{
        [SerializeField]
        private float m_GlobalCooldown = 0.5f;
       

        public override ActionStatus OnUpdate() {
            ItemContainer.Cooldown(item, this.m_GlobalCooldown);
			return ActionStatus.Success;
		}

	}
}
