using UnityEngine;

namespace DevionGames.StatSystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [ComponentMenu("Stat System/Refresh")]
    [System.Serializable]
    public class Refresh : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;

        [InspectorLabel("Stat")]
        [SerializeField]
        protected string m_StatName = "Health";
       
        private StatsHandler m_Handler;

        public override void OnStart()
        {
            this.m_Handler = this.m_Target == TargetType.Self ? gameObject.GetComponent<StatsHandler>() : playerInfo.gameObject.GetComponent<StatsHandler>();
        }

        public override ActionStatus OnUpdate()
        {
            Attribute stat = this.m_Handler.GetStat(this.m_StatName) as Attribute;
            if (stat == null) return ActionStatus.Failure;

            stat.CurrentValue = stat.Value;
            return ActionStatus.Success;
        }


    }
}