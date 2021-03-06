using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [System.Serializable]
    public class ItemCondition
    {
        public Item item;
        public Category category;
        public bool boolValue;
        public string stringValue;
        public int intValue;
    }
}
