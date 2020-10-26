using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevionGames.InventorySystem.Restrictions
{
    public class EquipmentRegion : Restriction
    {
        [EquipmentPicker(true)]
        public DevionGames.InventorySystem.EquipmentRegion region;

        public override bool CanAddItem(Item item)
        {

            if (item == null || !(item is EquipmentItem equipmentItem)) { return false; }

            List<DevionGames.InventorySystem.EquipmentRegion> requiredRegions = new List<DevionGames.InventorySystem.EquipmentRegion>(equipmentItem.Region);

            Restrictions.EquipmentRegion[] restrictions = GetComponents<Restrictions.EquipmentRegion>();
            for (int i = requiredRegions.Count - 1; i >= 0; i--)
            {
                if (restrictions.Select(x => x.region).Contains(requiredRegions[i]))
                {
                    return true;
                }
            }
            return false;

            /*if (item.GetType() != typeof(EquipmentItem))
            {
                return false;
            }
            EquipmentItem mItem = item as EquipmentItem;
          

            for (int i = 0; i < mItem.Region.Count; i++)
            {
                if (mItem.Region[i].Name == region.Name)
                {
                    return true;
                }
            }

            return false;*/
        }
    }
}