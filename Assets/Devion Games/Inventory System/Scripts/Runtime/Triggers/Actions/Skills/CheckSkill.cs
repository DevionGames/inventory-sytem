using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [Icon("Item")]
    [ComponentMenu("Inventory System/Check Skill")]
    public class CheckSkill : Action, ICondition
    {
        [Tooltip("The name of the window to lock.")]
        [SerializeField]
        private string m_WindowName = "Skills";

        [ItemPicker]
        [SerializeField]
        private Skill m_Skill= null;
        [SerializeField]
        private NotificationOptions m_SuccessNotification = null;
        [SerializeField]
        private NotificationOptions m_FailureNotification = null;

        private ItemContainer m_ItemContainer;

        public override void OnStart()
        {
            this.m_ItemContainer = WidgetUtility.Find<ItemContainer>(this.m_WindowName);
        }

        public override ActionStatus OnUpdate()
        {
            if (this.m_ItemContainer == null)
            {
                Debug.LogWarning("Missing window " + this.m_WindowName + " in scene!");
                return ActionStatus.Failure;
            }

            Skill current = (Skill)this.m_ItemContainer.GetItems(this.m_Skill.Id).FirstOrDefault();
            if(current != null){
                if (!current.CheckSkill()) {
                    if (this.m_FailureNotification != null && !string.IsNullOrEmpty(this.m_FailureNotification.text))
                        this.m_FailureNotification.Show();
                    return ActionStatus.Failure;
                }
            }
            if (this.m_SuccessNotification != null && !string.IsNullOrEmpty(this.m_SuccessNotification.text))
                this.m_SuccessNotification.Show();
            return ActionStatus.Success;
        }
    }
}
