using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Lock")]
    public class Lock : Action
    {
        [Tooltip("The name of the window to lock.")]
        [SerializeField]
        private string m_WindowName = "Loot";
        [SerializeField]
        private bool m_State = true;
        private ItemContainer m_ItemContainer;

        public override void OnStart()
        {
            this.m_ItemContainer = WidgetUtility.Find<ItemContainer>(this.m_WindowName);
        }

        public override ActionStatus OnUpdate()
        {
            if (this.m_ItemContainer == null)
            {
                Debug.LogWarning("Missing window " + this.m_WindowName + " in scene!");
                return ActionStatus.Failure;
            }

            this.m_ItemContainer.Lock(this.m_State);
           
            return ActionStatus.Success;
        }
    }
}
