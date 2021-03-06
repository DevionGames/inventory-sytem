using System.Linq;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [Icon("Item")]
    [ComponentMenu("Inventory System/Compare Skill")]
    public class CompareSkill : Action, ICondition
    {
        [SerializeField]
        private Skill m_Skill = null;
        /*[Tooltip("The name of the skills window.")]
        [SerializeField]
        private string m_SkillWindow = "Skills";*/
        [SerializeField]
        private ConditionType m_Condition = ConditionType.Greater;
        [Range(0f,100f)]
        [SerializeField]
        private float m_Value = 0f;
        [SerializeField]
        private NotificationOptions m_SuccessNotification = null;
        [SerializeField]
        private NotificationOptions m_FailureNotification = null;

        public override ActionStatus OnUpdate()
        {
            Skill skill = ItemContainer.GetItem(this.m_Skill.Id) as Skill;
            if (skill != null && Compare(skill.CurrentValue)) {
                if (this.m_SuccessNotification != null && !string.IsNullOrEmpty(this.m_SuccessNotification.text))
                    this.m_SuccessNotification.Show();
                return ActionStatus.Success;
            }

            if (this.m_FailureNotification != null && !string.IsNullOrEmpty(this.m_FailureNotification.text))
                this.m_FailureNotification.Show();
            return ActionStatus.Failure;
        }

        private bool Compare(float value) {
            switch (this.m_Condition)
            {
                case ConditionType.Greater:
                    return value > this.m_Value;
                case ConditionType.GreaterOrEqual:
                    return value >= this.m_Value;
                case ConditionType.Less:
                    return value < this.m_Value;
                case ConditionType.LessOrEqual:
                    return value <= this.m_Value;
            }
            return false;
        }
    }
}
