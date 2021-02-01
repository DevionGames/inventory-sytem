
using System.Linq;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [ComponentMenu("Stat System/Subtract Value")]
    [System.Serializable]
    public class SubtractValue : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;

        [InspectorLabel("Stat")]
        [SerializeField]
        protected string m_StatName="Vitality";
        [SerializeField]
        protected float m_Value = 1f;
       
        private StatsHandler m_Handler;

        public override void OnStart()
        {
            this.m_Handler = this.m_Target == TargetType.Self ? gameObject.GetComponent<StatsHandler>() : playerInfo.gameObject.GetComponent<StatsHandler>();
        }

        public override ActionStatus OnUpdate()
        {
            Stat stat = this.m_Handler.GetStat(this.m_StatName);
            stat.Subtract(this.m_Value);
            return ActionStatus.Success;
        }


    }
}
