using System.Linq;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [Icon("Item")]
    [ComponentMenu("Inventory System/Check Skill")]
    public class CheckSkill : Action, ICondition
    {
        [SerializeField]
        private Skill m_Skill= null;
        [SerializeField]
        private NotificationOptions m_SuccessNotification = null;
        [SerializeField]
        private NotificationOptions m_FailureNotification = null;

        public override ActionStatus OnUpdate()
        {
            Skill current = ItemContainer.GetItem(this.m_Skill.Id) as Skill;
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
