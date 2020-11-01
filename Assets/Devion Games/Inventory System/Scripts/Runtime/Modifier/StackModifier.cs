using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CreateAssetMenu(fileName = "SimpleStackModifier", menuName = "Devion Games/Inventory System/Modifiers/Stack")]
    [System.Serializable]
    public class StackModifier : ItemModifier
    {
        [SerializeField]
        protected int m_Min = 1;
        [SerializeField]
        protected int m_Max = 2;

        public override void Modify(Item item)
        {
            int stack = Random.Range(this.m_Min, this.m_Max);
            item.Stack = stack;
        }
    }
}