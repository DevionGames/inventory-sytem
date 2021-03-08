using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevionGames.InventorySystem.Restrictions
{
    public class EquipmentRegion : Restriction
    {
        public DevionGames.InventorySystem.EquipmentRegion region;

        public override bool CanAddItem(Item item)
        {
            if (region == null) {
                Debug.LogWarning("The restriction EquipmentRegion has a null reference. This can happen when you delete the region in database but not update your slots. Remove the restriction or add a reference.");
                return true; 
            }
            if (item == null || !(item is EquipmentItem equipmentItem)) { return false; }

            List<DevionGames.InventorySystem.EquipmentRegion> requiredRegions = new List<DevionGames.InventorySystem.EquipmentRegion>(equipmentItem.Region);

            Restrictions.EquipmentRegion[] restrictions = GetComponents<Restrictions.EquipmentRegion>();
            for (int i = requiredRegions.Count - 1; i >= 0; i--)
            {
                if (restrictions.Select(x => x.region.Name).Contains(requiredRegions[i].Name))
                {
                    return true;
                }
            }
            return false;
        }
    }
}