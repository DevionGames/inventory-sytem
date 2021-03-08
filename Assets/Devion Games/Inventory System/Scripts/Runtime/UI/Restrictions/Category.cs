using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DevionGames.InventorySystem.Restrictions
{
    public class Category : Restriction
    {
        [SerializeField]
        private DevionGames.InventorySystem.Category[] m_Categories = null;
        [SerializeField]
        private bool invert = false;
        public override bool CanAddItem(Item item)
        {

            if (this.m_Categories.Contains(null))
            {
                Debug.LogWarning("The restriction Category has a null reference. This can happen when you delete the category in database but not update your slots/container. Remove the restriction or add a reference.");
                return true;
            }

            if (item == null) { return false; }

            for (int i = 0; i < this.m_Categories.Length; i++)
            {
                if (this.m_Categories[i].IsAssignable(item.Category))
                {
                    return !invert;
                }
            }
            return invert;
        }
    }
}