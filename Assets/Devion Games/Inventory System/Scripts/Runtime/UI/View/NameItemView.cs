using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.InventorySystem
{
    public class NameItemView : ItemView
    {
        /// <summary>
        /// The text reference to display item name.
        /// </summary>
        [Tooltip("Text reference to display item name.")]
        [InspectorLabel("Name")]
        [SerializeField]
        protected Text m_ItemName;
        /// <summary>
        /// Should the item name be colored in rarity color?
        /// </summary>
        [Tooltip("Should the name use rarity color?")]
        [SerializeField]
        protected bool m_UseRarityColor = true;

        public override void Repaint(Item item)
        {
            if (this.m_ItemName != null)
            {
                //Updates the text with item name and rarity color. If this slot is empty, sets the text to empty.
                this.m_ItemName.text = (item != null ? (this.m_UseRarityColor ? UnityTools.ColorString(item.DisplayName, item.Rarity.Color) : item.DisplayName) : string.Empty);
            }
        }
    }
}