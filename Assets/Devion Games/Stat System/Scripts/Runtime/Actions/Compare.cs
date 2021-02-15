using UnityEngine;

namespace DevionGames.StatSystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [ComponentMenu("Stat System/Compare")]
    [System.Serializable]
    public class Compare : Action, ICondition
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;

        [InspectorLabel("Stat")]
        [SerializeField]
        protected string m_StatName = "Health";

        [InspectorLabel("Type")]
        [SerializeField]
        protected ValueType m_ValueType = ValueType.CurrentValue;

        [SerializeField]
        protected ConditionType m_Condition = ConditionType.Greater;

        [SerializeField]
        protected float m_Value;

        private StatsHandler m_Handler;

        public override void OnStart()
        {
            this.m_Handler = this.m_Target == TargetType.Self ? gameObject.GetComponent<StatsHandler>() : playerInfo.gameObject.GetComponent<StatsHandler>();
        }

        public override ActionStatus OnUpdate()
        {
            Stat stat = this.m_Handler.GetStat(this.m_StatName) as Stat;
            if (stat == null) return ActionStatus.Failure;

            float value = stat.Value;
            if (this.m_ValueType == ValueType.CurrentValue)
                value = (stat as Attribute).CurrentValue;

            switch (this.m_Condition) {
                case ConditionType.Greater:
                    return value > this.m_Value ? ActionStatus.Success : ActionStatus.Failure;
                case ConditionType.GreaterOrEqual:
                    return value >= this.m_Value ? ActionStatus.Success : ActionStatus.Failure;
                case ConditionType.Less:
                    return value < this.m_Value ? ActionStatus.Success : ActionStatus.Failure;
                case ConditionType.LessOrEqual:
                    return value <= this.m_Value ? ActionStatus.Success : ActionStatus.Failure;
            }

            return ActionStatus.Failure;
        }
    }
}