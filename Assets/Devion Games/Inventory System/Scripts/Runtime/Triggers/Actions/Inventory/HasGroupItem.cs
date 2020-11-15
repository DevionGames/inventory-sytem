using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Condition Item")]
    [ComponentMenu("Inventory System/Has Group Item")]
    public class HasGroupItem : Action, ICondition
    {
        [ItemGroupPicker]
        [SerializeField]
        protected ItemGroup m_RequiredGroupItem;
        [SerializeField]
        protected string m_Window="Equipment";


        public override ActionStatus OnUpdate()
        {
            for (int i = 0; i < this.m_RequiredGroupItem.Items.Length; i++)
            {
                Item item = this.m_RequiredGroupItem.Items[i];
                if (item != null && !string.IsNullOrEmpty(this.m_Window)) { 

                    if (ItemContainer.HasItem(this.m_Window, item, 1))
                    {
                        
                        return ActionStatus.Success;
                    }
                }
            }

            return ActionStatus.Failure;
        }
    }
}