using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class PriceItemView : ItemView
    {
        /// <summary>
        /// Item Container that will show the price of item.
        /// </summary>
        [Tooltip("ItemContainer reference to display the price.")]
        [SerializeField]
        protected ItemContainer m_Price;
        [Tooltip("What price should be displayed?")]
        [SerializeField]
        protected PriceType m_PriceType = PriceType.Buy;

        public override void Repaint(Item item)
        {
            if (this.m_Price != null)
            {
                this.m_Price.RemoveItems();
                if (item != null && item.IsSellable)
                {
                    this.m_Price.gameObject.SetActive(true);
                    Currency price = Instantiate(this.m_PriceType == PriceType.Buy ? item.BuyCurrency : item.SellCurrency);
                    price.Stack = Mathf.RoundToInt(this.m_PriceType == PriceType.Buy ? item.BuyPrice : item.SellPrice);
                    this.m_Price.StackOrAdd(price);
                }
                else {
                    this.m_Price.gameObject.SetActive(false);
                }
            }
        }

        public enum PriceType { 
            Buy,
            Sell
        }
    }
}