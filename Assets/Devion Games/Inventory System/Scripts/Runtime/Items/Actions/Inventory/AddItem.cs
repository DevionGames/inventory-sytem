using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem.ItemActions
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Add Item")]
    [System.Serializable]
    public class AddItem : ItemAction
    {
        [SerializeField]
        private string m_WindowName = "Inventory";
        [SerializeField]
        private Item m_Item = null;
        [Range(1, 200)]
        [SerializeField]
        private int m_Amount = 1;

        public override ActionStatus OnUpdate()
        {
            Item instance = InventoryManager.CreateInstance(this.m_Item);
            instance.Stack = this.m_Amount;
            if (ItemContainer.AddItem(this.m_WindowName, instance))
            {
                return ActionStatus.Success;
            }
            return ActionStatus.Failure;
        }
    }
}