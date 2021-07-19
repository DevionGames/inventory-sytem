using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.InventorySystem
{
    public class StackItemView : ItemView
    {
        /// <summary>
        ///The text to display item stack.
        /// </summary>
        [Tooltip("The text reference to display item stack.")]
        [SerializeField]
        protected Text m_Stack;

        protected override void Start()
        {
            if (this.m_Stack != null)
                this.m_Stack.raycastTarget = false;
        }

        public override void Repaint(Item item)
        {
            if (this.m_Stack != null)
            {
                if (item != null && item.MaxStack > 1)
                {
                    //Updates the stack and enables it.
                    this.m_Stack.text = item.Stack.ToString();
                    this.m_Stack.enabled = true;
                }
                else
                {
                    //If there is no item in this slot, disable stack field
                    this.m_Stack.enabled = false;
                }
            }
        }
    }
}