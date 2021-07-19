using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.InventorySystem
{
    public class DescriptionItemView : ItemView
    {
        /// <summary>
        /// The text reference to display item description.
        /// </summary>
        [Tooltip("The text reference to display item description")]
        [SerializeField]
        protected Text m_Description;

        public override void Repaint(Item item)
        {
            if (this.m_Description != null)
            {
                this.m_Description.text = (item != null ? item.Description : string.Empty);
            }

        }
    }
}