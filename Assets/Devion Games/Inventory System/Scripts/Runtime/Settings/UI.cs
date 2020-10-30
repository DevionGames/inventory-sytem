using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEngine;
using UnityEngine.Assertions;

namespace DevionGames.InventorySystem.Configuration
{
    [System.Serializable]
    public class UI : Settings
    {
        public override string Name
        {
            get
            {
                return "UI";
            }
        }

        [InspectorLabel("Context Menu","Name of ContextMenu widget.")]
        public string contextMenuName = "ContextMenu";
        [InspectorLabel("Tooltip", "Name of Tooltip widget.")]
        public string tooltipName = "Tooltip";
        [InspectorLabel("Price Tooltip", "Name of sell price tooltip widget.")]
        public string sellPriceTooltipName = "Sell Price Tooltip";
        [InspectorLabel("Stack", "Name of Stack widget.")]
        public string stackName = "Stack";
        [InspectorLabel("Notification", "Name of Notification widget.")]
        public string notificationName = "Notification";

        private Notification m_Notification;
        public Notification notification {
            get {
                if (this.m_Notification == null) {
                    this.m_Notification = WidgetUtility.Find<Notification>(this.notificationName);
                }
                Assert.IsNotNull(this.m_Notification, "Notification widget with name "+this.notificationName+" is not present in scene.");
                return this.m_Notification;
            }
        }

        private Tooltip m_Tooltip;
        public Tooltip tooltip
        {
            get
            {
                if (this.m_Tooltip == null)
                {
                    this.m_Tooltip = WidgetUtility.Find<Tooltip>(this.tooltipName);
                }
                Assert.IsNotNull(this.m_Tooltip, "Tooltip widget with name " + this.tooltipName + " is not present in scene.");
                return this.m_Tooltip;
            }
        }

        private ItemContainer m_SellPriceTooltip;
        public ItemContainer sellPriceTooltip
        {
            get
            {
                if (this.m_SellPriceTooltip == null)
                {
                    this.m_SellPriceTooltip = WidgetUtility.Find<ItemContainer>(this.sellPriceTooltipName);
                }
                return this.m_SellPriceTooltip;
            }
        }

        private Stack m_Stack;
        public Stack stack
        {
            get
            {
                if (this.m_Stack == null)
                {
                    this.m_Stack = WidgetUtility.Find<Stack>(this.stackName);
                }
                Assert.IsNotNull(this.m_Stack, "Stack widget with name " + this.stackName + " is not present in scene.");
                return this.m_Stack;
            }
        }

        private UIWidgets.ContextMenu m_ContextMenu;
        public UIWidgets.ContextMenu contextMenu
        {
            get
            {
                if (this.m_ContextMenu == null)
                {
                    this.m_ContextMenu = WidgetUtility.Find<UIWidgets.ContextMenu>(this.contextMenuName);
                }
                Assert.IsNotNull(this.m_ContextMenu, "ConextMenu widget with name " + this.contextMenuName + " is not present in scene.");
                return this.m_ContextMenu;
            }
        }
    }
}