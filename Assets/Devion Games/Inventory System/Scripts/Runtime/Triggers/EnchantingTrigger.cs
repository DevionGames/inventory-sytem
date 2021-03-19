using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class EnchantingTrigger : CraftingTrigger
    {

        public override bool OverrideUse(Slot slot, Item item)
        {
            if (!slot.MoveItem())
            {
                StartCrafting(item, 1);
            }
            return true;
        }

        protected override CraftingRecipe GetCraftingRecipe(Item item)
        {
            return item.EnchantingRecipe;
        }

        protected override IEnumerator CraftItem(Item item)
        {
            CraftingRecipe recipe = GetCraftingRecipe(item);
            this.m_ProgressDuration = recipe.Duration;
            this.m_ProgressInitTime = Time.time;
            yield return new WaitForSeconds(recipe.Duration);

            if (recipe.Skill != null)
            {
                Skill skill = ItemContainer.GetItem(recipe.Skill.Id) as Skill;
                if (skill != null && !skill.CheckSkill())
                {
                    NotifyFailedToCraft(item, FailureCause.Unknown);
                    if (recipe.RemoveIngredientsWhenFailed)
                    {
                        for (int j = 0; j < recipe.Ingredients.Count; j++)
                        {
                            this.m_RequiredIngredientsContainer.RemoveItem(recipe.Ingredients[j].item, recipe.Ingredients[j].amount);
                        }
                    }
                    yield break;
                }

            }

            recipe.CraftingModifier.Modify(item);
            for (int i = 0; i < item.ReferencedSlots.Count; i++) {
                item.ReferencedSlots[i].Repaint();
            }

            for (int i = 0; i < recipe.Ingredients.Count; i++)
            {
                this.m_RequiredIngredientsContainer.RemoveItem(recipe.Ingredients[i].item, recipe.Ingredients[i].amount);
            }
            NotifyItemCrafted(item);
        }

        protected override void NotifyAlreadyCrafting(Item item)
        {
            InventoryManager.Notifications.alreadyEnchanting.Show();
            ExecuteEvent<ITriggerFailedCraftStart>(Execute, item, FailureCause.InUse);
        }

        protected override void NotifyMissingIngredients(Item item)
        {
            InventoryManager.Notifications.missingMaterials.Show();
            ExecuteEvent<ITriggerFailedCraftStart>(Execute, item, FailureCause.Requirement);
        }

        protected override void NotifyItemCrafted(Item item)
        {
            InventoryManager.Notifications.enchantedItem.Show(UnityTools.ColorString(item.Name, item.Rarity.Color));
            ExecuteEvent<ITriggerCraftItem>(Execute, item);
        }

        protected override void NotifyFailedToCraft(Item item, FailureCause cause)
        {
            switch (cause)
            {
                case FailureCause.ContainerFull:
                    InventoryManager.Notifications.containerFull.Show(this.m_ResultStorageContainer.Name);
                    ExecuteEvent<ITriggerFailedToCraftItem>(Execute, item, FailureCause.ContainerFull);
                    break;
                case FailureCause.Unknown:
                    InventoryManager.Notifications.failedToEnchant.Show(item.DisplayName);
                    ExecuteEvent<ITriggerFailedToCraftItem>(Execute, item, FailureCause.Unknown);
                    break;
            }
        }

    }
}