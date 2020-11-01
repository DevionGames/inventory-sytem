using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class ItemModifierHandler : MonoBehaviour
    {
        public List<ItemModifier> modifiers = new List<ItemModifier>();

        public void ApplyModifiers(Item item) {
            for (int i = 0; i < modifiers.Count; i++) {
                modifiers[i].Modify(item);
            }
        }

        public void ApplyModifiers(Item[] items)
        {
            for (int i = 0; i < modifiers.Count; i++)
            {
                for (int j = 0; j < items.Length; j++)
                {
                    modifiers[i].Modify(items[j]);
                }
            }
        }
    }
}