using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem.Restrictions
{
    public class Category : Restriction
    {
        [CategoryPicker(true)]
        [SerializeField]
        private DevionGames.InventorySystem.Category[] m_Categories = null;
        [SerializeField]
        private bool invert = false;
        public override bool CanAddItem(Item item)
        {
            for (int i = 0; i < this.m_Categories.Length; i++)
            {
                if (this.m_Categories[i].IsAssignable(item.Category))
                {
                    return !invert;
                }
            }

           /* for (int i = 0; i < this.m_Categories.Length; i++) {
                if (item.Category != null && item.Category.Name == m_Categories[i].Name) {
                    return !invert;
                }
            }*/
            return invert;
        }
    }
}