using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class RecipeItemView : ItemView
    {
        /// <summary>
        /// Item container that will show crafting ingredients of the item
        /// </summary>
        [Tooltip("ItemContainer that will show crafting ingredients of the item.")]
        [SerializeField]
        protected ItemContainer m_Ingredients;

        [Tooltip("What recipe should be displayed?")]
        [SerializeField]
        protected RecipeType m_RecipeType = RecipeType.Crafting;

        public override void Repaint(Item item)
        {
            if (this.m_Ingredients != null)
            {
                this.m_Ingredients.RemoveItems();

                if (item != null)
                {
                    CraftingRecipe recipe = this.m_RecipeType == RecipeType.Crafting ? item.CraftingRecipe : item.EnchantingRecipe;
                    if (recipe != null)
                    {
                        this.m_Ingredients.gameObject.SetActive(true);
                        for (int i = 0; i < recipe.Ingredients.Count; i++)
                        {
                            Item ingredient = Instantiate(recipe.Ingredients[i].item);
                            ingredient.Stack = recipe.Ingredients[i].amount;
                            this.m_Ingredients.StackOrAdd(ingredient);
                        }
                    } else {
                        this.m_Ingredients.gameObject.SetActive(false);
                    }
                }
            }
        }

        public enum RecipeType { 
            Crafting,
            Enchanting
        }
    }
}