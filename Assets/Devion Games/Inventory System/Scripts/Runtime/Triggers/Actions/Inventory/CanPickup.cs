using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames.UIWidgets;
using System.Linq;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Condition Item")]
    [ComponentMenu("Inventory System/Can Pickup")]
    public class CanPickup : Action, ICondition
    {
        [SerializeField]
        private string m_WindowName = "Inventory";

        private ItemCollection m_ItemCollection;

        public override void OnStart()
        {
            this.m_ItemCollection = gameObject.GetComponent<ItemCollection>();
        }

        public override ActionStatus OnUpdate()
        {
            bool result = ItemContainer.CanAddItems(this.m_WindowName, this.m_ItemCollection.ToArray());
            if (!result) {
                InventoryManager.Notifications.containerFull.Show(this.m_WindowName);
            }
            return result? ActionStatus.Success: ActionStatus.Failure;
        }
    }

}