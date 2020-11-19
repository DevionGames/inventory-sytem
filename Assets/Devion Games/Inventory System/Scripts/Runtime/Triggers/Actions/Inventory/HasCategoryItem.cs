using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Condition Item")]
    [ComponentMenu("Inventory System/Has Category Item")]
    public class HasCategoryItem : Action, ICondition
    {
        [SerializeField]
        protected ItemCondition[] requiredItems;

        public override ActionStatus OnUpdate()
        {
            for (int i = 0; i < requiredItems.Length; i++)
            {
                ItemCondition condition = requiredItems[i];
                if (condition.category != null && !string.IsNullOrEmpty(condition.stringValue)) { 

                    if (!ItemContainer.HasCategoryItem(condition.stringValue,condition.category))
                    {
                        if (InventoryManager.UI.notification != null)
                        {
                            InventoryManager.UI.notification.AddItem(InventoryManager.Notifications.missingCategoryItem,condition.category.Name,condition.stringValue);
                        }
                        return ActionStatus.Failure;
                    }
                }
            }

            return ActionStatus.Success;
        }
    }
}