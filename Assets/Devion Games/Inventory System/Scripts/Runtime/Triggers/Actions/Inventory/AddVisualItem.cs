using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames.UIWidgets;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/AddVisualItem")]
    public class AddVisualItem : Action
    {
        [SerializeField]
        protected EquipmentItem m_Item;

        public override ActionStatus OnUpdate()
        {
            EquipmentHandler handler = playerInfo.gameObject.GetComponent<EquipmentHandler>();
            handler.EquipItem(m_Item);
            return ActionStatus.Success;
        }

        public override void OnInterrupt()
        {
            EquipmentHandler handler = playerInfo.gameObject.GetComponent<EquipmentHandler>();
            handler.UpdateEquipment();
        }
    }
}