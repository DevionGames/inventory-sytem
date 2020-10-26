using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Lock All")]
    public class LockAll : Action
    {
        [SerializeField]
        private bool m_State = true;

        public override ActionStatus OnUpdate()
        {
            ItemContainer[] containers = GameObject.FindObjectsOfType<ItemContainer>();
            for (int i = 0; i < containers.Length; i++)
            {
                containers[i].Lock(this.m_State);
            }

            return ActionStatus.Success;
        }
    }
}
