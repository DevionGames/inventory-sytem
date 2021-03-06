using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomPropertyDrawer(typeof(CraftingRecipe))]
    public class CraftingRecipeDrawer : PickerDrawer<CraftingRecipe>
    {
        protected override List<CraftingRecipe> GetItems(ItemDatabase database)
        {
            return database.craftingRecipes;
        }
    }
}