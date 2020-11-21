
using System.Linq;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [ComponentMenu("Stat System/Add Modifier")]
    public class RemoveModifier : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;

        [InspectorLabel("Stat")]
        [SerializeField]
        protected string m_StatName="Critical Strike";

        private StatsHandler m_Handler;

        public override void OnStart()
        {
            this.m_Handler = this.m_Target == TargetType.Self ? gameObject.GetComponent<StatsHandler>() : playerInfo.gameObject.GetComponent<StatsHandler>();
        }

        public override ActionStatus OnUpdate()
        {
            this.m_Handler.RemoveModifiersFromSource(this.m_StatName, this.m_Handler.gameObject);
            return ActionStatus.Success;
        }


    }
}
