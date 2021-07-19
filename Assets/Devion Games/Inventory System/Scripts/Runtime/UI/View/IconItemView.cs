using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.InventorySystem
{
    public class IconItemView : ItemView
    {
        /// <summary>
        /// The Image to display item icon.
        /// </summary>
        [Tooltip("Image reference to display the icon.")]
        [SerializeField]
        protected Image m_Ícon;

        public override void Repaint(Item item)
        {
            if (this.m_Ícon != null)
            {
                if (item != null)
                {
                    //Updates the icon and enables it.
                    this.m_Ícon.overrideSprite = item.Icon;
                    this.m_Ícon.enabled = true;
                }
                else
                {
                    //If there is no item in this slot, disable icon
                    this.m_Ícon.enabled = false;
                }
            }
        }
    }
}