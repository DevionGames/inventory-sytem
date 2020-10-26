using UnityEngine;
using System.Collections;

namespace DevionGames.InventorySystem.ItemActions{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Reduce Stack")]
	[System.Serializable]
	public class ReduceStack : ItemAction{
        [Range(1,200)]
        [SerializeField]
        private int m_Amount = 1;

		public override ActionStatus OnUpdate() {
            if (item.Stack < m_Amount) {
                return ActionStatus.Failure;
            }
            item.Stack -= m_Amount;
			return ActionStatus.Success;
		}

	}
}
