using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public static class NotificationExtension
    {
        public static void Show(this NotificationOptions options, params string[] replacements)
        {
            if (InventoryManager.UI.notification != null)
            {
                InventoryManager.UI.notification.AddItem(options, replacements);
            }
        }
    }
}

namespace DevionGames.InventorySystem.Configuration
{
    [System.Serializable]
    public class Notifications : Settings
    {

        public override string Name
        {
            get
            {
                return "Notification";
            }
        }
        [Header("Container:")]
        public NotificationOptions containerFull = new NotificationOptions()
        {
            text = "{0} is full!"
        };

        public NotificationOptions failedRestriction = new NotificationOptions()
        {
            text = "{0} can't be added to {1}."
        };

        public NotificationOptions missingItem = new NotificationOptions()
        {
            text = "This action requires {0}."
        };

        public NotificationOptions missingCategoryItem = new NotificationOptions()
        {
            text = "This action requires {0}."
        };

        public NotificationOptions inCooldown = new NotificationOptions()
        {
            text = "{0} is in cooldown for {1} seconds!"
        };

        [Header("Crafting:")]
        public NotificationOptions selectItem = new NotificationOptions()
        {
            text = "Select an item you would like to craft."
        };
        public NotificationOptions alreadyCrafting = new NotificationOptions()
        {
            text = "You are already crafting."
        };
        public NotificationOptions craftedItem = new NotificationOptions()
        {
            text = "Successfully crafted {0}"
        };
        public NotificationOptions missingIngredient = new NotificationOptions()
        {
            text = "You don't have all ingredients to craft this item!"
        };
        public NotificationOptions failedToCraft = new NotificationOptions()
        {
            text = "You failed to craft {0}."
        };
        public NotificationOptions missingSkillToCraft = new NotificationOptions()
        {
            text = "You don't have the skill to craft this item!"
        };
        public NotificationOptions requiresHigherSkill = new NotificationOptions()
        {
            text = "Your {0} is to low to craft {1}."
        };


        [Header("Vendor:")]
        public NotificationOptions soldItem = new NotificationOptions()
        {
            text = "Sold {0} for {1}."
        };
        public NotificationOptions boughtItem = new NotificationOptions()
        {
            text = "Bought {0} for {1}."
        };
        public NotificationOptions noCurrencyToBuy = new NotificationOptions()
        {
            text = "You don't have enough coins."
        };

        public NotificationOptions cantSellItem = new NotificationOptions()
        {
            text = "You can't sell this {0}!"
        };

        [Header("Trigger:")]
        public NotificationOptions toFarAway = new NotificationOptions()
        {
            text = "This is to far away!"
        };
        public NotificationOptions inUse = new NotificationOptions()
        {
            text = "My life is already fairly busy."
        };
        public NotificationOptions empty = new NotificationOptions()
        {
            text = "There is nothing to be found here."
        };

        [Header("Skills:")]
        public NotificationOptions skillGain = new NotificationOptions()
        {
            text = "Your {0} increased by {1}% to {2}%."
        };
    }
}