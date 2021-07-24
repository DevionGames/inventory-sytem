using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames.UIWidgets;
using System.Linq;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Remove Visual Item")]
    public class RemoveVisualItem : Action
    {
        [SerializeField]
        protected EquipmentRegion m_Region;

        public override ActionStatus OnUpdate()
        {
            EquipmentHandler handler = playerInfo.gameObject.GetComponent<EquipmentHandler>();
            List<VisibleItem> items = handler.VisibleItems.Where(x => (x.item as EquipmentItem).Region.Contains(this.m_Region)).ToList();

            for (int i = 0; i < items.Count; i++) {
                items[i].OnItemUnEquip(items[i].item);
            }
            return ActionStatus.Success;
        }

        public override void OnInterrupt()
        {
            EquipmentHandler handler = playerInfo.gameObject.GetComponent<EquipmentHandler>();
            handler.UpdateEquipment();
        }
    }
}